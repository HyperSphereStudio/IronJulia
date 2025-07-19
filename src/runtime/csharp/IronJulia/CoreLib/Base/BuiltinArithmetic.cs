using IronJulia.CoreLib;
using IronJulia.CoreLib.Interop;

public partial struct Base {
    public static readonly JulianRuntimeModule m_Base = new("Base", null);
    public static readonly Core.Function op_Add = new("+", m_Base);
    public static readonly Core.Function op_Sub = new("-", m_Base);
    public static readonly Core.Function op_Mul = new("*", m_Base);
    public static readonly Core.Function op_Div = new("/", m_Base);

    static Base() {
        foreach (var ty in typeof(Base).GetNestedTypes()) {
            m_Base.AddBinding(ty.Name, new NetRuntimeType(ty, m_Base));
        }
    }
}