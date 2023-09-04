using SwtorUnleashed.ViewModel.Base;

namespace SwtorUnleashed.ViewModel
{
    public class Asset : PropertyChangedNotifier
    {
        #region properties

        #region underlying members

        private bool   _isChecked;
        private string _name;

        #endregion

        public bool   IsChecked { get { return _isChecked; } set { if (value != _isChecked) { _isChecked = value; TriggerPropertyChanged("IsChecked"); } } }
        public string Name      { get { return _name;      } set { if (value != _name     ) { _name      = value; TriggerPropertyChanged("Name");      } } }

        #endregion
    }
}
