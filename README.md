# IronJulia

## Currently in extremly early development ... Basically nothing from the Julia STL will work :)

Implementation of the Julia Runtime into .NET 11


Some Current Tests
```

using IronJulia.AST;
using IronJulia.CoreLib;
using IronJulia.CoreLib.Interop;
using static IronJulia.AST.LoweredJLExpr;

ExprTests();
NativeArrayTests();
JuliaMDArrayTests();


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
=====Output======
begin
    i = 0
    while LessThan(i, 5)
        PrintAdd(i, 3)
        i += 1
    end
end
0 + 3 is 3
1 + 3 is 4
2 + 3 is 5
3 + 3 is 6
4 + 3 is 7

 */
static void ExprTests() {
    Console.WriteLine("==== EXPR TEST ====");
    //Expose the Net Metadata as a Julia Module
    var cMod = (Core.Module) new NetRuntimeType(typeof(MyClass), null);

    var blk = Block.CreateRootBlock();

/*
i = 0
while i < 5
    Test(i, 3);
    i += 1
*/

    var i = blk.CreateVariable(typeof(Base.Int), "i");
    blk.Statements.Add(Assignment.Create(i, Constant.Create(new Base.Int(0))));
    var loop = blk.CreateWhile();

    loop.Conditional.Statements.Add(FunctionInvoke.Create((Core.Function) cMod.getglobal("LessThan")!, 
        [i, Constant.Create(new Base.Int(5))]));  //LessThan(i, 5);

    loop.Body.Statements.Add(FunctionInvoke.Create((Core.Function) cMod.getglobal("PrintAdd")!, 
        [i, Constant.Create(new Base.Int(3))]));   //PrintAdd(i, 3)

    loop.Body.Statements.Add(Assignment.Create(i, 
        BinaryOperatorInvoke.Create(Base.op_Add, 
            i, Constant.Create(new Base.Int(1))), true));  //i += 1

    blk.Statements.Add(loop);

    blk.PrintJuliaString();
    
    new LoweredASTInterpreter().Interpret(blk, true);
    
    Console.WriteLine();
    Console.WriteLine();
}


public class MyClass {
    public static void PrintAdd(Base.Int a, Base.Int b) {
        Console.WriteLine($"{a} + {b} is {a + b}");
    }

    public static Base.Bool LessThan(Base.Int a, Base.Int b) {
        return a < b;
    }
    
}

```
