global using VoidPtr = Core.Ptr<Base.Nothing>;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using IronJulia.CoreLib.Interop;
using SpinorCompiler.Boot;
using static Base;

public partial struct Core {
    public static readonly Package Package;
    public static readonly Module ModuleValue;
    
    static Core() {
        Package = new(new AssemblyName("Core"));
    }
    
}