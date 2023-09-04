using System;
using System.ComponentModel;

namespace SwtorUnleashed.ViewModel.Base
{
    public class PropertyChangedNotifier : INotifyPropertyChanged
    {
        #region consts

        protected const string NotNullOrEmptyError = "Must not be null or empty";

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// trigger PropertyChanged event providing the exact C# property name (to support two-way data binding)
        /// </summary>
        /// <param name="propertyName">exact C# property name</param>
        protected void TriggerPropertyChanged(string propertyName)
        {
            #region args check

            if (String.IsNullOrEmpty(propertyName))
                throw new ArgumentException(NotNullOrEmptyError, "propertyName");

            #endregion
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
