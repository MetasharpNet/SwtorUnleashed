using System.Diagnostics;
using System.IO;
using System.Linq;

namespace SwtorUnleashed.Model
{
    public class Launcher
    {
        #region .ctor

        static Launcher()
        {
            Log.Method();
            Setup.Load();
        }

        #endregion

        #region StartSetup, StartUnleashed, StartRetail

        public static void StartSetup()
        {
            Log.Method();

            // Check settings
            if (!Setup.IsValid(false))
                return;

            // Remove first to start fresh
            // Used for patch intercepting
            RemoveUnleashed();

            if (!HasEnoughRam())
                return;

            // Create Ramdrive if needed
            if (!RamDrive.Exists())
                if (!RamDrive.Create())
                    return;

            // Setup after patching done
            SetupUnleashed();
        }

        public static void StartUnleashed()
        {
            Log.Method();

            // Check settings
            if (!Setup.IsValid(false))
            {
                Program.StartGui();
                return;
            }

            // Remove first to start fresh
            // Used for patch intercepting
            RemoveUnleashed();

            if (!HasEnoughRam())
                return;

            // Create Ramdrive if needed
            if (!RamDrive.Exists())
                if (!RamDrive.Create())
                {
                    Program.StartGui();
                    Tools.ShowError("This program was unable create the ramdrive.\n\nTherefor returning to the program interface.", "Failed to create ramdrive");
                    return;
                }

            // Setup after patching done
            SetupUnleashed();

            // Start launcher, let user login and patch
            Launch();
        }

        public static void StartRetail()
        {
            Log.Method();
            RemoveUnleashed(true);
            Launch();
        }

        private static void Launch()
        {
            Log.Method();
            if (!Swtor.IsValidGameDirectory())
            {
                Program.StartGui();
                Tools.ShowError("The given installation path in the settings is incorrect.\nPlease run setup again.", "Invalid install path");
                return;
            }
            var process = new Process
                {
                    StartInfo =
                        {
                            WorkingDirectory = Setup.Settings.GameDirectory,
                            FileName         = Path.Combine(Setup.Settings.GameDirectory, Swtor.LauncherFileName),
                            Arguments        = "",
                            UseShellExecute  = false
                        }
                };
            process.Start();
            Log.Info("Started [" + process.StartInfo.FileName + "] with the working directory [" + process.StartInfo.WorkingDirectory + "]");
        }

        #endregion

        #region SetupUnleashed

        private static void SetupUnleashed()
        {
            Log.Method();

            /* 
             * This part should be exectuted after removing the setup first
             * Removal should revert/fix everything back to retail setup
             * Therefore this part does not include extra checking
             */

            #region Ramdrive structure

            if (Tools.DirectoryExists(RamDrive.DirSwtor))
                Tools.TryDeleteDirectory(RamDrive.DirSwtor);

            Tools.TryCreateDirectory(RamDrive.DirSwtor);
            Tools.TryCreateDirectory(RamDrive.DirSwtorSwtor);
            Tools.TryCreateDirectory(RamDrive.DirSwtorPublictest);

            #endregion

            #region Local

            if (Setup.Settings.AddLocalApp)
            {
                Tools.TryMoveDirectory(Swtor.DirLocalAppSwtor, Swtor.DirLocalAppOriginal);
                Tools.TryCreateDirectoryLink(Swtor.DirLocalAppSwtor, RamDrive.DirSwtor);
                Tools.TryCreateDirectoryLink(RamDrive.DirSwtorSwtorSettings, Swtor.DirLocalAppOriginalSwtorSettings);
                Tools.TryCreateDirectoryLink(RamDrive.DirSwtorPublictestSettings, Swtor.DirLocalAppOriginalPublictestSettings);
            }

            #endregion

            #region DiskCacheArena

            if (Setup.Settings.AddDiskCacheArena)
            {
                Tools.TryMoveFile(Swtor.FileGameSwtorDiskCacheArena, Swtor.FileGameSwtorDiskCacheArenaOriginal);
                Tools.TryCreateFileLink(Swtor.FileGameSwtorDiskCacheArena, RamDrive.FileDiskCacheArena);
            }

            #endregion

            #region Assets

            var ramDriveAssets = Directory.GetFiles(Setup.Settings.RamDriveLetter + @":\", Swtor.AssetFilter, SearchOption.TopDirectoryOnly);
            foreach (var ramDriveAsset in ramDriveAssets)
                Tools.TryDeleteFile(ramDriveAsset);

            foreach (string assetFileName in Setup.Settings.AssetFileNames)
            {
                Tools.TryCopyFile(Swtor.GetAssetFilePath(assetFileName), RamDrive.GetAssetFilePath(assetFileName));
                Tools.TryMoveFile(Swtor.GetAssetFilePath(assetFileName), Swtor.GetAssetFilePath(assetFileName, true));
                Tools.TryCreateFileLink(Swtor.GetAssetFilePath(assetFileName), RamDrive.GetAssetFilePath(assetFileName));
            }

            #endregion
        }

        #endregion

        #region RemoveUnleashed

        public static void RemoveUnleashed(bool dismountRamdrive = false)
        {
            Log.Method();

            if (!Swtor.IsValidGameDirectory())
            {
                Tools.ShowError("Settings contain an invalid game directory,\nplease select a valid game directory in settings before continuing.", "Invalid game directory");
                return;
            }

            if (!Tools.DirectoryExists(Setup.Settings.LocalAppDirectory))
            {
                Tools.ShowError("Setting contain an invalid local directory,\nplease remove the file \'SwtorUnleashed.exe' and try again.", "Invalid local directory");
                return;
            }
                
            #region Restore Local

            if (!Tools.DirectoryExists(Swtor.DirLocalAppSwtor))
                Tools.TryCreateDirectory(Swtor.DirLocalAppSwtor);
            
            if (Tools.IsReparsePoint(Swtor.DirLocalAppSwtor))
            {
                Tools.TryDeleteDirectory(Swtor.DirLocalAppSwtor);

                if (Tools.DirectoryExists(Swtor.DirLocalAppOriginal))
                    Tools.TryMoveDirectory(Swtor.DirLocalAppOriginal, Swtor.DirLocalAppSwtor);
                else
                    Tools.TryCreateDirectory(Swtor.DirLocalAppSwtor);
            }
            else
            {
                if (Tools.DirectoryExists(Swtor.DirLocalAppOriginal))
                    Tools.TryDeleteDirectory(Swtor.DirLocalAppOriginal);
            }

            // Check underlying local swtor folders, in case of missing or not-yet-played installation
            if (!Tools.DirectoryExists(Swtor.DirLocalAppSwtorSwtor))
                Tools.TryCreateDirectory(Swtor.DirLocalAppSwtorSwtor);
            if (!Tools.DirectoryExists(Swtor.DirLocalAppSwtorPublictest))
                Tools.TryCreateDirectory(Swtor.DirLocalAppSwtorPublictest);
            if (!Tools.DirectoryExists(Swtor.DirLocalAppSwtorSwtorSettings))
                Tools.TryCreateDirectory(Swtor.DirLocalAppSwtorSwtorSettings);
            if (!Tools.DirectoryExists(Swtor.DirLocalAppSwtorPublictestSettings))
                Tools.TryCreateDirectory(Swtor.DirLocalAppSwtorPublictestSettings);

            #endregion

            #region Restore DiskCacheArena

            if (Tools.FileExists(Swtor.FileGameSwtorDiskCacheArena) && Tools.IsReparsePoint(Swtor.FileGameSwtorDiskCacheArena))
            {
                Tools.TryDeleteFile(Swtor.FileGameSwtorDiskCacheArena);
                Tools.TryMoveFile(Swtor.FileGameSwtorDiskCacheArenaOriginal, Swtor.FileGameSwtorDiskCacheArena);
            }

            #endregion

            #region Restore Assets

            var assetsOrig = Directory.GetFiles(Swtor.DirGameAssets, Swtor.AssetFilter, SearchOption.TopDirectoryOnly)
                                      .ToList()
                                      .Where(a => a.EndsWith(Swtor.OriginalTag + Swtor.AssetExtension));
            foreach (string assetOrig in assetsOrig)
            {
                string asset = assetOrig.Substring(0, assetOrig.Length - Swtor.OriginalTag.Length - Swtor.AssetExtension.Length) + Swtor.AssetExtension;
                Tools.TryDeleteFile(asset);
                Tools.TryMoveFile(assetOrig, asset);
            }

            #endregion

            #region Remove Ramdrive

            if (dismountRamdrive)
                RamDrive.Remove();

            #endregion
        }

        #endregion

        #region ShowBioWareAgreement

        public static void ShowBioWareAgreementUS()
        {
            Process.Start("IExplore.exe", "http://www.swtor.com/fr/community/showthread.php?p=5128820");
        }

        public static void ShowBioWareAgreementFR()
        {
            Process.Start("IExplore.exe", "http://www.swtor.com/community/showthread.php?t=441810");
        }

        #endregion

        #region HasEnoughRam

        public static bool HasEnoughRam()
        {
            Log.Method();

            int ramLeft = Tools.GetTotalRamInMegaBytes() - Swtor.RamNeededByGame;
            if (!RamDrive.Exists())
                ramLeft -= Setup.Settings.RamDriveSize;
            if (ramLeft <= 0)
            {
                Tools.ShowError("You lack " + (ramLeft * -1) + "MB of RAM free to put SWTOR (2.5GB) and Unleashed RamDrive (" + (Setup.Settings.RamDriveSize / 1024).ToString("0.0") + "GB) in RAM. That is without even taking Windows required RAM! Try to free some RAM or to put less things in the RAM drive (check your settings).");
                return false;
            }
            if (ramLeft < 512)
                Tools.ShowWarning("You will have only " + ramLeft + "MB of RAM free while playing the game for Windows and background applications. If you experience FPS (frames par second) drops while playing the game, try to free some RAM or to put less things in the RAM drive (check your settings)");

            var usedRam = Tools.GetOccupiedPagingRamInMegaBytes() + Tools.GetOccupiedRamInMegaBytes();
            ramLeft     += Tools.GetTotalPagingRamInMegaBytes() - usedRam;

            if (ramLeft <= 0)
                Tools.ShowError("You lack " + (ramLeft * -1) + "MB of free RAM to put SWTOR (2.5GB) and Unleashed RamDrive (" + (Setup.Settings.RamDriveSize / 1024).ToString("0.0") + "GB) in RAM. This is considering even your virtual ram (pagefile). Try to free some RAM or to put less things in the RAM drive (check your settings).");
            else if (ramLeft < 256)
                Tools.ShowWarning("You will have only " + ramLeft + "MB of free RAM while playing the game. This is considering even your virtual ram (pagefile). If you experience FPS (frames par second) drops while playing the game, try to free some RAM or to put less things in the RAM drive (check your settings)");
            return ramLeft > 0;
        }

        #endregion
    }
}
