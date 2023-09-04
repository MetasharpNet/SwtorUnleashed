using System;
using System.IO;

namespace SwtorUnleashed.Model
{
    public class RamDrive
    {
        #region consts

        private const string DriveName = "SWTOR_RAM";

        #endregion

        #region properties

        public static string DirSwtor                   { get { return Setup.Settings.RamDriveLetter + @":\SWTOR";                     } }
        public static string DirSwtorPublictest         { get { return Setup.Settings.RamDriveLetter + @":\SWTOR\publictest";          } }
        public static string DirSwtorPublictestSettings { get { return Setup.Settings.RamDriveLetter + @":\SWTOR\publictest\settings"; } }
        public static string DirSwtorSwtor              { get { return Setup.Settings.RamDriveLetter + @":\SWTOR\swtor";               } }
        public static string DirSwtorSwtorSettings      { get { return Setup.Settings.RamDriveLetter + @":\SWTOR\swtor\settings";      } }
        public static string FileDiskCacheArena         { get { return Setup.Settings.RamDriveLetter + @":\DiskCacheArena";            } }

        #endregion

        #region Create

        /// <summary>
        /// create a ram drive of the specified size
        /// </summary>
        /// <returns>true if success, false if failure</returns>
        public static bool Create()
        {
            Log.Method();
            if (Exists())
                return false;
            try
            {
                uint deviceNumber = 0;
                ImDiskWrapper.CreateDevice(Setup.Settings.RamDriveLetter, Setup.Settings.RamDriveSize, ref deviceNumber, true);
                Setup.Settings.RamDriveDevice = deviceNumber;
                Setup.Save();
                return DriveManager.FormatDrive(Setup.Settings.RamDriveLetter, DriveName);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                Tools.ShowError(ex.Message);
            }
            return false;
        }

        #endregion

        #region Exists

        /// <summary>
        /// Tells if the ram drive exists
        /// </summary>
        /// <returns>true if it does, false if it doesn't</returns>
        public static bool Exists()
        {
            Log.Method();
            return DriveManager.Exists(Setup.Settings.RamDriveLetter);
        }

        #endregion
       
        #region GetAssetFilePath

        public static string GetAssetFilePath(string assetName)
        {
            Log.Method(assetName);
            return Setup.Settings.RamDriveLetter + @":\" + assetName + ".tor";
        }

        #endregion

        #region Remove

        /// <summary>
        /// remove a ram drive
        /// </summary>
        /// <returns>true if success, false if failure</returns>
        public static bool Remove()
        {
            Log.Method();
            if (!Exists())
                return true;
            try
            {
                if (new DriveInfo(Setup.Settings.RamDriveLetter + ":").VolumeLabel != DriveName) // dot not touch non ram drives!
                    return false;
                ImDiskWrapper.RemoveMountPoint(Setup.Settings.RamDriveLetter);
                ImDiskWrapper.RemoveDevice(Setup.Settings.RamDriveLetter, Setup.Settings.RamDriveDevice);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                Tools.ShowError(ex.Message);
            }
            return false;
        }

        #endregion
    }
}
