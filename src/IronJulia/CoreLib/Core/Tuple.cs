using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SpinorCompiler.Boot;
using static Base;
using JuliaTupleUtils = SpinorCompiler.Boot.JuliaTupleUtils;

public partial struct Core {
     
    public interface JTuple : Any{
        public Any this[int index] { get; }
        public int Length { get; }
        public object NetTupleValue { get; }
        public bool IsNTuple { get; }
    }
    
    [StructLayout(LayoutKind.Sequential)]
    public struct Tuple<TT> : JTuple where TT: ITuple {
        public static readonly (SpinorCompiler.Boot.TypeTuple TypeTuple, bool IsNTuple) Meta;
        
        public TT Value;
        public Tuple(){}
        public Tuple(TT tuple) => Value = tuple;
        public Any this[int index] => (Any) Value[index]!;
        public int Length => Meta.TypeTuple.Types.Length;
        public object NetTupleValue => Value;
        public bool IsNTuple => Meta.IsNTuple;
        public static implicit operator TT(Tuple<TT> tuple) => tuple.Value;
        public static implicit operator Tuple<TT>(TT tuple) => new(tuple);
        private static bool GetMeta(System.Type t, List<System.Type> typeTuple) {
            var isNTuple = true;
            Span<System.Type> args = t.GetGenericArguments();
            var ty = args.Length > 0 ? args[0] : typeof(object);
            
            var n = Math.Min(7, args.Length);
            for (var i = 0; i < n; i++) {
                if (args[i] != ty) {
                    isNTuple = false;
                }
                typeTuple.Add(args[i]);
            }

            return (args.Length != 8 || GetMeta(args[7], typeTuple)) & isNTuple;
        }

        static Tuple() {
            var typeTuple = new List<System.Type>();
            var isNTuple = GetMeta(typeof(TT), typeTuple);
            Meta = (new TypeTuple(typeTuple.ToArray()), isNTuple);
            JuliaTupleUtils._type2tuples[typeof(Tuple<TT>)] = Meta.TypeTuple;
            JuliaTupleUtils._tuples2type[Meta.TypeTuple] = typeof(Tuple<TT>);
        }

        public override string ToString() => Value.ToString()!;
    }
}