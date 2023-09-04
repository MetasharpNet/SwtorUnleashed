using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;

namespace SwtorUnleashed.Model
{
    public sealed class DriveManager
    {
        #region Exists

        /// <summary>
        /// Tells if the specified drive exists
        /// </summary>
        /// <param name="driveLetter">drive letter. Example : 'A', 'B', 'C', 'D', ..., 'Z'.</param>
        /// <returns>true if it exists, false if it doesn't</returns>
        public static bool Exists(char driveLetter)
        {
            #region args check

            if (!Char.IsLetter(driveLetter))
                return false;

            #endregion
            Log.Method(driveLetter);
            try
            {
                return (DriveInfo.GetDrives().Any(d => d.Name[0] == driveLetter));
            }
            catch (Exception ex) { Log.Error(ex); }
            return false;
        }


        #endregion

        #region FormatDrive

        /// <summary>
        /// Format a drive using the best available method
        /// </summary>
        /// <param name="driveLetter">drive letter. Example : 'A', 'B', 'C', 'D', ..., 'Z'.</param>
        /// <param name="label">label for the drive</param>
        /// <param name="fileSystem">file system. Possible values : "FAT", "FAT32", "EXFAT", "NTFS", "UDF".</param>
        /// <param name="quickFormat">quick formatting?</param>
        /// <param name="enableCompression">enable drive compression?</param>
        /// <param name="clusterSize">cluster size (default=null for auto). Possible value depends on the file system : 512, 1024, 2048, 4096, 8192, 16384, 32768, 65536, ...</param>
        /// <returns>true if success, false if failure</returns>
        public static bool FormatDrive(char driveLetter, string label = "", string fileSystem = "NTFS", bool quickFormat = true, bool enableCompression = false, int? clusterSize = null)
        {
            Log.Method(driveLetter, label, fileSystem, quickFormat, enableCompression, clusterSize);
            return FormatDrive_CommandLine(driveLetter, label, fileSystem, quickFormat, enableCompression, clusterSize);
        }

        #endregion

        #region FormatDrive_CommandLine

        /// <summary>
        /// Format a drive using Format.com windows file
        /// </summary>
        /// <param name="driveLetter">drive letter. Example : 'A', 'B', 'C', 'D', ..., 'Z'.</param>
        /// <param name="label">label for the drive</param>
        /// <param name="fileSystem">file system. Possible values : "FAT", "FAT32", "EXFAT", "NTFS", "UDF".</param>
        /// <param name="quickFormat">quick formatting?</param>
        /// <param name="enableCompression">enable drive compression?</param>
        /// <param name="clusterSize">cluster size (default=null for auto). Possible value depends on the file system : 512, 1024, 2048, 4096, 8192, 16384, 32768, 65536, ...</param>
        /// <returns>true if success, false if failure</returns>
        public static bool FormatDrive_CommandLine(char driveLetter, string label = "", string fileSystem = "NTFS", bool quickFormat = true, bool enableCompression = false, int? clusterSize = null)
        {
            #region args check

            if (!Char.IsLetter(driveLetter) ||
                !IsFileSystemValid(fileSystem))
                return false;

            #endregion
            Log.Method(driveLetter, label, fileSystem, quickFormat, enableCompression, clusterSize);
            bool success = false;
            try
            {
                var di = GetDrive(driveLetter);
                if (di == null)
                    return false;
                var psi = new ProcessStartInfo
                    {
                        FileName               = "format.com",
                        WorkingDirectory       = Environment.SystemDirectory,
                        Arguments              = "/FS:" + fileSystem +
                                                 " /Y" +
                                                 " /V:" + label +
                                                 (quickFormat ? " /Q" : "") +
                                                 ((fileSystem == "NTFS" && enableCompression) ? " /C" : "") +
                                                 (clusterSize.HasValue ? " /A:" + clusterSize.Value : "") +
                                                 " " + driveLetter + ":",
                        UseShellExecute        = false,
                        CreateNoWindow         = true,
                        RedirectStandardOutput = true,
                        RedirectStandardInput  = true
                    };
                var formatProcess = Process.Start(psi);
                if (formatProcess != null)
                {
                    var swStandardInput = formatProcess.StandardInput;
                    swStandardInput.WriteLine();
                    formatProcess.WaitForExit();
                    success = true;
                }
            }
            catch (Exception ex) { Log.Error(ex); }
            return success;
        }

        #endregion

        #region FormatDrive_Shell32

        #region interop

        // http://msdn.microsoft.com/en-us/library/windows/desktop/bb762169(v=vs.85).aspx
        [DllImport("shell32.dll")]
        private static extern uint SHFormatDrive(IntPtr hwnd, uint drive, ShFormatFlags fmtId, ShFormatOptions options);

        private enum ShFormatFlags : uint
        {
            /// <summary>
            /// SHFMT_ID_DEFAULT
            /// </summary>
            ShfmtIdDefault = 0xFFFF,
            /// <summary>
            /// SHFMT_ERROR : A general error occured while formatting. This is not an indication that the drive cannot be formatted though.
            /// </summary>
            ShfmtError     = 0xFFFFFFFF,
            /// <summary>
            /// SHFMT_CANCEL : The drive format was cancelled by user/OS.
            /// </summary>
            ShfmtCancel    = 0xFFFFFFFE,
            /// <summary>
            /// SHFMT_NOFORMAT : A serious error occured while formatting. The drive is unable to be formatted by the OS.
            /// </summary>
            ShfmtNoformat  = 0xFFFFFFD
        }

        [Flags]
        private enum ShFormatOptions : uint
        {
            /// <summary>
            /// SHFMT_OPT_COMPLETE : Full formatting
            /// </summary>
            ShfmtOptComplete = 0x0,
            /// <summary>
            /// SHFMT_OPT_FULL : Quick Format
            /// </summary>
            ShfmtOptFull     = 0x1,
            /// <summary>
            /// SHFMT_OPT_SYSONLY : MS-DOS System Boot Disk
            /// </summary>
            ShfmtOptSysonly  = 0x2
        }

        #endregion

        /// <summary>
        /// Format a drive using Shell32.dll
        /// </summary>
        /// <param name="driveLetter">drive letter. Example : 'A', 'B', 'C', 'D', ..., 'Z'.</param>
        /// <param name="label">label for the drive</param>
        /// <param name="quickFormat">quick formatting?</param>
        /// <returns>true if success, false if failure</returns>
        [Obsolete("Unsupported by Microsoft nowadays. Prefer the FormatDrive() or FormatDrive_CommandLine() methods")]
        public static bool FormatDrive_Shell32(char driveLetter, string label = "", bool quickFormat = true)
        {
            #region args check

            if (!Char.IsLetter(driveLetter))
                return false;

            #endregion
            Log.Method(driveLetter, label, quickFormat);
            bool success = false;
            try
            {
                var di = GetDrive(driveLetter);
                if (di == null)
                    return false;
                var bytes        = Encoding.ASCII.GetBytes(di.Name.ToCharArray());
                uint driveNumber = Convert.ToUInt32(bytes[0] - Encoding.ASCII.GetBytes(new[] { 'A' })[0]);
                var options      = ShFormatOptions.ShfmtOptComplete;
                if (quickFormat)
                    options = ShFormatOptions.ShfmtOptFull;

                uint returnCode = SHFormatDrive(IntPtr.Zero, driveNumber, ShFormatFlags.ShfmtIdDefault, options);
                if (returnCode == (uint)ShFormatFlags.ShfmtError)
                    throw new Exception("An error occurred during the format. This does not indicate that the drive is unformattable.");
                if (returnCode == (uint)ShFormatFlags.ShfmtCancel)
                    throw new OperationCanceledException("The format was canceled.");
                if (returnCode == (uint)ShFormatFlags.ShfmtNoformat)
                    throw new IOException("The drive cannot be formatted.");

                SetLabel(driveLetter, label);
                success = true;
            }
            catch (Exception ex) { Log.Error(ex); }
            return success;
        }

        #endregion

        #region FormatDrive_Win32Api

        // http://msdn.microsoft.com/en-us/library/aa394515(VS.85).aspx
        
        /// <summary>
        /// Format a drive using Win32 API
        /// </summary>
        /// <param name="driveLetter">drive letter. Example : 'A', 'B', 'C', 'D', ..., 'Z'.</param>
        /// <param name="label">label for the drive</param>
        /// <param name="fileSystem">file system. Possible values : "FAT", "FAT32", "EXFAT", "NTFS", "UDF".</param>
        /// <param name="quickFormat">quick formatting?</param>
        /// <param name="enableCompression">enable drive compression?</param>
        /// <param name="clusterSize">cluster size. Possible value depends on the file system : 512, 1024, 2048, 4096, 8192, 16384, 32768, 65536, ...</param>
        /// <returns>true if success, false if failure</returns>
        [Obsolete("Might have troubles formatting ram drives. Prefer the FormatDrive() or FormatDrive_CommandLine() methods")]
        public static bool FormatDrive_Win32Api(char driveLetter, string label = "", string fileSystem = "NTFS", bool quickFormat = true, bool enableCompression = false, int clusterSize = 8192)
        {
            #region args check

            if (!Char.IsLetter(driveLetter) ||
                !IsFileSystemValid(fileSystem))
                return false;

            #endregion
            Log.Method(driveLetter, label, fileSystem, quickFormat, enableCompression, clusterSize);
            bool success = false;
            try
            {
                var moSearcher = new ManagementObjectSearcher(@"SELECT * FROM Win32_Volume WHERE DriveLetter='" + driveLetter + ":'");
                foreach (var managementObject in moSearcher.Get().OfType<ManagementObject>())
                {
                    managementObject.InvokeMethod("Format", new object[] { fileSystem, quickFormat, clusterSize, label, enableCompression });
                    success = true;
                }
            }
            catch (Exception ex)
            {
                success = false;
                Log.Error(ex);
            }
            return success;
        }

        #endregion

        #region GetDrive

        /// <summary>
        /// Get a DriveInfo object for the required drive letter or null if non available
        /// </summary>
        /// <param name="driveLetter">drive letter. Example : 'A', 'B', 'C', 'D', ..., 'Z'.</param>
        /// <returns>DriveInfo object if the drive exists and is available, null otherwise</returns>
        public static DriveInfo GetDrive(char driveLetter)
        {
            #region args check

            if (!Char.IsLetter(driveLetter))
                return null;

            #endregion
            Log.Method(driveLetter);
            try
            {
                if (Exists(driveLetter))
                    return new DriveInfo(driveLetter.ToString(CultureInfo.InvariantCulture));
            }
            catch (Exception ex) { Log.Error(ex); }
            return null;
        }

        #endregion

        #region IsFileSystemValid

        /// <summary>
        /// test if the provided filesystem value is valid
        /// </summary>
        /// <param name="fileSystem">file system. Possible values : "FAT", "FAT32", "EXFAT", "NTFS", "UDF".</param>
        /// <returns>true if valid, false if invalid</returns>
        public static bool IsFileSystemValid(string fileSystem)
        {
            #region args check

            if (fileSystem == null)
                return false;

            #endregion
            Log.Method(fileSystem);
            switch (fileSystem)
            {
                case "FAT":
                case "FAT32":
                case "EXFAT":
                case "NTFS":
                case "UDF":
                    return true;
                default:
                    return false;
            }
        }

        #endregion

        #region SetLabel

        /// <summary>
        /// set a drive label to the desired value
        /// </summary>
        /// <param name="driveLetter">drive letter. Example : 'A', 'B', 'C', 'D', ..., 'Z'.</param>
        /// <param name="label">label for the drive</param>
        /// <returns>true if success, false if failure</returns>
        public static bool SetLabel(char driveLetter, string label = "")
        {
            #region args check

            if (!Char.IsLetter(driveLetter))
                return false;
            if (label == null)
                label = "";

            #endregion
            Log.Method(driveLetter, label);
            try
            {
                var driveInfo = GetDrive(driveLetter);
                if (driveInfo == null)
                    return false;
                driveInfo.VolumeLabel = label;
                return true;
            }
            catch (Exception ex) { Log.Error(ex); }
            return false;
        }

        #endregion
    }
}
