using System.CodeDom.Compiler;
using IronJulia.CoreLib;

public partial struct Base {
    public readonly struct Symbol : Any, IEquatable<Symbol>
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
    
    public class Expr : Any {
        public Symbol head;
        public Core.Array<Any, Vals.Int1, ValueTuple<Int>> args;

        public Expr(Symbol head) {
            this.head = head;
            args = new(ValueTuple.Create(4));
        }
        
        public Expr(Symbol head, params Core.Array<Any, Vals.Int1, ValueTuple<Int>> args) {
            this.head = head;
            this.args = args;
        }

        public void Print(IndentedTextWriter tw) {
            tw.WriteLine("Expr ");
            tw.Indent++;
            tw.Write("Head: Symbol ");
            tw.WriteLine(head);
            tw.Write("Args Length: ");
            tw.WriteLine(args.size[0]);
            tw.Indent++;
            var idx = 1;
            foreach (var arg in args) {
                tw.Write(idx++);
                tw.Write(": ");
                if (arg is Expr ex) {
                    ex.Print(tw);
                }else{
                    tw.Write(arg.GetType().Name);
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
    
    
}