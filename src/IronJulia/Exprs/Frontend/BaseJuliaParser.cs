using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace IronJulia.Parse;

public abstract class BaseJuliaParser(ITokenStream input, TextWriter output, TextWriter errorOutput)
    : Parser(input, output, errorOutput) {
    public JuliaLexer Lexer { get; } = (JuliaLexer) ((CommonTokenStream) input).TokenSource;

    protected void SetInput(BaseInputCharStream stream) {
        Lexer.SetInputStream(stream);
        Lexer.Reset();
        Reset();
    }
    
    public static void PrintSyntaxTree(TextWriter tw, Parser parser, IParseTree root) {
        Recursive(root, tw, 0, parser.RuleNames);
    }
    
    private static void Recursive(IParseTree aRoot, TextWriter tw, int offset, string[] ruleNames) {
        for (var i = 0; i < offset; i++)
            tw.Write("  ");
        tw.WriteLine(Trees.GetNodeText(aRoot, ruleNames));
        if (aRoot is not ParserRuleContext prc) 
            return;

        if (prc.children == null) 
            return;
        
        foreach (var child in prc.children)
            Recursive(child, tw, offset + 1, ruleNames);
    }

    public static void PrintToken(IToken t) => Console.WriteLine(JuliaLexer.DefaultVocabulary.GetDisplayName(t.Type) + ":" + t.Text);
}