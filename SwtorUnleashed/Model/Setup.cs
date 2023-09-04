using System.IO;
using System.Linq;

namespace SwtorUnleashed.Model
{
    public class Setup
    {
        #region properties

        /// <summary>
        /// globally accessible application settings
        /// </summary>
        public static Settings Settings { get; set; }

        #endregion

        #region HasValidAssets

        /// <summary>
        /// check if the assets are all valid (might not after a game upgrade)
        /// </summary>
        /// <returns>true if success, false if failure</returns>
        public static bool HasValidAssets(bool ramdiskCheck = true)
        {
            Log.Method();
            return Settings.AssetFileNames.All(asset => File.Exists(Swtor.GetAssetFilePath(asset)));
        }

        #endregion

        #region IsValid

        /// <summary>
        /// check if the settings are valid
        /// </summary>
        /// <returns>true if success, false if failure</returns>
        public static bool IsValid(bool ramdiskCheck = true)
        {
            Log.Method();
            if (Settings.IsAnotherComputerSettings())
            {
                Tools.ShowWarning("Setting ComputerName=[" + Settings.ComputerName + "] doesn't match your computer name.\nTry either to relaunch SWTOR Unleashed to autofix it, or delete your swtorunleashed.xml file and start again the tool.", "Invalid Setting");
                return false;
            }
            //if (Setup.Settings.RamDriveSize < 50)
            //{
            //    Tools.ShowWarning("Setting RamDriveSize=[" + Setup.Settings.RamDriveSize + "] is <= 50.\nYou have not selected any file to be placed on the ramdisk.", "Invalid Setting");
            //    return false;
            //}
            if (!Swtor.IsValidGameDirectory())
            {
                Tools.ShowWarning("Setting GameDirectory=[" + Settings.GameDirectory + "] doesn't contain launcher.exe.\nPlease select a correct SWTOR game folder.", "Invalid Setting");
                return false;
            }
            if (!Directory.Exists(Settings.LocalAppDirectory))
            {
                Tools.ShowWarning("Setting LocalAppDirectory=[" + Settings.LocalAppDirectory + "] doesn't exist.\nPlease delete your swtorunleashed.xml file and start again the tool.", "Invalid Setting");
                return false;
            }
            if (ramdiskCheck && DriveManager.Exists(Settings.RamDriveLetter))
            {
                Tools.ShowWarning("Setting RamDriveLetter=[" + Settings.RamDriveLetter + "] is taken already.\nPlease select another drive letter.", "Invalid Setting");
                return false;
            }
            foreach (var asset in Settings.AssetFileNames.Where(asset => !File.Exists(Swtor.GetAssetFilePath(asset))))
            {
                Tools.ShowWarning("Setting AssetFileNames=[" + asset + "] asset doesn't exist.\nPlease remove from the main menu to restore it then try to change again your settings.", "Invalid Setting");
                return false;
            }
            return true;
        }

        #endregion

        #region Load, Save

        public static void Load()
        {
            Log.Method();
            Settings = Settings.Deserialize();
            if (Settings.IsAnotherComputerSettings() || !HasValidAssets())
            {
                RamDrive.Remove();
                Settings = new Settings();
                Save();
            }
        }

        public static void Save()
        {
            Log.Method();
            Settings.Serialize(Settings);
        }

        #endregion
    }
}
