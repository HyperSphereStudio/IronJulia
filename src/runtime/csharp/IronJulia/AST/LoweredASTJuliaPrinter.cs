using System.CodeDom.Compiler;

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
    
    protected Dictionary<Variable, Base.Symbol> _activeVariables = new();
    
    public LoweredASTJuliaPrinter(TextWriter tw) {
        Writer = new IndentedTextWriter(tw);
    }

    public void Print(ILoweredJLExpr expr, bool reset = true) {
        if (reset) {
            _activeVariables.Clear();
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
    
    public override void VisitStatesStart(ILoweredJLExpr expr, ref object? data, ref uint? nextState) {
        switch (expr) {
            case BinaryOperatorInvoke:
                break;
            case Constant ck:
                Write(ck.Value);
                break;
            case Label lbl:
                Write("@label ");
                Write(lbl.Name);
                break;
            case Goto gt:
                Write("@goto ");
                Write(gt.Label.Name);
                break;
            case Block:
                if(ActiveState.IsVisibleBlockDecor)
                    WriteLine("begin");
                ActiveState.PrintedStatement = false;
                Writer.Indent++;
                data = ActiveState;
                break;
            case Conditional:
                Write("if ");
                ActiveState.IsVisibleBlockDecor = false;
                break;
            case Assignment asn:
                Write(asn.Variable.Name);
                Write(" ");
                if (asn.IsBinaryCompound) {
                    PushNewVisit(asn.BinaryOperator.Arguments[1]);
                    Write(asn.BinaryOperator.Function.Name);
                    Write("= ");
                    nextState = null;
                    return;
                }
                Write("= ");
                break;
            case FunctionInvoke fi:
                Write(fi.Function.Name);
                Write("(");
                break;
            case While: 
                Write("while ");
                ActiveState.IsVisibleBlockDecor = false;
                break;
            case Variable v:
                var name = v.Name;
                if (!name.HasValue && !_activeVariables.TryGetValue(v, out var nv)) {
                    name = "var" + _activeVariables.Count;
                    _activeVariables[v] = name.Value;
                }
                Write(name);
                break;
            default: throw new NotSupportedException(expr.GetType().Name);
        }
    }

    public override void VisitStatesEnd(ILoweredJLExpr expr, ref object? data) {
        switch (expr) {
            case Constant:
            case Label:
            case Assignment:
            case Variable:
            case BinaryOperatorInvoke:
            case Goto:
                break;
            case Conditional:
            case While:
                Write("end");
                break;
            case FunctionInvoke:
                Write(")");
                break;
            case Block bk:
                Writer.Indent--;
                ActiveState = (PrinterBlockState) data!;
                if(ActiveState.IsVisibleBlockDecor)
                    WriteLine("end");
                foreach(var v in bk.Variables)
                    _activeVariables.Remove(v);
                break;
            default: throw new NotSupportedException(expr.GetType().Name);
        }
    }

    public override void AfterStateVisit(ILoweredJLExpr expr, ref object? data, uint? state, ref uint? nextState) {
        switch (expr) {
            case Constant:
            case Label:
            case Assignment:
            case Variable:
            case Conditional:
            case Goto:
            case While:
                break;
            case BinaryOperatorInvoke boi:
                if (state == 0) {
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
                if(state+1 < fi.Arguments.Length)
                    Write(", ");
                break;
            default: throw new NotSupportedException(expr.GetType().Name);
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

