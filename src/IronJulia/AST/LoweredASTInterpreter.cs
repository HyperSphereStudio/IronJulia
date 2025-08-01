using System.Diagnostics;
using System.Dynamic;
using System.Runtime.CompilerServices;
using IronJulia.CoreLib.Interop;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Extensions.ObjectPool;

namespace IronJulia.AST;

using static LoweredJLExpr;

public class LoweredASTInterpreter : NonRecursiveGraphVisitor<ILoweredJLExpr> {
    private readonly HashSet<ILoweredJLExpr> _activeSet = new();
    public readonly Dictionary<Variable, object?> Vars = new();
    private readonly Dictionary<Label, (int FrameIndex, uint? BlockInstrIndex)> _labels = new();
    private object? _lastValue;
    private readonly DefaultObjectPool<Callsite> _callsitePool = new(new DefaultPooledObjectPolicy<Callsite>());
    
    public void PrintActiveVariables(TextWriter? textWriter = null) {
        textWriter ??= Console.Out;
        foreach(var v in Vars){
            textWriter.Write(v.Key.Name);
            textWriter.Write(" = ");
            textWriter.WriteLine(v.Value);
        }
    }
    
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
    
    public override void VisitStatesStart(ref NodeVisitorState<ILoweredJLExpr> state) {
        if (!_activeSet.Add(state.Node))
            throw new NotSupportedException("Cannot recursively visit the same node!");
        switch (state.Node) {
            case Constant:
            case Assignment:
            case Variable:
            case GetProperty:
            case SetProperty:
                break;
            
            case FunctionInvoke:
                state.Data = _callsitePool.Get();
                break;
            
            case Block:
                _lastValue = null;
                break;
            
            case Conditional:
            case While:
            case For:
                break;
                
            case Label lb:
                Debug.Assert(Frames[^2].Node is Block);
                _labels.Add(lb, (Frames.Count - 1, Frames[^2].LastState));
                break;
            
            case Goto:
                break;
            default:
                throw new NotSupportedException(state.Node.GetType().Name);
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
                    Vars.Remove(v.Value);
                break;
            case For fr:
                Vars.Remove(fr.State); //Exposed to block scope
                break;
        }
    }
    
    private void ExecuteSetProperty(object? instance, object? value, Base.Symbol propertyName) {
        if (instance == null)
            throw new NullReferenceException();
        
        if (instance is Core.Module m) {
            m[propertyName] = value;
            return;
        }

        throw new NotSupportedException();
    }
    
    private void ExecuteGetProperty(object? instance, Base.Symbol propertyName) {
        if (instance == null)
            throw new NullReferenceException();

        if (instance is Core.Module m) {
            _lastValue = m[propertyName];
            return;
        }

        throw new NotSupportedException();
    }
    
    public override void VisitStatesEnd(ref NodeVisitorState<ILoweredJLExpr> state) {
        _activeSet.Remove(state.Node);
        switch (state.Node) {
            case Constant ck:
                _lastValue = ck.Value;
                break;
            case Variable v:
                _lastValue = Vars[v];
                break;
            case Assignment asn:
                Vars[asn.Variable] = _lastValue;
                break;
            case FunctionInvoke fi:
                var fid = ((Callsite?)state.Data)!;
                _lastValue = fi.Function.Invoke(fid);
                fid.Reset();
                _callsitePool.Return(fid);
                break;
            
            case GetProperty gp:
                ExecuteGetProperty(_lastValue, gp.Name);
                break;
            
            case SetProperty sp:
                ExecuteSetProperty(_lastValue, state.Data, sp.Name);
                break;
            
            case Conditional:
            case While:
            case For:
            case Label:
                break;
            
            case Block bk:
                UndoExpr(bk);
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
                throw new NotSupportedException(state.Node.GetType().Name);
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

    public override void AfterStateVisit(ref NodeVisitorState<ILoweredJLExpr> state) {
        switch (state.Node) {
            case Block:
            case Assignment:
            case Goto:
            case Label:
            case Variable:
            case Constant:
            case GetProperty:
                break;
            case SetProperty:
                if (state.LastState == SetProperty.EvalValue) {
                    state.Data = _lastValue;
                }
                break;
            case Conditional c:
                if (state.LastState == Conditional.EvalCondState) {
                    state.State = ReadBoolean() ? Conditional.EvalBodyState : (c.Else != null ? Conditional.EvalElseState : null);
                }
                break;
            case While:
                if (state.LastState == While.EvalCondState) {
                    if (!ReadBoolean()) {
                        state.State = null;
                        return;
                    }
                    var f = Frames[^2];
                    Debug.Assert(f.Node is Block);
                    Frames[^2] = f with { State = f.LastState, LastState = f.LastState - 1 }; //goto condition after body
                }
                break;
            case For fl:
                switch (state.State) {
                    case For.EvalInitialization:
                        Vars[fl.State] = _lastValue;       //state = iterate(iterable)
                        break;
                    case For.EvalCondState when ReferenceEquals(_lastValue, Base.Nothing.Instance): // state !== nothing
                        state.State = null;
                        return;
                    case For.EvalCondState: {
                        var f = Frames[^2];
                        Debug.Assert(f.Node is Block);
                        Frames[^2] = f with { State = f.LastState, LastState = f.LastState - 1 }; //goto condition after body
                        break;
                    }
                    case For.EvalNextState:
                        Vars[fl.State] = _lastValue;       //state = iterate(iterable, state)
                        break;
                }
                break;
            case FunctionInvoke:
                var fid = ((Callsite?) state.Data)!;
                fid.AddArg(_lastValue);
                break;
            default:
                throw new NotSupportedException(state.Node.GetType().Name);
        }
    }
}