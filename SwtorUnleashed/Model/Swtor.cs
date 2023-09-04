using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Win32;

namespace SwtorUnleashed.Model
{
    public class Swtor
    {
        #region consts

        private const string ProcessNameGame     = "swtor";
        private const string ProcessNameLauncher = "launcher";
        private const string ProcessLauncherDesc = "SWTOR Launcher";

        public  const string AssetExtension   = ".tor";
        public  const string AssetFilter      = "*.tor";
        private const string AssetPtsTag      = "test";
        public  const string LauncherFileName = "launcher.exe";
        public  const string OriginalTag      = "_ORIG";
        private const string SwtorRegKey      = @"HKEY_LOCAL_MACHINE\software\wow6432node\bioware\star wars-the old republic";
        private const string SwtorRegEntry    = "Install Dir";

        public  const string AssetFx          = "swtor_main_art_fx_1";
        public  const string AssetDyn1        = "swtor_main_art_dynamic_cape_1";
        public  const string AssetDyn2        = "swtor_main_art_dynamic_chest_1";
        public  const string AssetDyn3        = "swtor_main_art_dynamic_chest_tight_1";
        public  const string AssetDyn4        = "swtor_main_art_dynamic_hand_1";
        public  const string AssetDyn5        = "swtor_main_art_dynamic_head_1";
        public  const string AssetDyn6        = "swtor_main_art_dynamic_lower_1";
        public  const string AssetDyn7        = "swtor_main_art_dynamic_mags_1";

        public const int DiskCacheArenaSize = 1100;
        public const int LocalAppSize       = 266;
        public const int RamNeededByGame    = 2048 + 512; // 2 process

        #endregion

        #region properties

        public static string FileGameSwtorDiskCacheArena           { get { return Setup.Settings.GameDirectory     + @"\swtor\DiskCacheArena";               } }
        public static string FileGameSwtorDiskCacheArenaOriginal   { get { return Setup.Settings.GameDirectory     + @"\swtor\DiskCacheArena-orig";          } }
        public static string DirGameAssets                         { get { return Setup.Settings.GameDirectory     + @"\Assets";                             } }
        public static string DirLocalAppSwtor                      { get { return Setup.Settings.LocalAppDirectory + @"\SWTOR";                              } }
        public static string DirLocalAppSwtorSwtor                 { get { return Setup.Settings.LocalAppDirectory + @"\SWTOR\swtor";                        } }
        public static string DirLocalAppSwtorSwtorSettings         { get { return Setup.Settings.LocalAppDirectory + @"\SWTOR\swtor\settings";               } }
        public static string DirLocalAppSwtorPublictest            { get { return Setup.Settings.LocalAppDirectory + @"\SWTOR\publictest";                   } }
        public static string DirLocalAppSwtorPublictestSettings    { get { return Setup.Settings.LocalAppDirectory + @"\SWTOR\publictest\settings";          } }
        public static string DirLocalAppOriginal                   { get { return Setup.Settings.LocalAppDirectory + @"\SWTOR_ORIGINAL";                     } }
        public static string DirLocalAppOriginalSwtor              { get { return Setup.Settings.LocalAppDirectory + @"\SWTOR_ORIGINAL\swtor";               } }
        public static string DirLocalAppOriginalSwtorSettings      { get { return Setup.Settings.LocalAppDirectory + @"\SWTOR_ORIGINAL\swtor\settings";      } }
        public static string DirLocalAppOriginalPublictest         { get { return Setup.Settings.LocalAppDirectory + @"\SWTOR_ORIGINAL\publictest";          } }
        public static string DirLocalAppOriginalPublictestSettings { get { return Setup.Settings.LocalAppDirectory + @"\SWTOR_ORIGINAL\publictest\settings"; } }

        #endregion

        #region FindGameDirectory

        /// <summary>
        /// looks up for the game's directory
        /// </summary>
        /// <returns>game's directory path, null otherwise</returns>
        public static string FindGameDirectory()
        {
            Log.Method();
            try
            {
                string path = GetGameDirectoryFromRegistry();
                if (path != null && IsValidGameDirectory(path))
                    return path;
            }
            catch (Exception ex) { Log.Error(ex); }
            return null;
        }

        #endregion

        #region GetAssetFilePath

        public static string GetAssetFilePath(string assetName, bool tagAsOriginal = false)
        {
            Log.Method(assetName, tagAsOriginal);
            string tag = "";
            if (tagAsOriginal)
                tag = OriginalTag;
            return DirGameAssets + @"\" + assetName + tag + AssetExtension;
        }

        #endregion

        #region GetAvailableAssets

        public static List<string> GetAvailableAssets(bool includePtsAssets = false)
        {
            Log.Method(includePtsAssets);
            var assets = new List<string>();
            try
            {
                var assetsDir = new DirectoryInfo(DirGameAssets);
                if (!assetsDir.Exists)
                    return assets;
                foreach (FileInfo assetFile in assetsDir.GetFiles(AssetFilter))
                    if (!assetFile.Name.EndsWith(OriginalTag + AssetExtension))
                    {
                        if (!includePtsAssets && assetFile.Name.Contains(AssetPtsTag))
                            continue;
                        assets.Add(assetFile.Name.Replace(AssetExtension, ""));
                    }
            }
            catch (Exception ex) { Log.Error(ex); }
            return assets;
        }

        #endregion

        #region IsValidGameDirectory

        /// <summary>
        /// tests if the settings directory is a valid SWTOR game directory
        /// </summary>
        /// <returns>true if valid, false if invalid</returns>
        public static bool IsValidGameDirectory()
        {
            Log.Method();
            return IsValidGameDirectory(Setup.Settings.GameDirectory);
        }

        /// <summary>
        /// tests if the provided directory is a valid SWTOR game directory
        /// </summary>
        /// <param name="directory">game's directory to test</param>
        /// <returns>true if valid, false if invalid</returns>
        public static bool IsValidGameDirectory(string directory)
        {
            Log.Method(directory);
            if (String.IsNullOrEmpty(directory))
                return false;
            
            try
            {
                return File.Exists(Path.Combine(directory, LauncherFileName));
            }
            catch (Exception ex) { Log.Error(ex); }

            return false;
        }

        #endregion

        #region GetGameDirectoryFromRegistry

        public static string GetGameDirectoryFromRegistry()
        {
            Log.Method();
            var gameDirectory = Registry.GetValue(SwtorRegKey, SwtorRegEntry, null);
            return (gameDirectory != null) ? gameDirectory.ToString() : "";
        }

        #endregion

        #region SetGameDirectoryToRegistry

        public static void SetGameDirectorytoRegistry(string gameDirectory)
        {
            Log.Method(gameDirectory);
            try
            {
                if (IsValidGameDirectory(gameDirectory))
                    Registry.SetValue(SwtorRegKey, SwtorRegEntry, gameDirectory);
            }
            catch (Exception ex) { Log.Error(ex); }
        }

        #endregion

        #region IsRunning

        /// <summary>
        /// Checks if the game is still running by checking it with the current processes.
        /// </summary>
        /// <returns>True if the game is still active, false otherwise.</returns>
        public static bool IsRunningGame()
        {
            Log.Method();
            try
            {
                return (Process.GetProcessesByName(ProcessNameGame).Any());
            }
            catch (Exception ex) { Log.Error(ex); }
            return false;
        }

        public static bool IsRunningLauncher()
        {
            Log.Method();
            
            Process[] processList = Process.GetProcessesByName(ProcessNameLauncher);

            if (!processList.Any())
                return false;

            foreach (var process in processList)
            {
                try // try grab mainmodule
                {
                    if (process.MainModule.FileVersionInfo.FileDescription == ProcessLauncherDesc)
                        return true;
                }
                catch (Exception ex) // catch exception of a 32bit (this app) accessing a 64bit process
                {
                    Log.Error(ex);
                }
            }
            return false;
        }

        #endregion

        #region Enable/Disable Launcher

        //tbd

        #endregion
    }
}
