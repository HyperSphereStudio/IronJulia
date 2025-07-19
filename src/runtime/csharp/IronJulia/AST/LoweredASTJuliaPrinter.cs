using System.CodeDom.Compiler;

namespace IronJulia.AST;

using static LoweredJLExpr;

public class LoweredASTJuliaPrinter : NonRecursiveGraphVisitor<ILoweredJLExpr>
{
    public IndentedTextWriter Writer;
    public bool PrintStatementTypes, IsVisibleBlockDecor;
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
        IsVisibleBlockDecor = true;
        base.Visit(expr);
    }
    
    public override bool VisitStatesStart(ILoweredJLExpr expr, ref object? data) {
        switch (expr) {
            case Constant ck:
                Writer.Write(ck.Value);
                break;
            case Label lbl:
                Writer.Write("@label ");
                Writer.Write(lbl.Name);
                break;
            case Goto gt:
                Writer.Write("@goto ");
                Writer.Write(gt.Label.Name);
                break;
            case Block:
                if(IsVisibleBlockDecor)
                    Writer.WriteLine("begin");
                Writer.Indent++;
                data = IsVisibleBlockDecor;
                break;
            case Assignment asn:
                Writer.Write(asn.Variable.Name);
                Writer.Write(" ");
                if (asn.IsBinaryCompound) {
                    PushNewVisit(asn.BinaryOperator.Arguments[1]);
                    Writer.Write(asn.BinaryOperator.Function.Name);
                    Writer.Write("= ");
                    return false;
                }
                Writer.Write("= ");
                break;
            case FunctionInvoke fi:
                Writer.Write(fi.Function.Name);
                Writer.Write("(");
                break;
            case While: 
                Writer.Write("while ");
                IsVisibleBlockDecor = false;
                break;
            case Variable v:
                var name = v.Name;
                if (!name.HasValue && !_activeVariables.TryGetValue(v, out var nv)) {
                    name = "var" + _activeVariables.Count;
                    _activeVariables[v] = name.Value;
                }
                Writer.Write(name);
                break;
            default: throw new NotSupportedException(expr.GetType().Name);
        }
        return true;
    }

    public override void VisitStatesEnd(ILoweredJLExpr expr, ref object? data) {
        switch (expr) {
            case Constant:
            case Label:
            case Assignment:
            case Variable:
            case Goto:
                break;
            case While:
                Writer.Write("end");
                break;
            case FunctionInvoke:
                Writer.Write(")");
                break;
            case Block bk:
                Writer.Indent--;
                IsVisibleBlockDecor = (bool) data!;
                if(IsVisibleBlockDecor)
                    Writer.WriteLine("end");
                foreach(var v in bk.Variables)
                    _activeVariables.Remove(v);
                break;
            default: throw new NotSupportedException(expr.GetType().Name);
        }
    }

    public override bool AfterStateVisit(ILoweredJLExpr expr, ref object? data, uint? state) {
        switch (expr) {
            case Constant:
            case Label:
            case Assignment:
            case Variable:
            case Goto:
            case While:
                break; 
            case Block:
                Writer.WriteLine();
                break;
            case FunctionInvoke fi:
                if(state+1 < fi.Arguments.Length)
                    Writer.Write(", ");
                break;
            default: throw new NotSupportedException(expr.GetType().Name);
        }
        return true;
    }
}

public static class ASTUtils {
    public static void PrintJuliaString(this ILoweredJLExpr expr, TextWriter? writer = null) {
        writer ??= Console.Out;
        var visitor = new LoweredASTJuliaPrinter(writer) {
            PrintStatementTypes = true
        };
        visitor.Print(expr);
    }
    
}

