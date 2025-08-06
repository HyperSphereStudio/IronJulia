using System.Collections;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using IronJulia.AST;

namespace IronJulia.Parse;

public class JuliaExprParser : JuliaParser, IAntlrErrorListener<IToken>, IAntlrErrorListener<int>, IJuliaParserListener{
    private string? _file;
    private bool _createLineNumberNodes = true;
    public const int DisplayLineCharWidth = 40;
    private readonly ParseTreeProperty<object> _vals = new();
    private readonly ParseTreeWalker _wk = new();
    
    public JuliaExprParser() : base(new CommonTokenStream(new JuliaLexer(null))) {
        base.RemoveErrorListeners();
        base.AddErrorListener(this);
        Lexer.RemoveErrorListeners();
        Lexer.AddErrorListener(this);
    }

    public object Parse(string s, string? file = null, bool createLineNumberNodes = true) =>
        Parse(new AntlrInputStream(s), file, createLineNumberNodes);

    public object Parse(FileInfo file, bool createLineNumberNodes = true) =>
        Parse(new AntlrFileStream(file.FullName), file.FullName, createLineNumberNodes);

    public object Parse(BaseInputCharStream stream, string? file = null, bool createLineNumberNodes = true) {
        SetInput(stream);
        _file = file;
        _createLineNumberNodes = createLineNumberNodes;
        Debug();
        var fi = file_input();
        _wk.Walk(this, fi);
        return _vals.Get(fi);
    }
    
    #region Debugging
    private void ParserException(string msg, int line, int charPosInLine) {
        var ts = TokenStream;
        if (charPosInLine == -1)
            charPosInLine = ts.TokenSource.Column;
        var tc = ts.TokenSource.InputStream;

        //Trim Front To Single Line
        var start = tc.Index;
        for (var i = 1; i < start; i++) {
            var c = tc.LA(-i);
            if (c != '\n' && c != '\r') 
                continue;
            start -= i;
            break;
        }
        
        //Trim Rear To Single Line
        var end = tc.Index;
        for (var i = 1; i < start; i++) {
            var c = tc.LA(i);
            if (c != '\n' && c != '\r') 
                continue;
            end += i;
            break;
        }
        
        end = Math.Min(end, start + DisplayLineCharWidth);
        var textInterval = new Interval(start + 1, end - 1);
        var lineWindow = textInterval.Length > 0 ? tc.GetText(textInterval) : "";
        var errorLink = _file == null ? "" : new Uri(Path.GetFullPath(_file)).ToString();
        
        //Create Arrows of Offending Token Location
        var errorBars = new string(' ', charPosInLine) + '^';

        throw new Exception(string.Format("Julia Parsing Error:\t{0}\n:> {1} {2}:{3}\n\n{4}\n\t{5}", 
            msg, errorLink, line, charPosInLine, lineWindow, errorBars));
    }
    public void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line,
        int charPositionInLine, string msg, RecognitionException e) => ParserException(msg, line, charPositionInLine);

    public void SyntaxError(TextWriter output, IRecognizer recognizer, int offendingSymbol, int line,
        int charPositionInLine, string msg, RecognitionException e) => ParserException(msg, line, charPositionInLine);

    private void Debug() {
        DebugLexer();
        DebugParser();
    }

    private void DebugLexer() {
        try {
            PrintToken(Lexer.Token);
            for (var t = Lexer.NextToken(); t.Type != -1; t = Lexer.NextToken())
                PrintToken(t);
            Lexer.Reset();
            Lexer.NextToken();
        }
        catch (Exception) {
            Console.Error.WriteLine("\n\n\n##LEXER CRASH##");
            Console.Write("Lexer Modes:");
            Console.WriteLine(string.Join(", ",  Lexer.ModeStack.Select(x => Lexer.ModeNames[x])));
            Console.WriteLine($"Current:{Lexer.ModeNames[Lexer.CurrentMode]}");
            throw;
        }
    }

    private void DebugParser() {
        PrintSyntaxTree(Console.Out, this, file_input());
        Reset();
    }
    
    #endregion
    
    
    
    private object Get(IParseTree n) => _vals.Get(n);
    private void Put(IParseTree n, object v) => _vals.Put(n, v);

    public void VisitTerminal(ITerminalNode node) {}
    public void VisitErrorNode(IErrorNode node) {}
    public void EnterEveryRule(ParserRuleContext ctx) {}

    #region Enter
    public void EnterFile_input(File_inputContext ctx) {}
    public void EnterStmt(StmtContext ctx) {}
    public void EnterNumber(NumberContext ctx) {}
    public void EnterIf(IfContext ctx){}
    public void EnterElseIf(ElseIfContext ctx) {}
    public void EnterElse(ElseContext ctx) {}
    public void EnterNameRef(NameRefContext ctx) {}
    public void EnterLoopExpr(LoopExprContext ctx) {}
    public void EnterFunInvoke(FunInvokeContext ctx){}
    public void EnterLiteralExpr(LiteralExprContext ctx){}
    public void EnterGetExpr(GetExprContext ctx){}
    public void EnterPowBOpExpr(PowBOpExprContext ctx) {}
    public void EnterMacroExpr(MacroExprContext ctx){}
    public void EnterTupleExpr(TupleExprContext ctx){}
    public void EnterCondExpr(CondExprContext ctx){}
    public void EnterCondBOpExpr(CondBOpExprContext ctx){}
    public void EnterProdBOpExpr(ProdBOpExprContext ctx){}
    public void EnterLinBOpExpr(LinBOpExprContext ctx){}
    public void EnterAssignExpr(AssignExprContext ctx){}
    public void EnterBlockbody(BlockbodyContext ctx){}
    public void EnterLiteral(LiteralContext ctx){}
    #endregion
    
    #region Exit

    public void ExitLiteralExpr(LiteralExprContext ctx) => Put(ctx, Get(ctx.literal()));
    public void ExitEveryRule(ParserRuleContext ctx) {}

    public void ExitFile_input(File_inputContext ctx) {
        var blk = new Base.Expr(CommonSymbols.block_sym);
        foreach (var k in IterateChildrenOfType<StmtContext>(ctx)) {
            blk.args.Add(Get(k));
        }
        blk.args.Add(Get(ctx.expr()));
        Put(ctx, blk);
    }

    public void ExitStmt(StmtContext ctx)
    {
        Put(ctx, Get(ctx.expr()));
    }

    public void ExitFunInvoke(FunInvokeContext ctx) {
        var f = new Base.Expr(CommonSymbols.call_sym, (Base.Symbol) ctx.name.Text);
        foreach(var expr in IterateChildrenOfType<ExprContext>(ctx))
            f.args.Add(Get(expr));
        Put(ctx, f);
    }
    
    public void ExitLoopExpr(LoopExprContext ctx) {
        var loop_head = ctx.type.Type == While ? CommonSymbols.while_sym : CommonSymbols.for_sym;
        var loop_cond = Get(ctx.cond);
        var loop_body = Get(ctx.blockbody());
        Put(ctx, new Base.Expr(loop_head, loop_cond, loop_body));
    }
    
    public void ExitMacroExpr(MacroExprContext ctx) {
        var macroName = (Base.Symbol) ctx.name.Text;
        var mac = new Base.Expr(CommonSymbols.macrocall_sym, macroName);
        foreach (var k in ctx.args.children)
            mac.args.Add(Get(k));
        Put(ctx, mac);
    }
    
    public void ExitTupleExpr(TupleExprContext ctx) {
        var t = new Base.Expr(CommonSymbols.tuple_sym);
        foreach(var a in IterateChildrenOfType<ExprContext>(ctx))
            t.args.Add(Get(a));
        Put(ctx, t);
    }
    
    public void ExitPowBOpExpr(PowBOpExprContext ctx) {
        Put(ctx, new Base.Expr(CommonSymbols.call_sym, (Base.Symbol)ctx.PowOp().GetText(), Get(ctx.a), Get(ctx.b)));
    }

    public void ExitProdBOpExpr(ProdBOpExprContext ctx) {
        Put(ctx, new Base.Expr(CommonSymbols.call_sym, (Base.Symbol)ctx.ProdOp().GetText(), Get(ctx.a), Get(ctx.b)));
    }

    public void ExitLinBOpExpr(LinBOpExprContext ctx) {
        Put(ctx, new Base.Expr(CommonSymbols.call_sym, (Base.Symbol)ctx.LinOp().GetText(), Get(ctx.a), Get(ctx.b)));
    }
    
    public void ExitCondBOpExpr(CondBOpExprContext ctx) {
        Put(ctx, new Base.Expr(CommonSymbols.call_sym, (Base.Symbol)ctx.CondOp().GetText(), Get(ctx.a), Get(ctx.b)));
    }

    public void ExitAssignExpr(AssignExprContext ctx) {
        var op = "=";
        if(ctx.broadcast != null)
            op = ctx.broadcast.Text + op;
        Put(ctx, new Base.Expr((Base.Symbol) op, Get(ctx.key), Get(ctx.val)));
    }

    public void ExitGetExpr(GetExprContext ctx) => Put(ctx, Get(ctx.assignable()));

    public void ExitIf(IfContext ctx) {
        var blk = new Base.Expr(CommonSymbols.block_sym);
        foreach (var s in IterateChildrenOfType<StmtContext>(ctx))
            blk.args.Add(Get(s));
        Put(ctx, new Base.Expr(CommonSymbols.if_sym, Get(ctx.cond), blk));
    }

    public void ExitElseIf(ElseIfContext ctx)
    {
        var blk = new Base.Expr(CommonSymbols.block_sym);
        foreach (var s in IterateChildrenOfType<StmtContext>(ctx))
            blk.args.Add(Get(s));
        Put(ctx, new Base.Expr(CommonSymbols.elseif_sym, Get(ctx.cond), blk));
    }

    public void ExitElse(ElseContext ctx) {
        var blk = new Base.Expr(CommonSymbols.block_sym);
        foreach (var s in IterateChildrenOfType<StmtContext>(ctx))
            blk.args.Add(Get(s));
        Put(ctx, new Base.Expr(CommonSymbols.else_sym, blk));
    }

    public void ExitCondExpr(CondExprContext ctx) {
        var blk = (Base.Expr) Get(ctx.@if());
        foreach (var s in IterateChildrenOfType<ElseIfContext>(ctx)) {
            blk.args.Add(Get(s));
            blk = (Base.Expr) Get(s);
        }
        if(ctx.@else() != null)
            blk.args.Add(Get(ctx.@else()));
        Put(ctx, blk);
    }

    public void ExitNameRef(NameRefContext ctx) {
        object? v = null;
        foreach (var name in IterateChildrenOfType<ITerminalNode>(ctx)) {
            var sym = (Base.Symbol) name.GetText();
            if (v == null)
                v = sym;
            else {
                v = new Base.Expr(CommonSymbols.dot_sym, v, sym);
            }
        }
        Put(ctx, v!);
    }

    public void ExitLiteral(LiteralContext ctx) => Put(ctx, Get(ctx.number()));
    
    public void ExitNumber(NumberContext ctx) {
        var c = (ITerminalNode) ctx.GetChild(0);
        if (c.Symbol.Type == Decimal_Int) {
            Put(ctx, new Base.Int(nint.Parse(c.GetText())));
        }
    }

    public void ExitBlockbody(BlockbodyContext ctx) {
        var blk = new Base.Expr(CommonSymbols.block_sym);
        foreach(var st in IterateChildrenOfType<StmtContext>(ctx))
            blk.args.Add(Get(st));
        Put(ctx, blk);
    }
    #endregion

    internal static RuleListIterator<T> IterateChildrenOfType<T>(ParserRuleContext ctx) => new(ctx);
}

internal record struct RuleListIterator<T>(ParserRuleContext Ctx) : IEnumerable<T> {
    public IEnumerator<T> GetEnumerator() {
        foreach (var c in Ctx.children) {
            if(c is T k)
                yield return k;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}