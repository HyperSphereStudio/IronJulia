using System.Linq.Expressions;

namespace IronJulia.Compiler;

public abstract class CompilationMode {
    public abstract Expression EmitGetField();
    
}

public class UncollectableCompilationMode : CompilationMode {
    public override Expression EmitGetField()
    {
        throw new NotImplementedException();
    }
}

public class DiscCompilationMode : CompilationMode {
    public override Expression EmitGetField()
    {
        throw new NotImplementedException();
    }
}

