using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace IronJulia.CoreLib.Interop;

public class NetMethod : Core.Method{
    public override Type ReturnType => Info.ReturnType;
    public readonly MethodInfo Info;
    public ReadOnlySpan<ParameterInfo> ParameterSpan => GetParametersAsSpan(Info);

    public NetMethod(Core.Function f, MethodInfo info) : base(f) {
        Info = info;
        Debug.Assert(Info != null);
        foreach (var p in ParameterSpan)
            Specialization += JuliaTypeSpecializer.GetTypeSpecialization(p.ParameterType);
    }

    public override MethodToCallSiteMatch Match<TCVal, TVal>(Span<TCVal> args, Span<TCVal> kargs, Span<Base.Symbol> knames) {
        foreach (var p in ParameterSpan) {
            var pidx = knames!.IndexOf(p.Name!);
            if (pidx != -1) {
                if (!p.ParameterType.IsAssignableFrom(kargs[pidx].Type))
                    return MethodToCallSiteMatch.NoMatch;
            }
            else {
                if (args.Length > 0) {
                    if (!p.ParameterType.IsAssignableFrom(args[0].Type))
                        return MethodToCallSiteMatch.NoMatch;
                    args = args[1..];  //Next
                }else if (!p.IsOptional)
                    return MethodToCallSiteMatch.NoMatch;
            }
        }
        
        //To many args remaining
        if(args.Length != 0)
            return MethodToCallSiteMatch.NoMatch;  
        
        return MethodToCallSiteMatch.Exact;
    }

    [UnsafeAccessor(UnsafeAccessorKind.Method)]
    private static extern ReadOnlySpan<ParameterInfo> GetParametersAsSpan(MethodBase? info);
    
    public override object? Invoke(Span<RuntimeValue> args, Span<RuntimeValue> kargs, Span<Base.Symbol> knames) {
        var pspan = ParameterSpan;
        var fullArgs = new object?[pspan.Length];
        var fargs = fullArgs.AsSpan();
        
        foreach (var p in ParameterSpan) {
            var pidx = knames!.IndexOf(p.Name!);
            if (pidx != -1)
                fargs[0] = kargs[pidx].Value;
            else {
                if (args.Length > 0) {
                    fargs[0] = args[0].Value;
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

    public override Delegate CreateDelegate(Type t) => Info.CreateDelegate(t);
    public override T CreateDelegate<T>() => Info.CreateDelegate<T>();
}
