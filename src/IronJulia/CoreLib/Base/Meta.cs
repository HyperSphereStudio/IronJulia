using System.Diagnostics.CodeAnalysis;
using IronJulia.AST;

public partial struct Base {

    public static class Meta {

        public static LoweredJLExpr.ILoweredJLExpr Lower(object v) {
            return null;
        }
        
        public static object MacroExpand1(Core.Module m, object v) {
            switch (v) {
                case Expr ex:
                    var vx = ex.args.vec;
                    if (ex.head == CommonSymbols.macrocall_sym)
                        return ExpandMacro(m, vx);
                    for (var i = 1; i <= vx.Length; i++) {
                        vx[i] = MacroExpand1(m, vx[i]);
                    }
                    break;
            }
            return v;
        }

        public static bool TryGetKeyArg(object v, [NotNullWhen(true)] out Symbol? key, [NotNullWhen(true)] out object? arg) {
            if (v is Expr expr && expr.head == CommonSymbols.assign_sym && expr.args[1] is Symbol sym) {
                key = sym;
                arg = expr.args[2];
            }
            key = null;
            arg = null;
            return false;
        }

        private static object ExpandMacro(Core.Module m, Core.SimpleVector<object> args) {
            var cs = RuntimeJulianCallsite.Get();
            for (var i = 2; i <= args.Count; i++) {
                var v = args[i];
                if (TryGetKeyArg(v, out var key, out var arg))
                    cs.AddKeyArg(key.Value, new(arg));
                else
                    cs.AddArg(new(v));
            }
            return ((Core.Function) m[(Symbol) args[1]]!).Invoke(cs)!;
        }
        
    }
    
}