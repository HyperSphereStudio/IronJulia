parser grammar JuliaParser;

options { language=CSharp; superClass=BaseJuliaParser; tokenVocab=JuliaLexer; }
    
file_input
    : blocks=stmt* expr? EOF
    ;
    
stmt
    : StmtTerminator* 
      expr StmtTerminator+
    ;
    
expr
    : (name=MacroName args=expr*)              #macroExpr
    | (type=(While|For) cond=expr blockbody)   #loopExpr
    | (if elseIf* else? End)                   #condExpr    
    | a=expr broadcast=Dot? PowOp b=expr       #powBOpExpr
    | a=expr broadcast=Dot? ProdOp b=expr      #prodBOpExpr
    | a=expr broadcast=Dot? LinOp b=expr       #linBOpExpr
    | a=expr broadcast=Dot? CondOp b=expr      #condBOpExpr  
    | key=assignable broadcast=Dot? op=(PowOp|ProdOp|LinOp)? Assign val=expr     #assignExpr                                 
    | assignable                               #getExpr
    | (name=Name RPar (expr Comma)* expr LPar) #funInvoke
    | RPar (expr Comma)* expr LPar             #tupleExpr                                      
    | literal                                  #literalExpr     
    ;

if: If cond=expr StmtTerminator stmts=stmt*;  
elseIf: ElseIf cond=expr StmtTerminator stmts=stmt*;
else: Else StmtTerminator stmts=stmt*;

assignable
   :  (Name Dot)*   Name              #nameRef
   ; 
    
literal: 
     number;   
     
number: 
    Decimal_Int;

blockbody: StmtTerminator stmts=stmt* End;
