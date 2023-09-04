using System;

namespace SwtorUnleashed.ViewModel.Base
{
    public class CommandEventArgs : EventArgs
    {
        #region properties

        #region private

        private readonly CommandAdapter _adapter;
        private readonly CommandInfo    _command;

        #endregion

        public  CommandAdapter Adapter { get { return _adapter; } }
        public  CommandInfo    Command { get { return _command; } }

        #endregion

        #region .ctor

        public CommandEventArgs(CommandAdapter adapter, CommandInfo command)
        {
            _adapter = adapter;
            _command = command;
        }

        #endregion
    }
}
