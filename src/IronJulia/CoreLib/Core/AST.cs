using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using IronJulia.CoreLib;
using SpinorCompiler.Boot;
using SpinorCompiler.Utils;

public partial struct Base {

    public readonly struct Symbol : IAny, IEquatable<Symbol>
    {
        private static readonly Dictionary<string, Symbol> Symbols = new();
        
        private Symbol(string sym) => Value = sym;
        
        public readonly string Value;
        public override string ToString() => Value;

        public static Symbol Create(string value) {
            if(!Symbols.ContainsKey(value))
                Symbols.Add(value, new Symbol(value));
            return Symbols[value];
        }
        
        public static implicit operator Symbol(string value) => Create(value);
        public static explicit operator string(Symbol value) => value.Value;
        public bool Equals(Symbol s) => ReferenceEquals(Value, s.Value);
        public static bool operator ==(Symbol a, Symbol b) => a.Equals(b);
        public static bool operator !=(Symbol a, Symbol b) => !(a == b);
    }
    
    public class Expr : Base.IAny {
        public Symbol head;
        public Core.Array<object, Vals.Int1, ValueTuple<Int>> args;

        public Expr(Symbol head) {
            this.head = head;
            args = new(4);
        }
        
        public Expr(Symbol head, params Core.Array<object, Vals.Int1, ValueTuple<Base.Int>> args) {
            this.head = head;
            this.args = args;
        }

        public void Print(IndentedTextWriter tw) {
            tw.WriteLine("Expr ");
            tw.Indent++;
            tw.Write("Head: Symbol ");
            tw.WriteLine(head);
            tw.Write("Args Length: ");
            tw.WriteLine(args.size);
            tw.Indent++;
            var idx = 1;
            foreach (var arg in args) {
                tw.Write(idx++);
                tw.Write(": ");
                if (arg is Expr ex) {
                    ex.Print(tw);
                }else{
                    tw.Write(arg?.GetType()?.Name ?? "null");
                    tw.Write(" ");
                    tw.WriteLine(arg);
                }
            }
            tw.Indent -= 2;
        }

        public override string? ToString() {
            var sw = new IndentedTextWriter(new StringWriter());
            Print(sw);
            return sw.ToString();
        }
    }
    
    public class CodeInfo : Base.IAny{
        //An Any array of statements
        public object[] code;        
        
        /*
             An array of integer indices into the linetable, giving the location associated with each statement.
        */
        public int[] codelocs;
        
        //An array of source location objects
        public LineInfoNode[] linetable;
        
        public Type rettype;
        public ushort inlining_cost;
        public BasicOptimizationSettings constprop, inlineable;
        public bool propagate_inbounds;
        public Base.Symbol[] slotnames;
        public Base.Symbol[] slotflags;
        
        /*
            Either an array or an Int.

            If an Int, it gives the number of compiler-inserted temporary locations in the 
            function (the length of code array). If an array, specifies a type for each location.
         */
        public object ssavaluetypes;
    }

    public record LineInfoNode(Core.Module module, Base.Symbol method, Base.Symbol file, int inlined_at) : Base.IAny;
    public record GotoNode(Base.Int label) : Base.IAny;
    public record ReturnNode(object val) : Base.IAny;
    public record SlotNumber(Base.Int id) : Base.IAny;

    public record QuoteNode(object value) : IAny;
    public record GotoIfNot(Base.Int label, SSAValue cond) : Base.IAny;
    public record SSAValue(Base.Int id) : Base.IAny;
    public record GlobalRef(Base.Symbol name, Core.Module mod) : Base.IAny;


    public enum Purity {
        Consistent = 0x01 << 0,  // this method is guaranteed to return or terminate consistently (:consistent)
        EffectFree = 0x01 << 1, // this method is free from externally semantically visible side effects (:effect_free)
        NoThrow = 0x01 << 2,     // this method is guaranteed to not throw an exception (:nothrow)
        TerminatesGlobally = 0x01 << 3, //this method is guaranteed to terminate (:terminates_globally)
        TerminatesLocally =  0x01 << 4 //the syntactic control flow within this method is guaranteed to terminate (:terminates_locally)
    }

    public enum BasicOptimizationSettings : byte{
       Heuristic = 0,
       Aggressive = 1,
       None = 2
    }

    [Flags]
    public enum SlotFlag : byte {
       Assigned = 0x02,  //assigned (only false if there are no assignment statements with this var on the left)
       HasReadOrWrite = 0x08,  //used (if there is any read or write of the slot)
       StaticallyAssigned = 0x10,  //statically assigned once
       UseBeforeAssignment = 0x20 //might be used before assigned. This flag is only valid after type inference.
    }
    
    
}

