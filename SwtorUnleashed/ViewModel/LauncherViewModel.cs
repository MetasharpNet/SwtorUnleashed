using System.Windows;
using SwtorUnleashed.Model;
using SwtorUnleashed.ViewModel.Base;

namespace SwtorUnleashed.ViewModel
{
    public class LauncherViewModel : Base.ViewModel
    {
        #region window

        private Window _window;

        #endregion

        #region properties

        #region underlying members

        private string _windowTitle;

        #endregion

        public string WindowTitle { get { return _windowTitle; } set { if (value != _windowTitle) { _windowTitle = value; TriggerPropertyChanged("WindowTitle"); } } }

        #endregion

        #region commands

        [UiCommand]
        public void OnAuthorizedByBioWareUSClick()
        {
            Log.User();
            Launcher.ShowBioWareAgreementUS();
        }

        [UiCommand]
        public void OnAuthorizedByBioWareFRClick()
        {
            Log.User();
            Launcher.ShowBioWareAgreementFR();
        }

        [UiCommand]
        public void OnRemoveQuitClick()
        {
            Log.User();
            Launcher.RemoveUnleashed(true);
            _window.Close();
        }

        [UiCommand]
        public void OnSwtorRetailClick()
        {
            Log.User();
            Launcher.StartRetail();
            _window.Close();
        }

        [UiCommand]
        public void OnSwtorUnleashedClick()
        {
            Log.User();
            Launcher.StartUnleashed();
            _window.Close();
        }

        [UiCommand]
        public void OnWindowLoaded(Window window)
        {
            Log.Method();
            _window     = window;
            WindowTitle = AssemblyInfo.Product + " " + AssemblyInfo.Version;
        }

        #endregion
    }
}
