using System.Reflection;
using System.Runtime.CompilerServices;

namespace IronJulia.CoreLib.Interop;

public class NetMethod : Core.Method{
    public override Type ReturnType => Info.ReturnType;
    public readonly MethodInfo? Info;
    public ReadOnlySpan<ParameterInfo> ParameterSpan => GetParametersAsSpan(Info);

    public NetMethod(Core.Function f, MethodInfo? info) : base(f) {
        Info = info;
        foreach (var p in ParameterSpan)
            Specialization += JuliaTypeSpecializer.GetTypeSpecialization(p.ParameterType);
    }

    public override MethodToCallSiteMatch Match(Span<Type> argTypes, Span<Type> kargTypes, Span<string> knames) {
        foreach (var p in ParameterSpan) {
            var pidx = knames!.IndexOf(p.Name);
            if (pidx != -1) {
                if (!p.ParameterType.IsAssignableFrom(kargTypes[pidx]))
                    return MethodToCallSiteMatch.NoMatch;
            }
            else {
                if (argTypes.Length > 0) {
                    if (!p.ParameterType.IsAssignableFrom(argTypes[0]))
                        return MethodToCallSiteMatch.NoMatch;
                    argTypes = argTypes[1..];  //Next
                }else if (!p.IsOptional)
                    return MethodToCallSiteMatch.NoMatch;
            }
        }
        
        //To many args remaining
        if(argTypes.Length != 0)
            return MethodToCallSiteMatch.NoMatch;  
        
        return MethodToCallSiteMatch.Exact;
    }

    [UnsafeAccessor(UnsafeAccessorKind.Method)]
    private static extern ReadOnlySpan<ParameterInfo> GetParametersAsSpan(MethodBase? info);
    
    public override object? Invoke(Span<object?> args, Span<object?> kargs, Span<string> knames) {
        var pspan = ParameterSpan;
        var fullArgs = new object?[pspan.Length];
        var fargs = fullArgs.AsSpan();
        foreach (var p in ParameterSpan) {
            var pidx = knames!.IndexOf(p.Name);
            if (pidx != -1)
                fargs[0] = kargs[pidx];
            else {
                if (args.Length > 0) {
                    fargs[0] = args[0];
                    args = args[1..];  //Next
                }else if (p.IsOptional)
                    fargs[0] = p.DefaultValue;
                else
                    throw new Exception();
            }
            fargs = fargs[1..];  //Next
        }
        return Info.Invoke(null, fullArgs);
    }
}
