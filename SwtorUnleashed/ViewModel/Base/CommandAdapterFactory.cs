using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Windows.Input;

namespace SwtorUnleashed.ViewModel.Base
{
    internal static class CommandAdapterFactory<T>
        where T : class
    {
        #region members

        private static readonly Type _type;

        #endregion

        #region .ctor

        static CommandAdapterFactory()
        {
            try
            {
                TypeBuilder tb = CodeGen.Module.DefineType(typeof(T).Name + "*Commands", TypeAttributes.Class, typeof(CommandAdapter));
                var t          = typeof(T);
                var methods    = t.GetMethods();
                foreach (var m in methods)
                {
                    var attr = m.GetCustomAttributes(typeof(UiCommandAttribute), true);
                    if (attr.Length != 0)
                    {
                        FieldBuilder  fb     = tb.DefineField("_" + m.Name, typeof(ICommand), FieldAttributes.Private);
                        MethodBuilder mbGet  = tb.DefineMethod("get_" + m.Name, MethodAttributes.Public, typeof(ICommand), new Type[] { });
                        ILGenerator   ilgGet = mbGet.GetILGenerator();
                        ilgGet.Emit(OpCodes.Ldarg_0);
                        ilgGet.Emit(OpCodes.Ldfld, fb);
                        ilgGet.Emit(OpCodes.Ret);
                        MethodBuilder mbSet = tb.DefineMethod("set_" + m.Name, MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName, typeof(void), new[] { typeof(ICommand) });
                        ILGenerator ilgSet  = mbSet.GetILGenerator();
                        ilgSet.Emit(OpCodes.Ldarg_0);
                        ilgSet.Emit(OpCodes.Ldarg_1);
                        ilgSet.Emit(OpCodes.Stfld, fb);
                        ilgSet.Emit(OpCodes.Ret);
                        PropertyBuilder pb = tb.DefineProperty(m.Name, PropertyAttributes.None, typeof(ICommand), new Type[] { });
                        pb.SetGetMethod(mbGet);
                        pb.SetSetMethod(mbSet);
                    }
                }
                _type = tb.CreateType();
            }
            catch (Exception ex)
            {
                Debug.Fail(ex.Message);
            }
        }

        #endregion

        #region Create

        [DebuggerHidden]
        public static object Create(object sender)
        {
            var constructorInfo = _type.GetConstructor(new Type[] {});
            if (constructorInfo == null)
                throw new ApplicationException("Couldn't get a Type[] constructor.");
            var ca      = (CommandAdapter)constructorInfo.Invoke(new object[] { });
            var t       = typeof(T);
            var methods = t.GetMethods();
            foreach (var m in methods)
            {
                var attr = m.GetCustomAttributes(typeof(UiCommandAttribute), true);
                if (attr.Length != 0)
                {
                    string methodName      = m.Name;
                    string canDoMethodName = "Can"+methodName;
                    var hasCanDoMethod = (from method in methods where method.Name == canDoMethodName select method).FirstOrDefault() != null;
                    var cmd = new SimpleCommand(x =>
                        // executeMethod
                        {
                            var cea = new CommandEventArgs(ca, new CommandInfo { Name = methodName });
                            ca.DoBeforeCommand(cea);
                            try
                            {
                                MethodInfo miLocal = t.GetMethod(methodName);
                                ParameterInfo[] pi = miLocal.GetParameters();
                                if (pi.Length == 0)
                                    t.InvokeMember(methodName, BindingFlags.InvokeMethod, null, sender, new object[] {});
                                else if (pi.Length == 1)
                                    t.InvokeMember(methodName, BindingFlags.InvokeMethod, null, sender, new[] { x });
                            }
                            catch (Exception ex)
                            {
                                Trace.Fail(ex.Message);
                            }
                            finally
                            {
                                ca.DoAfterCommand(cea);
                            }
                        },
                        // canExecuteMethod
                        hasCanDoMethod ? (Predicate<object>)(x =>
                            {
                                return (bool)t.InvokeMember(canDoMethodName, BindingFlags.InvokeMethod, null, sender, new object[] { });
                            })
                                       : null);
                    _type.InvokeMember(m.Name, BindingFlags.SetProperty, null, ca, new object[] { cmd });
                    ca.Commands.Add(cmd);
                }
            }
            return ca;
        }

        #endregion
    }
}
