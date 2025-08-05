using System.Diagnostics;
using SpinorCompiler.Utils;

namespace IronJulia.AST;

public abstract class NonRecursiveGraphVisitor<T> where T:INodeVisitor<T> {
    public RandomAccessStack<NodeVisitorState<T>> Frames = new();

    public void PushNewVisit(T node, uint state = 0) {
        Debug.Assert(node != null);
        Frames.Push(new NodeVisitorState<T>
        {
            Node = node,
            State = state,
            VisitState = VisitState.VisitStateStart
        });
    }

    public abstract void VisitStatesStart(ref NodeVisitorState<T> state);
    public abstract void VisitStatesEnd(ref NodeVisitorState<T> state);
    public abstract void AfterStateVisit(ref NodeVisitorState<T> state);

    public virtual void Visit(T expr) {
        PushNewVisit(expr);
        while (Frames.Count > 0) {
            var eidx = Frames.Count - 1;
            var node = Frames[eidx];
            uint? ls;

            switch (node.VisitState) {
                case VisitState.VisitStateStart:
                    node.LastState = uint.MaxValue;
                    VisitStatesStart(ref node);
                    ls = node.State;
                    
                    if (node.State == null)
                        node.VisitState = VisitState.VisitStatesEnd;
                    else {
                        node.State = node.Node.Visit(this, node);
                        node.LastState = ls;
                        node.VisitState = VisitState.VisitedState;
                    }

                    Frames[eidx] = node;
                    
                    break;
                case VisitState.VisitedState:
                    var iterateNextState = node.VisitState == VisitState.VisitedState;
                    
                    AfterStateVisit(ref node);
                    ls = node.State;
                    
                    iterateNextState &= node.State != null;
 
                    if (iterateNextState)
                        node.State = node.Node.Visit(this, node);

                    Frames[eidx] = node with {
                        VisitState = iterateNextState ? VisitState.VisitedState : VisitState.VisitStatesEnd,
                        LastState = ls
                    };
                    
                    break;
                case VisitState.VisitStatesEnd:
                    Frames.Pop();
                    VisitStatesEnd(ref node);
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