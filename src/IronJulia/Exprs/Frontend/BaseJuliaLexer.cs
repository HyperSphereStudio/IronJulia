using Antlr4.Runtime;

namespace IronJulia.Parse;

public abstract class BaseJuliaLexer(ICharStream input, TextWriter output, TextWriter errorOutput)
    : Lexer(input, output, errorOutput) {
    
}