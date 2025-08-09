using System.CodeDom.Compiler;
using IronJulia.CoreLib;
using IronJulia.Parse;

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
