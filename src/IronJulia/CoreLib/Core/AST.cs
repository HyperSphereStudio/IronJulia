using IronJulia.CoreLib;

public partial struct Core {

    public class CodeInfo : Base.Any{
        public Array<Base.Any, Vals.Int1, ValueTuple<Vals.Int1>> code;
        public Array<int, Vals.Int1, ValueTuple<Vals.Int1>> codelocs;
        public Array<LineInfoNode, Vals.Int1, ValueTuple<Vals.Int1>> linetable;
        public Type rettype;
        public ushort inlining_cost;
        public BasicOptimizationSettings constprop, inlineable;
        public bool propagate_inbounds;
        
    }

    public record LineInfoNode(Core.Module module, Base.Symbol method, Base.Symbol file, int inlined_at) : Base.Any;
    public record GotoNode(Base.Int label) : Base.Any;
    public record ReturnNode(Base.Any val) : Base.Any;
    public record SlotNumber(Base.Int id) : Base.Any;

    public record QuoteNode(Base.Any value) : Base.Any;
    public record GotoIfNot(Base.Int label, Core.SSAValue cond) : Base.Any;
    public record SSAValue(Base.Int id) : Base.Any;
    public record GlobalRef(Base.Symbol name, Core.Module mod) : Base.Any;


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
    
    
}

