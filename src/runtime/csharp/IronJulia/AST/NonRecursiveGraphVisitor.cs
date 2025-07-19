using System.Diagnostics;
using SpinorCompiler.Utils;

namespace IronJulia.AST;

public abstract class NonRecursiveGraphVisitor<T> where T:INodeVisitor<T> {
    public RandomAccessStack<NodeVisitorState<T>> Frames = new();

    public void PushNewVisit(T node, uint state = 0)
    {
        Frames.Push(new NodeVisitorState<T>
        {
            Node = node,
            State = state,
            VisitState = VisitState.VisitStateStart
        });
    }

    public abstract bool VisitStatesStart(T expr, ref object? data);
    public abstract void VisitStatesEnd(T expr, ref object? data);
    public abstract bool AfterStateVisit(T expr, ref object? data, uint? state);

    public virtual void Visit(T expr)
    {
        PushNewVisit(expr);
        while (Frames.Count > 0)
        {
            var eidx = Frames.Count - 1;
            var node = Frames[eidx];
            uint? s = null;
            var iterateNextState = true;

            switch (node.VisitState)
            {
                case VisitState.VisitStateStart:
                    if (!VisitStatesStart(node.Node, ref node.Data)) {
                        iterateNextState = false;
                    }
                    else
                        s = node.Node.Visit(this, node);

                    Frames[eidx] = node with
                    {
                        State = s,
                        VisitState = iterateNextState ? VisitState.VisitedState : VisitState.VisitStatesEnd,
                        LastState = node.State
                    };
                    break;
                case VisitState.VisitedState:
                    iterateNextState = node.VisitState == VisitState.VisitedState;
                    iterateNextState &= AfterStateVisit(node.Node, ref node.Data, node.LastState);
                    iterateNextState &= node.State != null;

                    if (iterateNextState)
                        s = node.Node.Visit(this, node);

                    Frames[eidx] = node with
                    {
                        State = s,
                        VisitState = iterateNextState ? VisitState.VisitedState : VisitState.VisitStatesEnd,
                        LastState = node.State
                    };
                    break;
                case VisitState.VisitStatesEnd:
                    VisitStatesEnd(node.Node, ref node.Data);
                    Frames.Pop();
                    break;
                default:
                    throw new UnreachableException();
            }
        }
    }
}

internal enum VisitState
{
    VisitStateStart,
    VisitedState,
    VisitStatesEnd
}

public struct NodeVisitorState<T>
{
    public T Node;
    public uint? State { get; internal set; }
    internal uint? LastState;
    internal VisitState VisitState;
    public object? Data;
}

public interface INodeVisitor<T> where T: INodeVisitor<T> {
    public uint? Visit(NonRecursiveGraphVisitor<T> visit, NodeVisitorState<T> state);
}