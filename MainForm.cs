using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Aryes
{
    internal partial class MainForm : Form
    {
        private static bool Pause { get; set; }

        private const int BufferSize = 65536;
        private const int DesiredCompression = 75;
        private static long _reclaimed;
        private static string _mask;
        private static int _clusterSize = 4096;
        private readonly int[] _actionCount = new int[4];
        private Hashtable _blacklist;

        internal MainForm()
        {
            InitializeComponent();
        }

        private static string BlackListFile => Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "blacklist.txt");

        private void Form1_Load(object sender, EventArgs e)
        {
            worker.DoWork += (o, a) => Traverse(a.Argument as string);
            worker.ProgressChanged += (o, a) =>
                                          {
                                              var s = a.UserState as string;
                                              if (s != null)
                                                  label1.Text = s;
                                          };
            worker.RunWorkerCompleted += (o, a) =>
                                             {
                                                 SaveBlackList();
                                                 startButton.Enabled = true;
                                                 statusLabel.Text = "Done";
                                             };
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            var path = textBox1.Text;
            startButton.Enabled = false;
            pauseButton.Enabled = true;
            stopButton.Enabled = true;
            _mask = txtMask.Text.Length != 0 ? txtMask.Text : "*";
            _clusterSize = NTFSCompression.ClusterSize(Path.GetPathRoot(path));
            LoadBlackList();
            foreach (CompressAction action in Enum.GetValues(typeof(CompressAction)))
                _actionCount[(int)action] = 0;
            _reclaimed = 0;
            worker.RunWorkerAsync(path);
            statusLabel.Text = "Running";
        }

        private void pauseButton_Click(object sender, EventArgs e)
        {
            Pause = !Pause;
            statusLabel.Text = Pause ? "Paused" : "Running";
        }


        private void stopButton_Click(object sender, EventArgs e)
        {
            startButton.Enabled = true;
            pauseButton.Enabled = false;
            stopButton.Enabled = false;
            worker.CancelAsync();
            Pause = false;
        }

        private void Compress(string filename)
        {
            var action = CompressAction.None;
            try
            {
                var uSize = Allocated(new FileInfo(filename).Length);
                long cSize;

                var isCompressed = NTFSCompression.IsCompressed(filename);
                if (!isCompressed)
                {
                    if (uSize > _clusterSize)
                    {
                        cSize = SampleRatio(filename);
                        if (uSize < 3 * BufferSize || cSize < DesiredCompression)
                        {
                            NTFSCompression.Compress(filename);
                            action = CompressAction.Compress;
                            isCompressed = true;
                        }
                        else
                        {
                            action = CompressAction.Skip;
                        }
                    }
                }
                cSize = Allocated(NTFSCompression.FileSize(filename));
                if (cSize >= uSize && isCompressed)
                {
                    NTFSCompression.Uncompress(filename);
                    _blacklist.Add(filename, filename);
                    action = action == CompressAction.Compress ? CompressAction.None : CompressAction.Decompress;
                }
                _actionCount[(int)action]++;

                if (action == CompressAction.None) return;
                if (uSize > cSize)
                    _reclaimed += uSize - cSize;
                var s = new StringBuilder();
                foreach (var a in Enum.GetValues(typeof(CompressAction)).Cast<CompressAction>())
                    s.AppendFormat("{0} {1}{2}", a, _actionCount[(int)a], Environment.NewLine);
                s.AppendFormat("{0:#,##0} bytes reclaimed.{1}{1}", _reclaimed, Environment.NewLine);
                s.AppendFormat("{0}", filename);
                worker.ReportProgress(0, s.ToString());
            }
            catch
            {
                //log?
            }
        }

        private static int SampleRatio(string filename)
        {
            var buffer = new byte[BufferSize]; //TODO: parametrize -- 64KB for now
            var fsIn = File.OpenRead(filename);
            string tempFile;
            do
            {
                tempFile = Path.Combine(Environment.GetEnvironmentVariable("temp") ?? Environment.CurrentDirectory, Guid.NewGuid().ToString());
            } while (File.Exists(tempFile));
            var fsOut = File.Create(tempFile);
            var offset = fsIn.Length / 2 / _clusterSize * _clusterSize;
            fsIn.Seek(offset, SeekOrigin.Begin);
            fsIn.Read(buffer, 0, buffer.Length);
            fsOut.Write(buffer, 0, buffer.Length);
            fsIn.Close();
            fsOut.Close();
            NTFSCompression.Compress(tempFile);
            var cSize = Allocated(NTFSCompression.FileSize(tempFile));
            try
            {
                File.Delete(tempFile);
            }
            catch
            {
                //log?
            }
            return (int)cSize / BufferSize * 100;
        }

        private static long Allocated(long size)
        {
            return (size / _clusterSize + (size % _clusterSize != 0 ? 1 : 0)) * _clusterSize;
        }

        private void LoadBlackList()
        {
            _blacklist = new Hashtable();
            if (!File.Exists(BlackListFile)) return;
            var reader = File.OpenText(BlackListFile);
            while (true)
            {
                var key = reader.ReadLine();
                if (key == null)
                {
                    reader.Close();
                    return;
                }
                _blacklist.Add(key, key);
            }
        }

        private void SaveBlackList()
        {
            var writer = File.CreateText(BlackListFile);
            var array = new string[_blacklist.Count];
            _blacklist.Values.CopyTo(array, 0);
            Array.Sort(array);
            foreach (var str in array)
                writer.WriteLine(str);
            writer.Close();
            NTFSCompression.Compress(BlackListFile);
        }

        private void Traverse(string path)
        {
            IEnumerable<string> files;
            try
            {
                files = Directory.EnumerateFiles(path, _mask);
            }
            catch
            {
                //TODO: Log?
                return;
            }
            foreach (var file in files.Select(o => o.ToLower()))
            {
                while (Pause && !worker.CancellationPending)
                    Thread.Sleep(100);
                if (worker.CancellationPending)
                    return;
                if (!SkipFile(file))
                    Compress(file);
            }
            foreach (var dir in Directory.EnumerateDirectories(path))
            {
                while (Pause && !worker.CancellationPending)
                    Thread.Sleep(100);
                if (worker.CancellationPending)
                    break;
                Traverse(Path.Combine(path, dir));
            }
        }

        private static Hashtable _skipExtensions;

        private bool SkipFile(string file)
        {
            if (_skipExtensions == null)
            {
                _skipExtensions = new Hashtable();
                foreach (var ex in new[] { "jpg", "jpeg", "png", "gif", "7z", "zip", "ods", "odt", "xlsx", "docx" })
                    _skipExtensions.Add(ex, ex);
            }
            var ext = Path.GetExtension(file);
            ext = !string.IsNullOrEmpty(ext) ? ext.Substring(1) : string.Empty;
            return _blacklist.ContainsValue(file) || _skipExtensions.Contains(ext);
        }

        private void openButton_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.SelectedPath = textBox1.Text;
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                textBox1.Text = folderBrowserDialog.SelectedPath;
        }

        private void optionButton_Click(object sender, EventArgs e)
        {
            new Form2().ShowDialog();
        }

        private void aboutButton_Click(object sender, EventArgs e)
        {
            new AboutForm().ShowDialog();
        }

        private enum CompressAction
        {
            None,
            Compress,
            Decompress,
            Skip
        }
    }
}