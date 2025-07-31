using System.Reflection;
using System.Reflection.Emit;

namespace IronJulia.Emit;

public class CodeGenerator : ILocalCache{
    public readonly ILGenerator il;
    public readonly MethodBase InternalMethod;
    private readonly HashSet<LocalBuilder> FreeLocals = new();

    private CodeGenerator(ILGenerator il, MethodBase internalMethod) {
        this.il = il;
        InternalMethod = internalMethod;
    }

    public CodeGenerator(MethodBuilder mb) : this(mb.GetILGenerator(), mb)
    {
    }

    public CodeGenerator(ConstructorBuilder cb) : this(cb.GetILGenerator(), cb)
    {
    }

    public CodeGenerator(DynamicMethod cb) : this(cb.GetILGenerator(), cb) {
        
    }

    public LocalBuilder GetLocal(Type type) {
        foreach (var f in FreeLocals)
            if (f.LocalType == type) {
                FreeLocals.Remove(f);
                return f;
            }
        return il.DeclareLocal(type);
    }

    public void FreeLocal(LocalBuilder local) =>  FreeLocals.Add(local);
  
}