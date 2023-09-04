using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;
using SM = SwtorUnleashed.Model;

namespace SwtorUnleashed.Model
{
    public class Settings : INotifyPropertyChanged
    {
        #region consts

        private const string DefaultFileName       = "SwtorUnleashed.xml";
        private const char   DefaultRamDriveLetter = 'R';
        private const int    DefaultRamDriveSize   = 50; // 50MB start, for filesystem and disk vs file size differences
        public  const int    ExtraSize             = 100;
        private const string NotNullOrEmptyError   = "Must not be null or empty";

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// trigger PropertyChanged event providing the exact C# property name (to support two-way data binding)
        /// </summary>
        /// <param name="propertyName">exact C# property name</param>
        private void TriggerPropertyChanged(string propertyName)
        {
            #region args check

            if (String.IsNullOrEmpty(propertyName))
                throw new ArgumentException(NotNullOrEmptyError, "propertyName");

            #endregion
            SM.Log.Method(propertyName);
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region properties

        #region private property fields

        private bool         _addDiskCacheArena;
        private bool         _addExtraSpace;
        private bool         _addLocalApp;
        private bool         _addPts;
        private string       _computerName;
        private string       _configVersion;
        private string       _gameDirectory;
        private bool         _isSetup;
        private string       _localAppDirectory;
        private bool         _log;
        private uint         _ramdriveDevice;
        private char         _ramdriveLetter;
        private int          _ramDriveSize;
        private List<string> _assetFileNames;

        #endregion

        public bool          AddDiskCacheArena { get { return _addDiskCacheArena; } set { if (_addDiskCacheArena != value) { _addDiskCacheArena = value; TriggerPropertyChanged("AddDiskCacheArena"); } } }
        public bool          AddExtraSpace     { get { return _addExtraSpace;     } set { if (_addExtraSpace     != value) { _addExtraSpace     = value; TriggerPropertyChanged("AddExtraSpace"    ); } } }
        public bool          AddLocalApp       { get { return _addLocalApp;       } set { if (_addLocalApp       != value) { _addLocalApp       = value; TriggerPropertyChanged("AddLocalApp"      ); } } }
        public bool          AddPts            { get { return _addPts;            } set { if (_addPts            != value) { _addPts            = value; TriggerPropertyChanged("AddPTS"           ); } } }
        public string        ComputerName      { get { return _computerName;      } set { if (_computerName      != value) { _computerName      = value; TriggerPropertyChanged("ComputerName"     ); } } }
        public string        ConfigVersion     { get { return _configVersion;     } set { if (_configVersion     != value) { _configVersion     = value; TriggerPropertyChanged("ConfigVersion"    ); } } }
        public string        GameDirectory     { get { return _gameDirectory;     } set { if (_gameDirectory     != value) { _gameDirectory     = value; TriggerPropertyChanged("GameDirectory"    ); } } }
        public bool          IsSetup           { get { return _isSetup;           } set { if (_isSetup           != value) { _isSetup           = value; TriggerPropertyChanged("IsSetup"          ); } } }
        public string        LocalAppDirectory { get { return _localAppDirectory; } set { if (_localAppDirectory != value) { _localAppDirectory = value; TriggerPropertyChanged("LocalAppDirectory"); } } }
        public bool          Log               { get { return _log;               } set { if (_log               != value) { _log               = value; TriggerPropertyChanged("Log"              ); } } }
        public uint          RamDriveDevice    { get { return _ramdriveDevice;    } set { if (_ramdriveDevice    != value) { _ramdriveDevice    = value; TriggerPropertyChanged("RamDriveDevice"   ); } } }
        public char          RamDriveLetter    { get { return _ramdriveLetter;    } set { if (_ramdriveLetter    != value) { _ramdriveLetter    = value; TriggerPropertyChanged("RamDriveLetter"   ); } } }
        public int           RamDriveSize      { get { return _ramDriveSize;      } set { if (_ramDriveSize      != value) { _ramDriveSize      = value; TriggerPropertyChanged("RamDriveSize"     ); } } }
        public List<string>  AssetFileNames    { get { return _assetFileNames;    }
                                                 set
                                                 {
                                                     if (_assetFileNames != value && (_assetFileNames       == null        ||
                                                                                      value                 == null        ||
                                                                                      _assetFileNames.Count != value.Count ||
                                                                                      !_assetFileNames.TrueForAll(value.Contains)))
                                                     {
                                                         _assetFileNames = value;
                                                         TriggerPropertyChanged("AssetFileNames");
                                                     }
                                                 } }

        #endregion

        #region .ctor

        public Settings()
        {
            AddDiskCacheArena = false;
            AddExtraSpace     = false;
            AddLocalApp       = false;
            AddPts            = false;
            AssetFileNames    = new List<string>();
            ComputerName      = Environment.MachineName;
            ConfigVersion     = AssemblyInfo.Version;
            GameDirectory     = Swtor.FindGameDirectory();
            IsSetup           = false;
            LocalAppDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            Log               = false;
            RamDriveLetter    = DefaultRamDriveLetter;
            RamDriveSize      = DefaultRamDriveSize;
        }

        #endregion

        #region CreateDefaultIfMissing, Deserialize, Serialize

        private static void CreateDefaultIfMissing(string filePath)
        {
            SM.Log.Method(filePath);
            if (File.Exists(filePath) == false)
            {
                var s = new Settings();
                Serialize(s, filePath);
            }
        }

        public static Settings Deserialize()
        {
            SM.Log.Method();
            return Deserialize(DefaultFileName);
        }

        public static Settings Deserialize(string filePath)
        {
            SM.Log.Method(filePath);
            CreateDefaultIfMissing(filePath);
            var deserializer      = new XmlSerializer(typeof(Settings));
            TextReader textReader = new StreamReader(filePath);
            var settings          = (Settings)deserializer.Deserialize(textReader);
            textReader.Close();
            if (settings.IsAnotherComputerSettings())
                return new Settings();
            if (settings.IsObsoleteSettings())
                return new Settings();
            return settings;
        }

        public static void Serialize(Settings settings)
        {
            Serialize(settings, DefaultFileName);
        }

        public static void Serialize(Settings settings, string filePath)
        {
            SM.Log.Method("", filePath);
            var serializer        = new XmlSerializer(typeof(Settings));
            TextWriter textWriter = new StreamWriter(filePath);
            serializer.Serialize(textWriter, settings);
            textWriter.Close();
        }

        #endregion

        #region IsAnotherComputerSettings

        public bool IsAnotherComputerSettings()
        {
            SM.Log.Method();
            return (ComputerName != Environment.MachineName);
        }

        #endregion

        #region IsObsoleteSettings

        public bool IsObsoleteSettings()
        {
            SM.Log.Method();
            // settings are obsolete if their version is < 3.4.0
            var obsolete = (String.Compare(ConfigVersion, "3.4.0", StringComparison.InvariantCulture) < 0);
            if (!obsolete && ConfigVersion != AssemblyInfo.Version)
            {
                ConfigVersion = AssemblyInfo.Version;
                Serialize(this);
            }
            return obsolete;
        }

        #endregion
    }
}

