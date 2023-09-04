using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace SwtorUnleashed.ViewModel.Base
{
    public class CommandAdapter
    {
        #region properties

        #region underlying members

        private readonly List<SimpleCommand> _commands = new List<SimpleCommand>();

        #endregion

        public List<SimpleCommand> Commands { get { return _commands; } }
        public object              Tag      { get; set; }

        #endregion

        #region events

        public event EventHandler<CommandEventArgs> AfterCommand;
        public event EventHandler<CommandEventArgs> BeforeCommand;

        #endregion

        #region .ctor

        public CommandAdapter()
        {
            CommandManager.RequerySuggested += CommandManager_RequerySuggested;
        }

        #endregion

        #region methods

        void CommandManager_RequerySuggested(object sender, EventArgs e)
        {
            RefreshAllCommands();
        }

        public void DoAfterCommand(CommandEventArgs args)
        {
            var eh = AfterCommand;
            if (eh != null)
                eh(this, args);        
        }

        public void DoBeforeCommand(CommandEventArgs args)
        {
            var eh = BeforeCommand;
            if (eh != null)
                eh(this, args);
        }

        public void RefreshAllCommands()
        {
            foreach (var c in Commands)
                c.RaiseCanExecuteChanged();
        
        }

        #endregion
    }
}
