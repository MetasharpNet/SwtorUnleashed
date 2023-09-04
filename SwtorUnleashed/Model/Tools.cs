using System.Linq;
using Microsoft.VisualBasic.Devices;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SwtorUnleashed.Model
{
    public sealed class Tools
    {
        #region consts

        private const string NotNullOrEmptyError = "Must not be null or empty";

        #endregion

        #region GetDirectoryTotalSize

        public static long GetDirectoryTotalSize(string directoryPath)
        {
            #region args check

            if (String.IsNullOrEmpty(directoryPath))
                throw new ArgumentException(NotNullOrEmptyError, "directoryPath");

            #endregion
            Log.Method(directoryPath);
            if (!Directory.Exists(directoryPath))
            {
                Log.Info("Directory doesn't exist => size=0");
                return 0;
            }
            Log.Info("Directory exists");
            var size = GetDirectoryTotalSize(new DirectoryInfo(directoryPath));
            Log.Info("=> size=" + size);
            return size;
        }

        public static long GetDirectoryTotalSize(DirectoryInfo di)
        {
            #region args check

            if (di == null)
                throw new ArgumentNullException("di");

            #endregion
            Log.Method(di);
            long size = 0;
            try
            {
                var filesInfos = di.GetFiles();
                size += filesInfos.Sum(fi => fi.Length);
            }
            catch (Exception ex) { Log.Error(ex); }
            try
            {
                var directoriesInfos = di.GetDirectories();
                size += directoriesInfos.Sum(d => GetDirectoryTotalSize(d.FullName));
            }
            catch (Exception ex) { Log.Error(ex); }
            return size;
        }

        #endregion

        #region ShowError, ShowWarning

        public static void ShowError(string message, string caption = "Error")
        {
            #region args check

            if (message == null)
                message = "";

            #endregion
            Log.Error(message);
            MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void ShowWarning(string message, string caption = "Warning")
        {
            #region args check

            if (message == null)
                message = "";

            #endregion
            Log.Warning(message);
            MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        #endregion

        #region TryCreateDirectoryLink, TryCreateFileLink

        [DllImport("kernel32.dll")]
        private static extern bool CreateSymbolicLink(string lpSymlinkFileName, string lpTargetFileName, int dwFlags);

        private const int SymlinkFlagDir = 1;
        private const int SymlinkFlagFile = 0;

        public static bool TryCreateDirectoryLink(string linkDirectoryPath, string targetDirectoryPath)
        {
            #region args check

            if (String.IsNullOrEmpty(linkDirectoryPath))
                throw new ArgumentException(NotNullOrEmptyError, "linkDirectoryPath");
            if (String.IsNullOrEmpty(targetDirectoryPath))
                throw new ArgumentException(NotNullOrEmptyError, "targetDirectoryPath");

            #endregion
            Log.Method(linkDirectoryPath, targetDirectoryPath);
            try
            {
                CreateSymbolicLink(linkDirectoryPath, targetDirectoryPath, SymlinkFlagDir);
                return true;
            }
            catch (Exception ex ) { Log.Error(ex); }
            return false;
        }

        public static bool TryCreateFileLink(string linkFilePath, string targetFilePath)
        {
            #region args check

            if (String.IsNullOrEmpty(linkFilePath))
                throw new ArgumentException(NotNullOrEmptyError, "linkFilePath");
            if (String.IsNullOrEmpty(targetFilePath))
                throw new ArgumentException(NotNullOrEmptyError, "targetFilePath");

            #endregion
            Log.Method(linkFilePath, targetFilePath);
            try
            {
                CreateSymbolicLink(linkFilePath, targetFilePath, SymlinkFlagFile);
                return true;
            }
            catch (Exception ex ) { Log.Error(ex); }
            return false;
        }

        #endregion

        #region TryCopyDirectory, TryCopyFile

        public static bool TryCopyDirectory(string sourceDirectoryPath, string destinationDirectoryPath)
        {
            #region args check

            if (String.IsNullOrEmpty(sourceDirectoryPath))
                throw new ArgumentException(NotNullOrEmptyError, "sourceDirectoryPath");
            if (String.IsNullOrEmpty(destinationDirectoryPath))
                throw new ArgumentException(NotNullOrEmptyError, "sourceDirectoryPath");

            #endregion
            Log.Method(sourceDirectoryPath, destinationDirectoryPath);
            try
            {
                // create all of the directories
                foreach (string dirPath in Directory.GetDirectories(sourceDirectoryPath, "*", SearchOption.AllDirectories))
                    Directory.CreateDirectory(dirPath.Replace(sourceDirectoryPath, destinationDirectoryPath));
                // copy all the files
                foreach (string newPath in Directory.GetFiles(sourceDirectoryPath, "*.*", SearchOption.AllDirectories))
                    File.Copy(newPath, newPath.Replace(sourceDirectoryPath, destinationDirectoryPath));
                return true;
            }
            catch (Exception ex) { Log.Error(ex); }
            return false;
        }

        public static bool TryCopyFile(string sourceFilePath, string destinationFilePath)
        {
            #region args check

            if (String.IsNullOrEmpty(sourceFilePath))
                throw new ArgumentException(NotNullOrEmptyError, "sourceFilePath");
            if (String.IsNullOrEmpty(destinationFilePath))
                throw new ArgumentException(NotNullOrEmptyError, "sourceFilePath");

            #endregion
            Log.Method(sourceFilePath, destinationFilePath);
            try
            {
                File.Copy(sourceFilePath, destinationFilePath, true);
                return true;
            }
            catch (Exception ex) { Log.Error(ex); }
            return false;
        }

        #endregion

        #region TryCreateDirectory

        public static bool TryCreateDirectory(string directoryPath)
        {
            #region args check

            if (String.IsNullOrEmpty(directoryPath))
                throw new ArgumentException(NotNullOrEmptyError, "directoryPath");

            #endregion
            Log.Method(directoryPath);
            try
            {
                Directory.CreateDirectory(directoryPath);
                return true;
            }
            catch (Exception ex) { Log.Error(ex); }
            return false;
        }

        #endregion

        #region TryDeleteDirectory, TryDeleteFile

        public static bool TryDeleteDirectory(string directoryPath)
        {
            #region args check

            if (String.IsNullOrEmpty(directoryPath))
                throw new ArgumentException(NotNullOrEmptyError, "directoryPath");

            #endregion
            Log.Method(directoryPath);
            try
            {
                Directory.Delete(directoryPath, true);
                return true;
            }
            catch (Exception ex) { Log.Error(ex); }
            return false;
        }

        public static bool TryDeleteFile(string filePath)
        {
            #region args check

            if (String.IsNullOrEmpty(filePath))
                throw new ArgumentException(NotNullOrEmptyError, "filePath");

            #endregion
            Log.Method(filePath);
            try
            {
                File.Delete(filePath);
                return true;
            }
            catch (Exception ex) { Log.Error(ex); }
            return false;
        }

        #endregion

        #region TryMoveDirectory, TryMoveFile

        public static bool TryMoveDirectory(string sourceDirectory, string destinationDirectory)
        {
            #region args check

            if (String.IsNullOrEmpty(sourceDirectory))
                throw new ArgumentException(NotNullOrEmptyError, "sourceDirectory");
            if (String.IsNullOrEmpty(destinationDirectory))
                throw new ArgumentException(NotNullOrEmptyError, "destinationDirectory");

            #endregion
            Log.Method(sourceDirectory, destinationDirectory);
            try
            {
                Directory.Move(sourceDirectory, destinationDirectory);
                return true;
            }
            catch (Exception ex) { Log.Error(ex); }
            return false;
       }

        public static bool TryMoveFile(string sourceFilePath, string destinationFilePath)
        {
            #region args check

            if (String.IsNullOrEmpty(sourceFilePath))
                throw new ArgumentException(NotNullOrEmptyError, "sourceFilePath");
            if (String.IsNullOrEmpty(destinationFilePath))
                throw new ArgumentException(NotNullOrEmptyError, "destinationFilePath");

            #endregion
            Log.Method(sourceFilePath, destinationFilePath);
            try
            {
                File.Move(sourceFilePath, destinationFilePath);
                 return true;
            }
            catch (Exception ex) { Log.Error(ex); }
            return false;
        }

        #endregion

        #region IsReparsepoint

        /// <summary>
        /// Determine if a given file or directory is a reparsepoint.
        /// </summary>
        /// <param name="path">Full path to the file or directory to be tested.</param>
        /// <returns>True if the file or direcotry is a reparsepoint, false otherwise.</returns>
        public static bool IsReparsePoint(string path)
        {
            Log.Method(path);
            return ((File.GetAttributes(path) & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint);
        }

        #endregion

        #region DirectoryExists, FileExists

        public static bool DirectoryExists(string path)
        {
            Log.Method(path);
            if (String.IsNullOrEmpty(path))
                return false;

            try
            {
                return Directory.Exists(path);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return false;
            }
        }

        public static bool FileExists(string path)
        {
            Log.Method(path);
            
            if (String.IsNullOrEmpty(path))
                return false;

            try
            {
                return File.Exists(path);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return false;
            }
        }

        #endregion

        #region GetParentExplorerProcess

        public static Process GetParentExplorerProcess()
        {
            Log.Method();
            var p = GetParentProcess();
            while (!p.MainModule.FileName.ToLowerInvariant().EndsWith("explorer.exe"))
                p = GetParentProcess(p);
            return p;
        }
        
        #endregion

        #region GetParentProcess

        #region NtQueryInformationProcess

        [DllImport("ntdll.dll")]
        private static extern int NtQueryInformationProcess(IntPtr processHandle, int processInformationClass, ref ProcessBasicInformation processInformation, int processInformationLength, out int returnLength);

        #endregion

        /// <summary>
        /// Gets the parent process of the current process.
        /// </summary>
        /// <returns>An instance of the Process class.</returns>
        public static Process GetParentProcess()
        {
            Log.Method();
            return GetParentProcess(Process.GetCurrentProcess());
        }

        /// <summary>
        /// Gets the parent process of specified process.
        /// </summary>
        /// <param name="processId">The process id.</param>
        /// <returns>An instance of the Process class.</returns>
        public static Process GetParentProcess(int processId)
        {
            Log.Method(processId);
            var process = Process.GetProcessById(processId);
            return GetParentProcess(process);
        }

        /// <summary>
        /// Gets the parent process of a specified process.
        /// </summary>
        /// <param name="p">The process.</param>
        /// <returns>An instance of the Process class.</returns>
        public static Process GetParentProcess(Process p)
        {
            Log.Method(p);
            var pbi = new ProcessBasicInformation();
            int returnLength;
            int status = NtQueryInformationProcess(p.Handle, 0, ref pbi, Marshal.SizeOf(pbi), out returnLength);
            if (status != 0)
                throw new Win32Exception(status);

            try
            {
                return Process.GetProcessById(pbi.InheritedFromUniqueProcessId.ToInt32());
            }
            catch (ArgumentException) { }
            return null; // not found
        }

        #endregion

        #region GetAvailableRam, GetOccupiedRam, GetTotalRam (InMegaBytes)

        /// <summary>
        /// Return the available RAM in MB.
        /// </summary>
        /// <returns>Available RAM in MB</returns>
        public static int GetAvailableRamInMegaBytes()
        {
            Log.Method();
            var ci = new ComputerInfo();
            Log.Info((ci.AvailablePhysicalMemory / 1024 / 1024) + "MB");
            return (int)(ci.AvailablePhysicalMemory / 1024 / 1024);
        }

        /// <summary>
        /// Return the available paging RAM in MB.
        /// </summary>
        /// <returns>Available paging RAM in MB</returns>
        public static int GetAvailablePagingRamInMegaBytes()
        {
            Log.Method();
            Log.Info((GetWin32AvailablePagingRam() / 1024 / 1024) + "MB");
            return (int)(GetWin32AvailablePagingRam() / 1024 / 1024);
        }

        /// <summary>
        /// Return the occupied RAM in MB.
        /// </summary>
        /// <returns>Occupied RAM in MB</returns>
        public static int GetOccupiedRamInMegaBytes()
        {
            Log.Method();
            var ci = new ComputerInfo();
            Log.Info(((ci.TotalPhysicalMemory - ci.AvailablePhysicalMemory) / (1024 * 1024)) + "MB");
            return (int)((ci.TotalPhysicalMemory - ci.AvailablePhysicalMemory) / (1024 * 1024));
        }

        /// <summary>
        /// Return the occupied paging RAM in MB.
        /// </summary>
        /// <returns>Occupied paging RAM in MB</returns>
        public static int GetOccupiedPagingRamInMegaBytes()
        {
            Log.Method();
            Log.Info(((GetWin32TotalPagingRam() - GetWin32AvailablePagingRam()) / (1024 * 1024)) + "MB");
            return (int)((GetWin32TotalPagingRam() - GetWin32AvailablePagingRam()) / (1024 * 1024));
        }

        /// <summary>
        /// Return the total RAM in MB.
        /// </summary>
        /// <returns>Total RAM in MB</returns>
        public static int GetTotalRamInMegaBytes()
        {
            Log.Method();
            var ci = new ComputerInfo();
            Log.Info((ci.TotalPhysicalMemory / (1024 * 1024)) + "MB");
            return (int)(ci.TotalPhysicalMemory / (1024 * 1024));
        }

        /// <summary>
        /// Return the total paging RAM in MB.
        /// </summary>
        /// <returns>Total paging RAM in MB</returns>
        public static int GetTotalPagingRamInMegaBytes()
        {
            Log.Method();
            Log.Info((GetWin32TotalPagingRam() / (1024 * 1024)) + "MB");
            return (int)(GetWin32TotalPagingRam() / (1024 * 1024));
        }

        #region GetWin32AvailablePagingRam, GetWin32TotalPagingRam (Win32 Helpers)

        public static ulong GetWin32AvailablePagingRam()
        {
            Log.Method();
            var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
            foreach (var managementBaseObject in searcher.Get())
                return UInt64.Parse(managementBaseObject.Properties["FreeSpaceInPagingFiles"].Value.ToString()) * 1024;
            return 0;
        }

        public static ulong GetWin32TotalPagingRam()
        {
            Log.Method();
            var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
            foreach (var managementBaseObject in searcher.Get())
                return (UInt64.Parse(managementBaseObject.Properties["SizeStoredInPagingFiles"].Value.ToString()) * 1024) + GetWin32AvailablePagingRam();
            return 0;
        }

        #endregion

        #endregion
    }
}
