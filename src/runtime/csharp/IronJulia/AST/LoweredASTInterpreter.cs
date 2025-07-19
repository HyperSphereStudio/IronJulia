using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.ObjectPool;

namespace IronJulia.AST;

using static LoweredJLExpr;

public class LoweredASTInterpreter : NonRecursiveGraphVisitor<ILoweredJLExpr>
{
    private readonly HashSet<ILoweredJLExpr> _activeSet = new();
    public readonly Dictionary<Variable, object?> Vars = new();
    private readonly Dictionary<Label, int> _labels = new();
    private object? _lastValue;
    private readonly DefaultObjectPool<Callsite> _callsitePool = new(new DefaultPooledObjectPolicy<Callsite>());

    public object? Interpret(Block expr, bool reset) {
        if (reset) {
            Vars.Clear();
            _labels.Clear();
            _activeSet.Clear();
        }
        _lastValue = null;
        Visit(expr);
        return _lastValue;
    }
    
    public override bool VisitStatesStart(ILoweredJLExpr expr, ref object? data) {
        if (!_activeSet.Add(expr))
            throw new NotSupportedException("Cannot recursively visit the same node!");
        switch (expr) {
            case Block:
            case Assignment:
            case Goto:
            case Variable:
            case Constant:
                break;
            case While:
                break;
            case Label lb:
                _labels.Add(lb, Frames.Count);
                break;
            case FunctionInvoke:
                data = _callsitePool.Get();
                break;
            default:
                throw new NotSupportedException(expr.GetType().Name);
        }
        return true;
    }

    
    //Dont trigger any VisitChildrenEnd's
    private void UndoExpr(ILoweredJLExpr expr) {
        _activeSet.Remove(expr);
        switch (expr) {
            case Label lbl:
                _labels.Remove(lbl);
                break;
            case Variable var:
                Vars.Remove(var);
                break;
            case Block bk:
                foreach(var lbl in bk.Labels)
                    _labels.Remove(lbl);
                foreach(var v in bk.Variables)
                    Vars.Remove(v);
                break;
        }
    }
    
    public override void VisitStatesEnd(ILoweredJLExpr expr, ref object? data) {
        _activeSet.Remove(expr);
        switch (expr) {
            case While:
            case Label:
                break;
            case Block bk:
                UndoExpr(bk);
                break;
            case Variable v:
                _lastValue = Vars[v];
                break;
            case Constant ck:
                _lastValue = ck.Value;
                break;
            case Assignment asn:
                Vars[asn.Variable] = _lastValue;
                break;
            case FunctionInvoke fi:
                var fid = ((Callsite?)data)!;
                _lastValue = fi.Function.Invoke(fid);
                fid.Reset();
                _callsitePool.Return(fid);
                break;
            case Goto gt:
                var lblState = _labels[gt.Label];
                while (Frames.Count > lblState)    //Go back to label frame
                    UndoExpr(Frames.Pop().Node);
                break;
            default:
                throw new NotSupportedException(expr.GetType().Name);
        }
        
    }

    public bool TryReadBoolean([NotNullWhen(true)] out bool? b) {
        if (_lastValue is Base.Bool bv) {
            b = bv.Value;
            return true;
        }
        b = null;
        return false;
    }

    public override bool AfterStateVisit(ILoweredJLExpr expr, ref object? data, uint? state) {
        switch (expr) {
            case Block:
            case Assignment:
            case Goto:
            case Label:
            case Variable:
            case Constant:
                break;
            case While:
                if (state == While.EvalCondState) {
                    if (!TryReadBoolean(out var v))
                        throw new NotSupportedException("Conditional in Loop Is Non Boolean!");
                    if (!v.Value)
                        return false;
                    
                    //Repeat this block, note that the previous node is going to be a Block Expr.
                    var f = Frames[^2];
                    Frames[^2] = f with { State = f.LastState, LastState = f.LastState - 1 };      
                }
                break;
            case FunctionInvoke:
                var fid = ((Callsite?) data)!;
                fid.AddArg(_lastValue);
                break;
            default:
                throw new NotSupportedException(expr.GetType().Name);
        }
        return true;
    }
}