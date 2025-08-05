lexer grammar JuliaLexer;

options { language=CSharp; superClass=BaseJuliaLexer; }

StringStartEnd: '"';

LinOp: [+-];
ProdOp: [*/];
PowOp: [^];
CondOp: '==' | '!=' | '<' | '>' | '<=' | '>=';

While: 'while';
For: 'for';
Quote: 'quote';
Begin: 'begin';
If: 'if';
ElseIf: 'elseif';
Else: 'else';
Cond: '?';
Dot: '.';
Comma: ',';
Assign: '=';
Mutable: 'mutable';
Abstract: 'abstract';
ElementOf: '::';
Import: 'import';
Using: 'using';
Struct: 'struct';
StmtTerminator: (NewLine | ';')+;
End: 'end';
MacroName: '@' Name;
NewLine: ('\r'? '\n' | '\r');
Skip: (Whitespace | MultiLineComment | LineComment) -> skip;
RPar: '(';
LPar: ')';
MultiLineComment: '#=' .*? '=#';
LineComment: '#' ~[\r\n]* NewLine;

Name: ID_START ID_CONTINUE*;
Decimal_Int: NON_ZERO_DIGIT DIGIT* | '0'+;

fragment NON_ZERO_DIGIT: [1-9];
fragment DIGIT: [0-9];
fragment OCT_DIGIT: [0-7];
fragment HEX_DIGIT: [0-9a-fA-F];
fragment BIN_DIGIT: [01];
fragment Whitespace: [ \t]+;
fragment ID_START: [a-zA-Z];
fragment ID_CONTINUE: [a-zA-Z0-9];
