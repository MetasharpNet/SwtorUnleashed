using System;

namespace SwtorUnleashed.ViewModel.Base
{
    /// <summary>
    /// This command can be bound to the WPF/Silverlight UI element. If you define also a "bool CanCommandName()" for your "CommandName" command, it'll be automatically taken into account.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class UiCommandAttribute : Attribute
    {
    }
}
