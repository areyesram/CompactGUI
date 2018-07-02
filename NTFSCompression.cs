using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace Aryes
{
    internal class NTFSCompression
    {
        #region API refs and consts

        private const int FileAttributeCompressed = 0x800;
        private const int FsctlSetCompression = 0x9c040;

        private const int CompressionFormatNone = 0;
        private const int CompressionFormatDefault = 1;
        private const int CompressionFormatLznt1 = 2;

        private const int FsPersistentAcls = 8;

        [DllImport("kernel32.dll")]
        private static extern int DeviceIoControl(SafeFileHandle hDevice, int dwIoControlCode, ref short lpInBuffer, int nInBufferSize, IntPtr lpOutBuffer, int nOutBufferSize, ref int lpBytesReturned, IntPtr lpOverlapped);
        internal static long DiskFree(string path)
        {
            var freeBytesAvailable = 0L;
            var totalBytes = 0L;
            var totalFreeBytes = 0L;
            GetDiskFreeSpaceEx(path, ref freeBytesAvailable, ref totalBytes, ref totalFreeBytes);
            return totalFreeBytes;
        }

        [DllImport("kernel32.dll")]
        public static extern int GetCompressedFileSize(string lpFileName, out int lpFileSizeHigh);

        [DllImport("Coredll.dll")]
        private static extern bool GetDiskFreeSpaceEx(string directoryName, ref long freeBytesAvailable, ref long totalBytes, ref long totalFreeBytes);

        [DllImport("kernel32.dll")]
        private static extern int GetFileAttributes(string name);

        [DllImport("kernel32.dll")]
        private static extern long GetVolumeInformation(string pathName, StringBuilder volumeNameBuffer, uint volumeNameSize, ref uint volumeSerialNumber, ref uint maximumComponentLength, ref uint fileSystemFlags, StringBuilder fileSystemNameBuffer, uint fileSystemNameSize);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool GetDiskFreeSpace(string lpRootPathName,
           out uint lpSectorsPerCluster,
           out uint lpBytesPerSector,
           out uint lpNumberOfFreeClusters,
           out uint lpTotalNumberOfClusters);

        #endregion

        internal static void Compress(string fileName)
        {
            SetCompression(fileName, CompressionFormatDefault);
        }

        internal static void Uncompress(string fileName)
        {
            SetCompression(fileName, CompressionFormatNone);
        }

        internal static bool IsCompressed(string filename)
        {
            return (GetFileAttributes(filename) & FileAttributeCompressed) != 0;
        }

        internal static long DiskSize(string path)
        {
            var freeBytesAvailable = 0L;
            var totalBytes = 0L;
            var totalFreeBytes = 0L;
            GetDiskFreeSpaceEx(path, ref freeBytesAvailable, ref totalBytes, ref totalFreeBytes);
            return totalBytes;
        }

        internal static int FileSize(string filename)
        {
            int num;
            return GetCompressedFileSize(filename, out num);
        }

        internal static bool IsNTFS(string path)
        {
            uint fsFlags = 0;
            uint maxCompLen = 0;
            uint vol = 0;

            var drive = Path.GetPathRoot(path);
            return GetVolumeInformation(drive, null, 0, ref vol, ref maxCompLen, ref fsFlags, null, 0) != 0 &&
                   (fsFlags & FsPersistentAcls) != 0;
        }

        internal static int ClusterSize(string drive)
        {
            uint sectorsPerCluster, bytesPerSector, numberOfFreeClusters, totalNumberOfClusters;
            GetDiskFreeSpace(drive, out sectorsPerCluster, out bytesPerSector,
                out numberOfFreeClusters, out totalNumberOfClusters);
            return Convert.ToInt32(sectorsPerCluster * bytesPerSector);
        }

        private static void SetCompression(string fileName, short compression)
        {
            var lpBytesReturned = 0;
            var lpInBuffer = compression;
            try
            {
                var stream = File.Open(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                DeviceIoControl(stream.SafeFileHandle, FsctlSetCompression, ref lpInBuffer, 2, IntPtr.Zero, 0, ref lpBytesReturned, IntPtr.Zero);
                stream.Close();
            }
            catch (Exception)
            {
            }
        }
    }
}