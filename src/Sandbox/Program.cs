using System.CodeDom.Compiler;
using IronJulia.AST;
using IronJulia.CoreLib;
using IronJulia.CoreLib.Interop;
using IronJulia.Parse;
using static IronJulia.AST.LoweredJLExpr;

//RangeTests();
//ExprTests();
//NativeArrayTests();
//JuliaMDArrayTests();
ExprParsing();

static void RangeTests() {
    Console.WriteLine("==== Range Tests ====");
    
    Console.WriteLine("println(1:10)");
    var ur = new Core.UnitRange<Base.Int>(1, 10);
    Console.WriteLine(string.Join(", ", ur));
}

/*
 ==== Matrix{Int} Tests ====
  Array{Int, 2}[4, 0, 0, 0, 0, 0, 8, 0, 0, 0, 0, 0, 12, 0, 0, 0, 0, 0, 16, 0, 0, 0, 0, 0, 20]
 */
static void JuliaMDArrayTests() {
    Console.WriteLine("==== Matrix{Int} Tests ====");
    Core.Array<Base.Int, Vals.Int2, (Base.Int, Base.Int)> mat = new((5, 5));
    for (var i = 1; i <= 5; i++) {
        mat[(i, i)] = i * 4;
    }
    Console.WriteLine(mat);
}

/*
==== Vector{System.Int} Tests ====
[System.Int32(i) for i in 1:10]
0
Array{Int32, 1}[2, 4, 6, 8, 10, 12, 14, 16, 18, 20]

Treating it as a dynamic object...
Array{Int32, 1}[2, 4, 6, 8, 10, 12, 14, 16, 18, 20, 20]
 */

static void NativeArrayTests() {
    Console.WriteLine("==== Vector{System.Int} Tests ====");
    
    Console.WriteLine("[System.Int32(i) for i in 1:10]");
    var arr = jlapi.jl_alloc_array_1d<int>(10);
    for (var i = 1; i <= arr.Length; i++)
        arr[i] = i * 2;
    Console.WriteLine(arr);
    Console.WriteLine();

    Console.WriteLine("Treating it as a dynamic object...");
    dynamic darr = arr;
    jlapi.jl_array_ptr_1d_push(darr, (int) 20);
    Console.WriteLine(arr);
    Console.WriteLine();
    
    Console.WriteLine();
    Console.WriteLine();
}

/*
==== EXPR TEST ====
begin
    j = 1
    @label top
    i = 0
    while i < 5
        Test(i, 3, j)
        i += 1
    end
    if j == 1
        j = 2
        @goto top
    end
    j
end
0 + 3 is 3. j=1
1 + 3 is 4. j=1
2 + 3 is 5. j=1
3 + 3 is 6. j=1
4 + 3 is 7. j=1
0 + 3 is 3. j=2
1 + 3 is 4. j=2
2 + 3 is 5. j=2
3 + 3 is 6. j=2
4 + 3 is 7. j=2
2
*/
static void ExprTests() {
    Console.WriteLine("==== EXPR TEST ====");
    //Expose the Net Metadata as a Julia Module
    var cMod = NetType.GetOrCreateModuleForType(typeof(MyClass));
    var blk = Block.Create();

/*
j = 1

@label top
i = 0

while i < 5
    PrintAdd(i, 3, j)
    i += 1
end

if j == 1
   j = 2
   @goto top
end

MyBasicFieldExample = 5;       //setproperty!(MyClass, MyBasicFieldExample)
println(MyBasicFieldExample)   
j
*/

    var i = blk.CreateVariable(typeof(Base.Int), "i");
    var j = blk.CreateVariable(typeof(Base.Int), "j");
    var top = blk.CreateLabel("top");
    
    blk.Append(Assignment.Create(j, Constant.Create(new Base.Int(1))));  //j = 1
    blk.Append(top);  //@label top
    blk.Append(Assignment.Create(i, Constant.Create(new Base.Int(0))));  //i = 0
    
    var loop = blk.CreateWhile();
  
    loop.Condition.Append(
        BinaryOperatorInvoke.Create(Base.op_LessThan, 
        i, Constant.Create(new Base.Int(5))));  //i < 5

    loop.Body.Append(FunctionInvoke.Create((Core.Function) cMod.getglobal("Test")!, 
         NewCallsite(i, Constant.Create(new Base.Int(3)), j)));   //Test(i, 3, j)

    loop.Body.Append(Assignment.Create(i, 
        BinaryOperatorInvoke.Create(Base.op_Add, 
            i, Constant.Create(new Base.Int(1))), true));  //i += 1
    
    blk.Append(loop);
    
    var ifStmt = blk.CreateConditional();
    ifStmt.Condition!.Append(BinaryOperatorInvoke.Create(Base.op_Equality, 
        j, Constant.Create(new Base.Int(1))));   //j == 1
    ifStmt.Body.Append(Assignment.Create(j, Constant.Create(new Base.Int(2))));  //j = 2
    ifStmt.Body.Append(Goto.Create(top)); 
    
    blk.Append(ifStmt);
    blk.Append(SetProperty.Create(Constant.Create(cMod), "MyBasicBindingExample", Constant.Create(new Base.Int(5)))); //MyClass.MyBasicBindingExample = 5
    var gp = GetProperty.Create(Constant.Create(cMod), "MyBasicBindingExample");
    blk.Append(FunctionInvoke.Create(Base.println, NewCallsite(gp)));  //println(MyClass.MyBasicBindingExample)
    blk.Append(j);
    
    blk.PrintJuliaString();

    var itp = new LoweredASTInterpreter();
    Console.WriteLine(itp.Interpret(blk, true));
    Console.WriteLine();
    Console.WriteLine();
}

/*
Julia Expr Output (post remove_linenums! and macro expanding):
 Expr
  head: Symbol block
  args: Array{Any}((8,))
    1: Expr
      head: Symbol =
      args: Array{Any}((2,))
        1: Symbol j
        2: Int64 1
    2: Expr
      head: Symbol symboliclabel
      args: Array{Any}((1,))
        1: Symbol top
    3: Expr
      head: Symbol =
      args: Array{Any}((2,))
        1: Symbol i
        2: Int64 0
    4: Expr
      head: Symbol while
      args: Array{Any}((2,))
        1: Expr
          head: Symbol call
          args: Array{Any}((3,))
            1: Symbol <
            2: Symbol i
            3: Int64 5
        2: Expr
          head: Symbol block
          args: Array{Any}((2,))
            1: Expr
              head: Symbol call
              args: Array{Any}((4,))
                1: Symbol PrintAdd
                2: Symbol i
                3: Int64 3
                4: Symbol j
            2: Expr
              head: Symbol +=
              args: Array{Any}((2,))
                1: Symbol i
                2: Int64 1
    5: Expr
      head: Symbol if
      args: Array{Any}((2,))
        1: Expr
          head: Symbol call
          args: Array{Any}((3,))
            1: Symbol ==
            2: Symbol j
            3: Int64 1
        2: Expr
          head: Symbol block
          args: Array{Any}((2,))
            1: Expr
              head: Symbol =
              args: Array{Any}((2,))
                1: Symbol j
                2: Int64 2
            2: Expr
              head: Symbol symbolicgoto
              args: Array{Any}((1,))
                1: Symbol top
    6: Expr
      head: Symbol =
      args: Array{Any}((2,))
        1: Symbol MyBasicFieldExample
        2: Int64 5
    7: Expr
      head: Symbol call
      args: Array{Any}((2,))
        1: Symbol println
        2: Symbol MyBasicFieldExample
    8: Symbol j
        
Iron Julia After Remove Line nums and macro expand:
Expr
    Head: Symbol block
    Args Length: 0
        1: Expr
            Head: Symbol =
            Args Length: 0
                1: Symbol j
                2: Int 1
        2: Expr
            Head: Symbol symboliclabel
            Args Length: 0
                1: Symbol top
        3: Expr
            Head: Symbol =
            Args Length: 0
                1: Symbol i
                2: Int 0
        4: Expr
            Head: Symbol while
            Args Length: 0
                1: Expr
                    Head: Symbol call
                    Args Length: 0
                        1: Symbol <
                        2: Symbol i
                        3: Int 5
                2: Expr
                    Head: Symbol block
                    Args Length: 0
                        1: Expr
                            Head: Symbol call
                            Args Length: 0
                                1: Symbol PrintAdd
                                2: Symbol i
                                3: Int 3
                                4: Symbol j
                        2: Expr
                            Head: Symbol =
                            Args Length: 0
                                1: Symbol i
                                2: Int 1
        5: Expr
            Head: Symbol if
            Args Length: 0
                1: Expr
                    Head: Symbol call
                    Args Length: 0
                        1: Symbol ==
                        2: Symbol j
                        3: Int 1
                2: Expr
                    Head: Symbol block
                    Args Length: 0
                        1: Expr
                            Head: Symbol =
                            Args Length: 0
                                1: Symbol j
                                2: Int 2
                        2: Expr
                            Head: Symbol symbolicgoto
                            Args Length: 0
                                1: Symbol top
        6: Expr
            Head: Symbol =
            Args Length: 0
                1: Symbol MyBasicFieldExample
                2: Int 5
        7: Expr
            Head: Symbol call
            Args Length: 0
                1: Symbol println
                2: Symbol MyBasicFieldExample
        8: Symbol j
        
 */

static void ExprParsing() {
    var expr = """
               j = 1
               
               @label top
               i = 0
               
               while i < 5
                   PrintAdd(i, 3, j)
                   i += 1
               end
               
               if j == 1
                  j = 2
                  @goto top
               end
               
               MyBasicFieldExample = 5;                # setproperty!(MyClass, MyBasicFieldExample)
               println(MyBasicFieldExample)   
               j
               """;

    var jp = new JuliaExprParser();
    var jexp = (Base.Expr)jp.Parse(expr);
    jexp = (Base.Expr) Base.Meta.MacroExpand1(Base.m_Base, jexp);
    jexp.Print(new IndentedTextWriter(Console.Out));
}

public class MyClass {
    public static Base.Int MyBasicBindingExample = 3;
    public static void Test(Base.Int a, Base.Int b, Base.Int j) {
        Console.WriteLine($"{a} + {b} is {a + b}. j={j}");
    }
}
