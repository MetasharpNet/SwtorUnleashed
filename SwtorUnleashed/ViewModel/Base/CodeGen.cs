using System;
using System.Reflection;
using System.Reflection.Emit;

namespace SwtorUnleashed.ViewModel.Base
{
    internal static class CodeGen
    {
        public static readonly AssemblyName    AssemblyName;
        public static readonly AssemblyBuilder Assembly;
        public static readonly ModuleBuilder   Module;

        static CodeGen()
        {
            AssemblyName = new AssemblyName("{2DC02D44-E1F2-4784-BF43-1D5E652A4194}");
            Assembly     = AppDomain.CurrentDomain.DefineDynamicAssembly(AssemblyName, AssemblyBuilderAccess.Run);
            Module       = Assembly.DefineDynamicModule("{06A6FDC6-15D9-49dd-9146-60F167F64E13}");
        }
    }
}
