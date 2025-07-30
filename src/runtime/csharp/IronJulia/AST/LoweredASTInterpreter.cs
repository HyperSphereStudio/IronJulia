using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.ObjectPool;

namespace IronJulia.AST;

using static LoweredJLExpr;

public class LoweredASTInterpreter : NonRecursiveGraphVisitor<ILoweredJLExpr>
{
    private readonly HashSet<ILoweredJLExpr> _activeSet = new();
    public readonly Dictionary<Variable, object?> Vars = new();
    private readonly Dictionary<Label, (int FrameIndex, uint? BlockInstrIndex)> _labels = new();
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
    
    public override void VisitStatesStart(ILoweredJLExpr expr, ref object? data, ref uint? nextState) {
        
        if (!_activeSet.Add(expr))
            throw new NotSupportedException("Cannot recursively visit the same node!");
        switch (expr) {
            case Assignment:
            case Goto:
            case Variable:
            case While:
            case Conditional:
            case Constant:
                break;
            case Block:
                _lastValue = null;
                break;
            case Label lb:
                Debug.Assert(Frames[^2].Node is Block);
                _labels.Add(lb, (Frames.Count - 1, Frames[^2].LastState));
                break;
            case FunctionInvoke:
                data = _callsitePool.Get();
                break;
            default:
                throw new NotSupportedException(expr.GetType().Name);
        }
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
            case Conditional:
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
                
                //Pop until returns to last valid frame
                while (Frames.Count > lblState.FrameIndex)
                    UndoExpr(Frames.Pop().Node);

                var activeFrame = Frames.Peek();
                var activeBlock = (Block) activeFrame.Node;
                for (var i = activeFrame.LastState!.Value; i > lblState.BlockInstrIndex; i--) {
                    UndoExpr(activeBlock.Statements[(int) i]); //Walk backwards the block
                }
                
                Frames[^1] = activeFrame with {
                    State = lblState.BlockInstrIndex + 1, LastState = lblState.BlockInstrIndex!.Value
                };
                
                break;
            default:
                throw new NotSupportedException(expr.GetType().Name);
        }
    }

    private object? ConvertTo(object? v, Type t) {
        if (v == null) {
            if (t == typeof(Base.Nothing))
                return Base.Nothing.Instance;
            throw new Exception("Unable to convert null to " + t);
        }
        if(v.GetType().IsAssignableTo(t))
            return v;
        
        var cs = _callsitePool.Get();
        cs.AddArg(t);
        cs.AddArg(v);
        var o = Base.convert.Invoke(cs);
        cs.Reset();
        _callsitePool.Return(cs);
        return o;
    }

    public bool ReadBoolean() {
        if (_lastValue is bool v)
            return v;
        return ((Base.Bool) ConvertTo(_lastValue, typeof(Base.Bool))!).Value;
    }

    public override void AfterStateVisit(ILoweredJLExpr expr, ref object? data, uint? state, ref uint? nextState) {
        switch (expr) {
            case Block:
            case Assignment:
            case Goto:
            case Label:
            case Variable:
            case Constant:
                break;
            case Conditional c:
                if (state == Conditional.EvalCondState) {
                    nextState = ReadBoolean() ? Conditional.EvalBodyState : (c.Else != null ? Conditional.EvalElseState : null);
                }
                break;
            case While:
                if (state == While.EvalCondState) {
                    if (!ReadBoolean()) {
                        nextState = null;
                        return;
                    }
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
    }
}