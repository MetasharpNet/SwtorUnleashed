using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using SwtorUnleashed.Model;
using SwtorUnleashed.ViewModel.Base;
using WF = System.Windows.Forms;

namespace SwtorUnleashed.ViewModel
{
    public class SetupViewModel : Base.ViewModel
    {
        #region window

        private Window _window;

        #endregion

        #region .ctor

        public SetupViewModel()
        {
            _assets = new ObservableCollection<Asset>();
            #region Design Mode Sample Values
            
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                _assets.Add(new Asset { Name = "test_file_checked",   IsChecked = true  });
                _assets.Add(new Asset { Name = "test_file_unchecked", IsChecked = false });
                return;
            }

            #endregion
            foreach (string asset in Setup.Settings.AssetFileNames)
                _assets.Add(new Asset { Name = asset, IsChecked = true});
        }

        #endregion

        #region properties

        #region underlying members

        private ObservableCollection<Asset>  _assets;
        private bool                         _diskCacheArena;
        private bool                         _extraSpace;
        private string                       _gameDirectory;
        private bool                         _localApp;
        private bool                         _logFile;
        private string                       _ramDriveLetter;
        private string                       _ramDriveSize;
        private bool                         _showPts;
        private string                       _windowTitle;

        #endregion

        public ObservableCollection<Asset>  Assets         { get { return _assets;         } set { if (value != _assets        ) { _assets         = value;                            TriggerPropertyChanged("Assets");         } } }
        public bool                         DiskCacheArena { get { return _diskCacheArena; } set { if (value != _diskCacheArena) { _diskCacheArena = value; OnDiskCacheArenaToggled(); TriggerPropertyChanged("DiskCacheArena"); } } }
        public bool                         ExtraSpace     { get { return _extraSpace;     } set { if (value != _extraSpace    ) { _extraSpace     = value; OnExtraSpaceToggled();     TriggerPropertyChanged("ExtraSpace");     } } }
        public string                       GameDirectory  { get { return _gameDirectory;  } set { if (value != _gameDirectory ) { _gameDirectory  = value;                            TriggerPropertyChanged("GameDirectory");  } } }
        public bool                         LocalApp       { get { return _localApp;       } set { if (value != _localApp      ) { _localApp       = value; OnLocalAppToggled();       TriggerPropertyChanged("LocalApp");       } } }
        public bool                         LogFile        { get { return _logFile;        } set { if (value != _logFile       ) { _logFile        = value; OnLogFileToggled();        TriggerPropertyChanged("LogFile");            } } }
        public string                       RamDriveLetter { get { return _ramDriveLetter; } set { if (value != _ramDriveLetter) { _ramDriveLetter = value;                            TriggerPropertyChanged("RamDriveLetter"); } } }
        public string                       RamDriveSize   { get { return _ramDriveSize;   } set { if (value != _ramDriveSize  ) { _ramDriveSize   = value;                            TriggerPropertyChanged("RamDriveSize");   } } }
        public bool                         ShowPts        { get { return _showPts;        } set { if (value != _showPts       ) { _showPts        = value; OnShowPtsToggled();        TriggerPropertyChanged("ShowPTS");        } } }
        public string                       WindowTitle    { get { return _windowTitle;    } set { if (value != _windowTitle   ) { _windowTitle    = value;                            TriggerPropertyChanged("WindowTitle");    } } }

        #endregion

        #region commands

        #region buttons

        [UiCommand]
        public void OnBrowseGameDirectoryClick()
        {
            Log.User();
            using (var fbd = new WF.FolderBrowserDialog())
            {
                fbd.ShowNewFolderButton = false;
                fbd.RootFolder = Environment.SpecialFolder.MyComputer;
                if (fbd.ShowDialog() == WF.DialogResult.OK)
                {
                    if (Swtor.IsValidGameDirectory(fbd.SelectedPath))
                    {
                        GameDirectory                = fbd.SelectedPath;
                        Setup.Settings.GameDirectory = fbd.SelectedPath;
                        LoadAssets();
                    }
                    else
                        Tools.ShowError("This folder is not SWTOR one, it doesn't contain launcher.exe", "Wrong folder");
                }
            }
        }

        [UiCommand]
        public void OnCancelClick()
        {
            Log.User();
            Setup.Load();
            _window.Close();
        }

        [UiCommand]
        public void OnDeselectAllClick()
        {
            Log.User();
            foreach (var a in Assets)
            {
                a.IsChecked = false;
                OnAssetToggled(a);
            }
        }

        [UiCommand]
        public void OnSaveClick()
        {
            Log.User();
            RamDrive.Remove();
            if (!Setup.IsValid())
            {
                Tools.ShowError("Please choose valid settings so they can be saved.", "Invalid settings");
                return;
            }          
            SaveSettings();
            _window.Close();
        }

        [UiCommand]
        public void OnSelectAllClick()
        {
            Log.User();
            foreach (var a in Assets)
            {
                a.IsChecked = true;
                OnAssetToggled(a);
            }
        }

        [UiCommand]
        public void OnSelectFxClick()
        {
            Log.User();
            foreach (var a in Assets)
                if (a.Name == Swtor.AssetFx)
                {
                    a.IsChecked = true;
                    OnAssetToggled(a);
                    return;
                }
        }

        [UiCommand]
        public void OnSelectDynamicClick()
        {
            Log.User();
            var dynamicAssets = new List<string> { Swtor.AssetDyn1,
                                                   Swtor.AssetDyn2,
                                                   Swtor.AssetDyn3,
                                                   Swtor.AssetDyn4,
                                                   Swtor.AssetDyn5,
                                                   Swtor.AssetDyn6,
                                                   Swtor.AssetDyn7 };
            foreach (var a in Assets)
                if (dynamicAssets.Contains(a.Name))
                {
                    a.IsChecked = true;
                    OnAssetToggled(a);
                }
        }

        #endregion

        #region check boxes

        #region UpdateRamDriveSizeOnCheckBoxToggle

        private void UpdateRamDriveSizeOnCheckBoxToggle(bool isChecked, int sizeInMegaBytes)
        {
            Log.Method();
            if (isChecked)
            {
                Setup.Settings.RamDriveSize += sizeInMegaBytes;
            }
            else
            {
                Setup.Settings.RamDriveSize -= sizeInMegaBytes;
            }
            RamDriveSize = Setup.Settings.RamDriveSize.ToString(CultureInfo.InvariantCulture);
        }

        #endregion

        [UiCommand]
        public void OnAssetToggled(Asset asset)
        {
            Log.User();
            var assetFile = new FileInfo(Swtor.GetAssetFilePath(asset.Name));
            if (File.Exists(Swtor.GetAssetFilePath(asset.Name, true))) // if a setup is already "on", examin ORIG asset instead of an empty link file
                assetFile = new FileInfo(Swtor.GetAssetFilePath(asset.Name, true));
            int assetSize = Convert.ToInt32(assetFile.Length / 1024 / 1024) + 1;
            if (asset.IsChecked)
            {
                if (Setup.Settings.AssetFileNames.Contains(asset.Name))
                    return;
                Setup.Settings.AssetFileNames.Add(asset.Name);
            }
            else
            {
                if (!Setup.Settings.AssetFileNames.Contains(asset.Name))
                    return;
                Setup.Settings.AssetFileNames.Remove(asset.Name);
            }
            UpdateRamDriveSizeOnCheckBoxToggle(asset.IsChecked, assetSize);
        }

        [UiCommand]
        public void OnDiskCacheArenaToggled()
        {
            Log.User();
            if (Setup.Settings.AddDiskCacheArena != DiskCacheArena)
            {
                Setup.Settings.AddDiskCacheArena = DiskCacheArena;
                UpdateRamDriveSizeOnCheckBoxToggle(DiskCacheArena, Swtor.DiskCacheArenaSize);
            }
        }

        [UiCommand]
        public void OnExtraSpaceToggled()
        {
            Log.User();
            if (Setup.Settings.AddExtraSpace != ExtraSpace)
            {
                Setup.Settings.AddExtraSpace = ExtraSpace;
                UpdateRamDriveSizeOnCheckBoxToggle(ExtraSpace, Settings.ExtraSize);
            }
        }

        [UiCommand]
        public void OnLocalAppToggled()
        {
            Log.User();
            if (Setup.Settings.AddLocalApp != LocalApp)
            {
                Setup.Settings.AddLocalApp = LocalApp;
                UpdateRamDriveSizeOnCheckBoxToggle(LocalApp, Swtor.LocalAppSize);
            }
        }

        [UiCommand]
        public void OnLogFileToggled()
        {
            Log.User();
            Setup.Settings.Log = LogFile;
        }

        [UiCommand]
        public void OnShowPtsToggled()
        {
            Log.User();
            if (Setup.Settings.AddPts != ShowPts)
            {
                Setup.Settings.AddPts = ShowPts;
                LoadAssets();
            }
        }

        #endregion

        #region text boxes

        [UiCommand]
        public void OnRamDriveLetterKeyUp()
        {
            Log.User();
            Setup.Settings.RamDriveLetter = Char.Parse(RamDriveLetter);
        }

        #endregion

        #region window

        [UiCommand]
        public void OnWindowLoaded(Window window)
        {
            Log.Method();
            _window     = window;
            WindowTitle = AssemblyInfo.Product + " Settings";
            LoadSettings();
            if (String.IsNullOrEmpty(Setup.Settings.GameDirectory))
                GameDirectory = "Please select the install location of SWTOR";
        }

        #endregion

        #endregion

        #region settings

        #region assets

        private void LoadAssets()
        {
            Log.Method();
            //Assets.Clear();
            var availableAssets = Swtor.GetAvailableAssets(Setup.Settings.AddPts);
            foreach (var availableAsset in availableAssets)
            {
                bool checkIt = Setup.Settings.AssetFileNames.Contains(availableAsset);
                bool found   = false;
                foreach (Asset a in Assets)
                    if (a.Name == availableAsset)
                    {
                        a.IsChecked = checkIt;
                        found= true;
                    }
                if (!found)
                    Assets.Add(new Asset { Name = availableAsset, IsChecked = false });
            }
            for (int i = Assets.Count - 1 ; i >= 0; --i)
            {
                if (availableAssets.All(a => Assets[i].Name != a))
                {
                    if (Assets[i].IsChecked)
                    {
                        Assets[i].IsChecked = false;
                        OnAssetToggled(Assets[i]);
                    }
                    Assets.RemoveAt(i);
                }
            }
        }

        private void SaveAssets()
        {
            Log.Method();
            Setup.Settings.AssetFileNames.Clear();
            foreach (var a in Assets)
                if (a.IsChecked)
                    Setup.Settings.AssetFileNames.Add(a.Name);
        }

        #endregion

        private void LoadSettings()
        {
            Log.Method();
            try
            {
                Setup.Load();
                DiskCacheArena = Setup.Settings.AddDiskCacheArena;
                ExtraSpace     = Setup.Settings.AddExtraSpace;
                LocalApp       = Setup.Settings.AddLocalApp;
                LogFile        = Setup.Settings.Log;
                ShowPts        = Setup.Settings.AddPts;
                GameDirectory  = Setup.Settings.GameDirectory;
                RamDriveSize   = Setup.Settings.RamDriveSize.ToString(CultureInfo.InvariantCulture);
                RamDriveLetter = Setup.Settings.RamDriveLetter.ToString(CultureInfo.InvariantCulture);
                LoadAssets();
            }
            catch (Exception ex) { Log.Error(ex); }
        }

        private void SaveSettings()
        {
            Log.Method();
            Setup.Settings.AddDiskCacheArena = DiskCacheArena;
            Setup.Settings.AddExtraSpace     = ExtraSpace;
            Setup.Settings.AddLocalApp       = LocalApp;
            Setup.Settings.AddPts            = ShowPts;
            Setup.Settings.GameDirectory     = GameDirectory;
            Setup.Settings.Log               = LogFile;
            Setup.Settings.RamDriveLetter    = RamDriveLetter[0];
            Setup.Settings.RamDriveSize      = Int32.Parse(RamDriveSize);
            SaveAssets();
            if (!Setup.IsValid())
            {
                Tools.ShowError("Please choose valid settings before saving them.", "Invalid settings");
                return;
            }
            Setup.Save();
        }

        #endregion
    }
}
