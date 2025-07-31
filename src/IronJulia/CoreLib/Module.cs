using System.Reflection;
using System.Reflection.Emit;

namespace SpinorCompiler.Boot;

public class Package {
    public AssemblyBuilder Assembly;
    public ModuleBuilder Module;

    public Package(AssemblyName name) {
        Assembly = AssemblyBuilder.DefineDynamicAssembly(name, AssemblyBuilderAccess.RunAndCollect);
        Module = Assembly.DefineDynamicModule(name.Name + ".dll");
        
    }
}