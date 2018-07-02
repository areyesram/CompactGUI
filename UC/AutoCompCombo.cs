using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Aryes.UC
{
    /// <summary>
    /// 
    /// </summary>
    public partial class AutoCompCombo : ComboBox
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="path"></param>
        public delegate void SelectedPathChangedEventHandler(object sender, string path);

        /// <summary>
        /// 
        /// </summary>
        public AutoCompCombo()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Recursive
        {
            get { return menuPathRecursive.Checked; }
            set { menuPathRecursive.Checked = value; }
        }

        private void EnableEditMenu()
        {
            var flag = SelectedText.Length != 0;
            menuPathCut.Enabled = flag;
            menuPathCopy.Enabled = flag;
            menuPathPaste.Enabled = Clipboard.ContainsText();
            menuPathDelete.Enabled = flag;
        }

        private void menuPathBrowse_Click(object sender, EventArgs e)
        {
            BrowseForDirectory();
        }

        /// <summary>
        /// 
        /// </summary>
        private void BrowseForDirectory()
        {
            folderBrowserDialog.SelectedPath = Text;
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                Text = folderBrowserDialog.SelectedPath;
        }

        private void menuPathCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(SelectedText);
        }

        private void menuPathCut_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(SelectedText);
            SelectedText = string.Empty;
        }

        private void menuPathDelete_Click(object sender, EventArgs e)
        {
            SelectedText = string.Empty;
        }

        private void menuPathPaste_Click(object sender, EventArgs e)
        {
            SelectedText = Clipboard.GetText();
        }

        private void menuPathRecursive_Click(object sender, EventArgs e)
        {
            menuPathRecursive.Checked = !menuPathRecursive.Checked;
        }

        private void menuPathSelect_Click(object sender, EventArgs e)
        {
            SelectionStart = 0;
            SelectionLength = Text.Length;
        }

        private void menuStripPath_Opening(object sender, CancelEventArgs e)
        {
            EnableEditMenu();
        }
    }
}