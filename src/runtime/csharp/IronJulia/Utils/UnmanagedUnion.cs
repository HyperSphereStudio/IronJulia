using System.Runtime.InteropServices;

namespace SpinorCompiler.Utils;

[StructLayout(LayoutKind.Explicit)]
public struct UnmanagedUnion<A, B> where A : unmanaged where B : unmanaged {
    [FieldOffset(0)] public A Item1;
    [FieldOffset(0)] public B Item2;
};

public struct UnmanagedTagUnion<A, B> where A : unmanaged where B : unmanaged {
    private UnmanagedUnion<A, B> _union;
    public nint TypeIndex { get; private set; }
    
    public UnmanagedTagUnion(){}
    
    public A GetItem1() => _union.Item1;
    public B GetItem2() => _union.Item2;
    
    public void SetItem1(A a) {
        _union.Item1 = a;
        TypeIndex = 1;
    }
    public void SetItem2(B b) {
        _union.Item2 = b;
        TypeIndex = 2;
    }

    public Type? Type {
        get {
            return TypeIndex switch {
                1 => typeof(A),
                2 => typeof(B),
                _ => null
            };
        }
    }
};

[StructLayout(LayoutKind.Explicit)]
public struct UnmanagedUnion<A, B, C> where A : unmanaged where B : unmanaged where C: unmanaged {
    [FieldOffset(0)] public A Item1;
    [FieldOffset(0)] public B Item2;
    [FieldOffset(0)] public C Item3;
};


public struct UnmanagedTagUnion<A, B, C> where A : unmanaged where B : unmanaged where C: unmanaged{
    private UnmanagedUnion<A, B, C> _union;
    public nint TypeIndex { get; private set; }
    
    public UnmanagedTagUnion(){}
    
    public A GetItem1() => _union.Item1;
    public B GetItem2() => _union.Item2;
    public C GetItem3() => _union.Item3;
    
    public void SetItem1(A a) {
        _union.Item1 = a;
        TypeIndex = 1;
    }
    public void SetItem2(B b) {
        _union.Item2 = b;
        TypeIndex = 2;
    }
    public void SetItem3(C c) {
        _union.Item3 = c;
        TypeIndex = 3;
    }

    public Type? Type {
        get {
            return TypeIndex switch {
                1 => typeof(A),
                2 => typeof(B),
                3 => typeof(C),
                _ => null
            };
        }
    }
};

[StructLayout(LayoutKind.Explicit)]
public struct UnmanagedUnion<A, B, C, D> where A : unmanaged where B : unmanaged where C: unmanaged where D: unmanaged {
    [FieldOffset(0)] public A Item1;
    [FieldOffset(0)] public B Item2;
    [FieldOffset(0)] public C Item3;
    [FieldOffset(0)] public D Item4;
};

public struct UnmanagedTagUnion<A, B, C, D> where A : unmanaged where B : unmanaged where C: unmanaged where D: unmanaged{
    private UnmanagedUnion<A, B, C, D> _union;
    public nint TypeIndex { get; private set; }
    
    public UnmanagedTagUnion(){}
    
    public void SetItem1(A a) {
        _union.Item1 = a;
        TypeIndex = 1;
    }
    public void SetItem2(B b) {
        _union.Item2 = b;
        TypeIndex = 2;
    }
    public void SetItem3(C c) {
        _union.Item3 = c;
        TypeIndex = 3;
    }
    public void SetItem4(D d) {
        _union.Item4 = d;
        TypeIndex = 4;
    }
    
    public A GetItem1() => _union.Item1;
    public B GetItem2() => _union.Item2;
    public C GetItem3() => _union.Item3;
    public D GetItem4() => _union.Item4;

    public Type? Type {
        get {
            return TypeIndex switch {
                1 => typeof(A),
                2 => typeof(B),
                3 => typeof(C),
                4 => typeof(D),
                _ => null
            };
        }
    }
};