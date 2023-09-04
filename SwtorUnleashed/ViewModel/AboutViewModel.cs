using SwtorUnleashed.Model;
using SwtorUnleashed.ViewModel.Base;

namespace SwtorUnleashed.ViewModel
{
    public class AboutViewModel : Base.ViewModel
    {
        #region properties

        #region underlying members

        private string _company;
        private string _copyright;
        private string _description;
        private string _product;
        private string _title;
        private string _version;
        private string _windowTitle;

        #endregion

        public string Company      { get { return _company;     } set { if (value != _company    ) { _company     = value; TriggerPropertyChanged("Company");     } } }
        public string Copyright    { get { return _copyright;   } set { if (value != _copyright  ) { _copyright   = value; TriggerPropertyChanged("Copyright");   } } }
        public string Description  { get { return _description; } set { if (value != _description) { _description = value; TriggerPropertyChanged("Description"); } } }
        public string Product      { get { return _product;     } set { if (value != _product    ) { _product     = value; TriggerPropertyChanged("Product");     } } }
        public string Title        { get { return _title;       } set { if (value != _title      ) { _title       = value; TriggerPropertyChanged("Title");       } } }
        public string Version      { get { return _version;     } set { if (value != _version    ) { _version     = value; TriggerPropertyChanged("Version");     } } }
        public string WindowTitle  { get { return _windowTitle; } set { if (value != _windowTitle) { _windowTitle = value; TriggerPropertyChanged("WindowTitle"); } } }

        #endregion

        #region commands

        [UiCommand]
        public void OnWindowLoaded()
        {
            Log.Method();
            Company     = AssemblyInfo.Company;
            Copyright   = AssemblyInfo.Copyright;
            Description = AssemblyInfo.Description;
            Product     = AssemblyInfo.Product;
            Title       = AssemblyInfo.Title;
            Version     = AssemblyInfo.Version;
            WindowTitle = "About " + Product;
        }

        #endregion
    }
}
