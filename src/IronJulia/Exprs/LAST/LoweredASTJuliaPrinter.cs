using System.CodeDom.Compiler;
using IronJulia.CoreLib.Interop;

namespace IronJulia.AST;

using static LoweredJLExpr;

public class LoweredASTJuliaPrinter : NonRecursiveGraphVisitor<ILoweredJLExpr>
{
    public struct PrinterBlockState{
        public bool PrintStatementTypes, IsVisibleBlockDecor, PrintedStatement;
    }
    
    public IndentedTextWriter Writer;
    public PrinterBlockState ActiveState;
    public bool IsRootBlock;
    
    public LoweredASTJuliaPrinter(TextWriter tw) {
        Writer = new IndentedTextWriter(tw);
    }

    public void Print(ILoweredJLExpr expr, bool reset = true) {
        if (reset) {
            Writer.Indent = 0;
        }
        Writer.Indent = 0;
        if (expr is Block)
            IsRootBlock = true;
        ActiveState.IsVisibleBlockDecor = true;
        base.Visit(expr);
    }

    private void Write(object? o) {
        Writer.Write(o);
        ActiveState.PrintedStatement = true;
    }

    private void WriteLine(object? o) {
        Writer.WriteLine(o);
        ActiveState.PrintedStatement = true;
    }
    
    public override void VisitStatesStart(ref NodeVisitorState<ILoweredJLExpr> state) {
        switch (state.Node) {
            case Constant ck:
                switch (ck.Value) {
                    case NetRuntimeType rt:
                        Write(rt.Name);
                        break;
                    default:
                        Write(ck.Value);
                        break;
                }
                break;
            case Variable v:
                Write(v.Name);
                break;
            case Assignment asn:
                Write(asn.Variable.Name);
                Write(" ");
                if (asn.IsBinaryCompound) {
                    PushNewVisit(asn.BinaryOperator.CallSite.Values[1]);
                    Write(asn.BinaryOperator.Function.Name);
                    state.State = null;
                }
                Write("= ");
                break;
            case GetProperty:
                break;
            case SetProperty:
                //Force reverse execution order (instance then value)
                if (state.State == SetProperty.EvalValue && state.LastState != SetProperty.EvalInstance) {
                    state.State = SetProperty.EvalInstance;
                }
                break;
            case Conditional:
                Write("if ");
                ActiveState.IsVisibleBlockDecor = false;
                break;
            case BinaryOperatorInvoke:
                break;
            case FunctionInvoke fi:
                Write(fi.Function.Name);
                Write("(");
                break;
            case Block:
                if(ActiveState.IsVisibleBlockDecor)
                    WriteLine("begin");
                ActiveState.PrintedStatement = false;
                Writer.Indent++;
                state.Data = ActiveState;
                break;
            case Label lbl:
                Write("@label ");
                Write(lbl.Name);
                break;
            case Goto gt:
                Write("@goto ");
                Write(gt.Label.Name);
                break;
            case While: 
                Write("while ");
                ActiveState.IsVisibleBlockDecor = false;
                break;
            case For: 
                Write("for ");
                ActiveState.IsVisibleBlockDecor = false;
                break;
            default: throw new NotSupportedException(state.Node.GetType().Name);
        }
    }

    public override void VisitStatesEnd(ref NodeVisitorState<ILoweredJLExpr> state) {
        switch (state.Node) {
            case Constant:
            case Label:
            case Assignment:
            case Variable:
            case SetProperty:
            case GetProperty:
            case BinaryOperatorInvoke:
            case Goto:
                break;
            case Conditional:
            case For:
            case While:
                Write("end");
                break;
            case FunctionInvoke:
                Write(")");
                break;
            case Block:
                Writer.Indent--;
                ActiveState = (PrinterBlockState) state.Data!;
                if(ActiveState.IsVisibleBlockDecor)
                    WriteLine("end");
                break;
            default: throw new NotSupportedException(state.Node.GetType().Name);
        }
    }

    public override void AfterStateVisit(ref NodeVisitorState<ILoweredJLExpr> state) {
        switch (state.Node) {
            case Constant:
            case Label:
            case Assignment:
            case Variable:
                break;
            case GetProperty gp:
                Write(".");
                Write(gp.Name);
                break;
            case SetProperty sp:
                if (state.LastState == SetProperty.EvalInstance) {
                    Write(".");
                    Write(sp.Name);
                    Write(" = ");
                    state.State = SetProperty.EvalValue;
                }
                else {
                    state.State = null;
                }
                break;
            case Conditional:
            case Goto:
            case For:
            case While:
                break;
            
            case BinaryOperatorInvoke boi:
                if (state.LastState == 0) {
                    Write(" ");
                    Write(boi.Function.Name);
                    Write(" ");
                }
                break;
            case Block:
                if(ActiveState.PrintedStatement)
                    Writer.WriteLine();
                ActiveState.PrintedStatement = false;
                break;
            case FunctionInvoke fi:
                if(state.State < fi.CallSite.Values.Count)
                    Write(", ");
                else if (fi.CallSite.KeyArgNames.Count > 0) {
                    var kvs = state.State - fi.CallSite.Values.Count;
                    if (kvs == 0)
                        Write(";");
                    else if (state.State < fi.CallSite.Values.Count)
                        Write(", ");
                }
                break;
            default: throw new NotSupportedException(state.Node.GetType().Name);
        }
    }
}

public static class ASTUtils {
    public static void PrintJuliaString(this ILoweredJLExpr expr, TextWriter? writer = null) {
        writer ??= Console.Out;
        var visitor = new LoweredASTJuliaPrinter(writer) {
            ActiveState = new(){PrintStatementTypes = true}
        };
        visitor.Print(expr);
    }
    
}

