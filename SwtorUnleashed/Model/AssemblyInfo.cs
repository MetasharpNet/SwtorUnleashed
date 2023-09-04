using System.IO;
using System.Linq;
using System.Reflection;

namespace SwtorUnleashed.Model
{
    public static class AssemblyInfo
    {
        #region private static properties

        private static Assembly ExecutingAssembly { get; set; }

        #endregion

        #region .ctor

        static AssemblyInfo()
        {
            ExecutingAssembly = Assembly.GetExecutingAssembly();
        }

        #endregion

        #region public static properties

        public static string Company
        {
            get
            {
                var a = ExecutingAssembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false).FirstOrDefault() as AssemblyCompanyAttribute;
                if (a == null)
                    return "";
                return a.Company;
            }
        }

        public static string Copyright
        {
            get
            {
                var a = ExecutingAssembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false).FirstOrDefault() as AssemblyCopyrightAttribute;
                if (a == null)
                    return "";
                return a.Copyright;
            }
        }

        public static string Description
        {
            get
            {
                var a = ExecutingAssembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false).FirstOrDefault() as AssemblyDescriptionAttribute;
                if (a == null)
                    return "";
                return a.Description;
            }
        }

        public static string Product
        {
            get
            {
                var a = ExecutingAssembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false).FirstOrDefault() as AssemblyProductAttribute;
                if (a == null)
                    return "";
                return a.Product;
            }
        }

        public static string Title
        {
            get
            {
                var a = ExecutingAssembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false).FirstOrDefault() as AssemblyTitleAttribute;
                if (a != null && a.Title != null)
                    return a.Title;
                return Path.GetFileNameWithoutExtension(ExecutingAssembly.CodeBase);
            }
        }

        public static string Version
        {
            get
            {
                return ExecutingAssembly.GetName().Version.ToString();
            }
        }

        #endregion
    }
}
