using System.Linq;

namespace SwtorUnleashed.ViewModel.Base
{
    public class ViewModel : PropertyChangedNotifier
    {
        #region properties

        private CommandAdapter _commands;
        public  CommandAdapter Commands
        {
            get
            {
                // this magic will return CommandAdapter specific for the current class
                if (_commands == null)
                {
                    var type  = typeof(CommandAdapterFactory<>).MakeGenericType(new[] { GetType() });
                    var methodInfo = type.GetMember("Create").FirstOrDefault() as System.Reflection.MethodInfo;
                    if (methodInfo != null)
                        _commands = (CommandAdapter)methodInfo.Invoke(null, new object[] { this });
                }
                return _commands;
            }
        }

        #endregion
    }
}
