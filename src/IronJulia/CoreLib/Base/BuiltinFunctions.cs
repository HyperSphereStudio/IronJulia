using System.Reflection;
using IronJulia.AST;
using IronJulia.CoreLib;
using IronJulia.CoreLib.Interop;

public partial struct Base {
    public static readonly JulianRuntimeModule m_Base = new("Base", null);
    public static readonly Core.Function op_Add = new("+", m_Base);
    public static readonly Core.Function op_Sub = new("-", m_Base);
    public static readonly Core.Function op_Mul = new("*", m_Base);
    public static readonly Core.Function op_Div = new("/", m_Base);
    public static readonly Core.Function op_LessThan = new("<", m_Base);
    public static readonly Core.Function op_GreaterThan = new(">", m_Base);
    public static readonly Core.Function op_LessThanOrEqual = new("<=", m_Base);
    public static readonly Core.Function op_GreaterThanOrEqual = new(">=", m_Base);
    public static readonly Core.Function op_Equality = new("==", m_Base);
    public static readonly Core.Function op_InEquality = new("!=", m_Base);
    public static readonly Core.Function convert = new("convert", m_Base);
    public static readonly Core.Function println = new("println", m_Base);
    public static readonly Core.Function iterate = new("iterate", m_Base);
    public static readonly Core.Function getproperty = new("getproperty", m_Base);
    public static readonly Core.Function setproperty = new("setproperty!", m_Base);
    public static readonly Core.Function getfield = new("getfield", m_Base);
    public static readonly Core.Function setfield = new("setfield!", m_Base);
    
    public static bool StrictCompare<T1, T2>(T1? a, T2? b) {
        return typeof(T1) == typeof(T2) && (a?.Equals(b) ?? false);
    }

    public static Expr @goto(Symbol label) => new(CommonSymbols.goto_sym, label);
    public static Expr @label(Symbol label) => new(CommonSymbols.label_sym, label);
    
    static Base() {
        foreach (var ty in typeof(Base).GetNestedTypes()) {
            m_Base.AddBinding(ty.Name, new NetRuntimeType(ty, m_Base));
        }
        m_Base.AddBinding("println", println);
        println.Methods.Add(new NetMethod(println, typeof(Console).GetMethod("WriteLine", BindingFlags.Public|BindingFlags.Static, [typeof(object)])!));

        var @goto = new Core.Function("@goto", m_Base);
        @goto.Methods.Add(new NetMethod(@goto, typeof(Base).GetMethod("goto", BindingFlags.Public|BindingFlags.Static)!));
        m_Base.AddBinding("@goto", @goto);
        
        var @label = new Core.Function("@label", m_Base);
        @label.Methods.Add(new NetMethod(@label, typeof(Base).GetMethod("label", BindingFlags.Public|BindingFlags.Static)!));
        m_Base.AddBinding("@label", @label);
    }
}

