using IronJulia.AST;
using IronJulia.CoreLib;
using IronJulia.CoreLib.Interop;
using static IronJulia.AST.LoweredJLExpr;

ExprTests();
//NativeArrayTests();
//JuliaMDArrayTests();


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
    var cMod = (Core.Module) new NetRuntimeType(typeof(MyClass), null);

    var blk = Block.CreateRootBlock();

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

j
*/

    var i = blk.CreateVariable(typeof(Base.Int), "i");
    var j = blk.CreateVariable(typeof(Base.Int), "j");
    var top = blk.CreateLabel("top");
    
    blk.Statements.Add(Assignment.Create(j, Constant.Create(new Base.Int(1))));  //j = 1
    blk.Statements.Add(top);  //@label top
    blk.Statements.Add(Assignment.Create(i, Constant.Create(new Base.Int(0))));  //i = 0
    
    var loop = blk.CreateWhile();
    
    loop.Condition.Statements.Add(
        BinaryOperatorInvoke.Create(Base.op_LessThan, 
        i, Constant.Create(new Base.Int(5))));  //i < 5

    loop.Body.Statements.Add(FunctionInvoke.Create((Core.Function) cMod.getglobal("Test")!, 
        [i, Constant.Create(new Base.Int(3)), j]));   //Test(i, 3, j)

    loop.Body.Statements.Add(Assignment.Create(i, 
        BinaryOperatorInvoke.Create(Base.op_Add, 
            i, Constant.Create(new Base.Int(1))), true));  //i += 1
    
    blk.Statements.Add(loop);
    
    var ifStmt = blk.CreateConditional();
    ifStmt.Condition!.Statements.Add(BinaryOperatorInvoke.Create(Base.op_Equality, 
        j, Constant.Create(new Base.Int(1))));   //j == 1
    ifStmt.Body.Statements.Add(Assignment.Create(j, Constant.Create(new Base.Int(2))));  //j = 2
    ifStmt.Body.Statements.Add(Goto.Create(top)); 
    
    blk.Statements.Add(ifStmt);
    blk.Statements.Add(j);

    blk.PrintJuliaString();
    
    Console.WriteLine(new LoweredASTInterpreter().Interpret(blk, true));
    Console.WriteLine();
    Console.WriteLine();
}


public class MyClass {
    public static void Test(Base.Int a, Base.Int b, Base.Int j) {
        Console.WriteLine($"{a} + {b} is {a + b}. j={j}");
    }
}
