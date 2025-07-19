namespace IronJulia.AST;

using static LoweredJLExpr;

public class LoweredTypeAnalyzer : NonRecursiveGraphVisitor<ILoweredJLExpr> {
    private Type _lastType;
    
    public void Analyze(ILoweredJLExpr expr) {
        _lastType = typeof(Base.Any);
        Visit(expr);
    }
    
    public override bool VisitStatesStart(ILoweredJLExpr expr, ref object? data) {
        switch (expr) {
            case Block:
            case Assignment:
            case Goto:
            case Variable:
            case Constant:
            case Label:
                break;
            case FunctionInvoke:
                break;
            default:
                throw new NotSupportedException(expr.GetType().Name);
        }
        return true;
    }

    public override void VisitStatesEnd(ILoweredJLExpr expr, ref object? data) {
        switch (expr) {
            case Assignment asn:
            case While:
            case Block:
                break;
            
            case Label:
            case Goto:
                return;
            
            case FunctionInvoke fi:
                _lastType = typeof(Base.Any);
                break;
            
            case Constant ck:
                _lastType = ck.ReturnType;
                break;
            case Variable v:
                _lastType = v.ReturnType;
                break;
            default:
                throw new NotSupportedException(expr.GetType().Name);
        }
        expr.ReturnType = _lastType;
    }

    public override bool AfterStateVisit(ILoweredJLExpr expr, ref object? data, uint? state) {
        switch (expr) {
            case Assignment:
            case While:
            case Block:
            case Label:
            case Goto:
            case FunctionInvoke:
            case Constant:
            case Variable:
                break;
            default:
                throw new NotSupportedException(expr.GetType().Name);
        }

        return true;
    }
}
