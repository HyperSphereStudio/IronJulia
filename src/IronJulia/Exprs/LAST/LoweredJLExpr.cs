global using LoweredCallsite = JulianCallsite<IronJulia.AST.LoweredJLExpr.ILoweredJLExpr, IronJulia.AST.LoweredJLExpr.ILoweredJLExpr>;

using System.Diagnostics;
using IronJulia.CoreLib;

namespace IronJulia.AST;

public static class LoweredJLExpr {
    public static LoweredCallsite NewCallsite(params ILoweredJLExpr[] args) {
        var j = new LoweredCallsite();
        foreach (var a in args)
            j.AddArg(a);
        return j;
    }
    
    public interface ILoweredJLExpr : Base.IAny, INodeVisitor<ILoweredJLExpr>, ICallsiteValue<ILoweredJLExpr, ILoweredJLExpr> {
        public Type ReturnType { get; set; }
        ILoweredJLExpr ICallsiteValue<ILoweredJLExpr, ILoweredJLExpr>.Value => this;
        Type ICallsiteValue<ILoweredJLExpr, ILoweredJLExpr>.Type => ReturnType;
    }
    
    public class Constant : ILoweredJLExpr {
        private static readonly Dictionary<object, Constant> _constantCache = new();
        
        public object Value;
        public Type ReturnType { get => Value.GetType(); set => throw new NotImplementedException(); }
        private Constant(object value) => Value = value;

        public static Constant Create(object value) => _constantCache.TryGetValue(value, out var v) ? v : new(value);
        public static Constant Create<T>(T value) => Create(jlapi.jl_box(value));
        
        public uint? Visit(NonRecursiveGraphVisitor<ILoweredJLExpr> visit, NodeVisitorState<ILoweredJLExpr> state) => null;

        static Constant() {
            AddToCache(new Base.Int(0));
            AddToCache(new Base.Int(1));
            AddToCache(new Base.Int(-1));
            AddToCache(Base.Bool.True);
            AddToCache(Base.Bool.False);
            AddToCache(Base.Nothing.Instance);

            void AddToCache<T>(T value){
                var k = jlapi.jl_box(value);
                _constantCache[k] = new(k);
            }
        }
    }

    public class Label : ILoweredJLExpr {
        public Base.Symbol? Name;
        public Type ReturnType { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
        
        private Label(Base.Symbol? name) {
            Name = name;
        }

        internal static Label Create(Base.Symbol? name) {
            return new(name);
        }
        
        public uint? Visit(NonRecursiveGraphVisitor<ILoweredJLExpr> visit, NodeVisitorState<ILoweredJLExpr> state) => null;
    }

    public class While : ILoweredJLExpr {
        public const uint EvalCondState = 0, EvalBodyState = 1;
        public readonly Block Condition;
        public readonly Block Body;
        public readonly Label Break, Continue;
        public Type ReturnType { get; set; }
        
        internal While(Block parent) {
            Condition = parent.CreateBlock();
            Body = parent.CreateBlock();
            Break = parent.CreateLabel();
            Continue = parent.CreateLabel();
        }
        
        public uint? Visit(NonRecursiveGraphVisitor<ILoweredJLExpr> visit, NodeVisitorState<ILoweredJLExpr> state) {
            switch (state.State) {
                case EvalCondState:
                    visit.PushNewVisit(Condition);
                    return EvalBodyState;
                case EvalBodyState:
                    visit.PushNewVisit(Body);
                    return null;
            }
            return null;
        }
    }
    
    public class For : ILoweredJLExpr {
        public const uint EvalInitialization = 0, EvalCondState = 1, EvalBodyState = 2, EvalNextState = 3;
        public readonly Block Iterable, Body;
        public FunctionInvoke Initialization, IterateNext;
        public readonly Variable State;
        public Type ReturnType { get; set; }
        
        internal For(Block parent) {
            Body = parent.CreateBlock();
            State = parent.CreateVariable(typeof(object), "state");
            Iterable = parent.CreateBlock();
            Initialization = FunctionInvoke.Create(Base.iterate, NewCallsite(Iterable));
            IterateNext = FunctionInvoke.Create(Base.iterate, NewCallsite(Iterable, State));
        }

        public uint? Visit(NonRecursiveGraphVisitor<ILoweredJLExpr> visit, NodeVisitorState<ILoweredJLExpr> state) {
            switch (state.State) {
                case EvalInitialization:
                    visit.PushNewVisit(Initialization);
                    return EvalCondState;
                case EvalCondState:  //Builtin state !== nothing
                    return EvalBodyState;
                case EvalBodyState:
                    visit.PushNewVisit(Body);
                    return EvalNextState;
                case EvalNextState:
                    visit.PushNewVisit(IterateNext);
                    return null;
            }
            return null;
        }
        
    }

    public class Conditional : ILoweredJLExpr {
        public const uint EvalCondState = 0, EvalBodyState = 1, EvalElseState = 2;
        public readonly Block Body;
        public readonly Block? Condition;
        public readonly Conditional? Else;
        
        public Type ReturnType { get; set; }
        
        internal Conditional(Block parent, bool condition, bool hasElse) {
            Body = parent.CreateBlock();
            if(condition)
                Condition = parent.CreateBlock();
            if(hasElse)
                Else = parent.CreateConditional();
        }
        
        public uint? Visit(NonRecursiveGraphVisitor<ILoweredJLExpr> visit, NodeVisitorState<ILoweredJLExpr> state) {
            switch (state.State) {
                case EvalCondState:
                    if(Condition != null)
                        visit.PushNewVisit(Condition);
                    return EvalBodyState;
                case EvalBodyState:
                    visit.PushNewVisit(Body);
                    return Else != null ? EvalElseState : null;
                case EvalElseState:
                    if(Else != null)
                        visit.PushNewVisit(Else);
                    return null;
            }
            return null;
        }
    }
    
    public class Goto : ILoweredJLExpr {
        public Type ReturnType { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
        public readonly Label Label;
        
        private Goto(Label label) {
            Label = label;
        }

        public static Goto Create(Label label) {
            return new(label);
        }

        public uint? Visit(NonRecursiveGraphVisitor<ILoweredJLExpr> visit, NodeVisitorState<ILoweredJLExpr> state) => null;
    }
    
    public class Block : ILoweredJLExpr {
        public readonly Block? Parent;
        public readonly Dictionary<Base.Symbol, Variable> Variables = [];
        public readonly List<ILoweredJLExpr> Statements = [];
        public readonly List<Label> Labels = [];
        public Type ReturnType { get; set; }

        protected Block(Block? parent) {
            Parent = parent;
        }

        public T Append<T>(T t) where T : ILoweredJLExpr {
            Statements.Add(t);
            return t;
        }

        public static Block Create() => new(null);

        public Block CreateBlock() => new(this);
        
        public Label CreateLabel(Base.Symbol? name = null) {
            var lbl = Label.Create(name);
            Labels.Add(lbl);
            return lbl;
        }

        public While CreateWhile() => new (this);
        public For CreateFor() => new(this);
        
        public Variable? FindVariableInScope(Base.Symbol name, bool allScopes = false) {
            if (Variables.TryGetValue(name, out var v) || !allScopes)
                return v;
            return Parent!.FindVariableInScope(name, true);
        }
        
        public Variable CreateVariable(Type bindingType, Base.Symbol? name = null) {
              var i = 0;
              var rootName = name ?? "var";
              name ??= rootName;
              
              while (Variables.ContainsKey(name.Value)) {
                  Console.WriteLine(name);
                  name = rootName.Value + i;
                  i++;
              }
            
              var v = new Variable(bindingType, this, name.Value);
              Variables[name.Value] = v;
              return v;
        }

        public Conditional CreateConditional(bool hasCondition = true, bool hasElse = false) => new (this, hasCondition, hasElse);

        public uint? Visit(NonRecursiveGraphVisitor<ILoweredJLExpr> visit, NodeVisitorState<ILoweredJLExpr> state){
            if (state.State >= Statements.Count) return null;
            visit.PushNewVisit(Statements[(int) state.State!.Value]);
            return state.State + 1 >= Statements.Count ? null : state.State + 1;
        }
    }

    public class Variable : ILoweredJLExpr{
        public readonly Base.Symbol Name;
        public readonly Block Parent;
        public Type ReturnType { get; set; }

        public uint? Visit(NonRecursiveGraphVisitor<ILoweredJLExpr> visit, NodeVisitorState<ILoweredJLExpr> state) => null;
        internal Variable(Type returnType, Block parent, Base.Symbol name) {
            ReturnType = returnType;
            Parent = parent;
            Name = name;
        }
    }

    //Optimized GetField
    public class GetProperty : ILoweredJLExpr
    {
        public const int EvalInstance = 0;
        public readonly ILoweredJLExpr Instance;
        public readonly Base.Symbol Name;
        public Type ReturnType { get; set; }

        internal GetProperty(ILoweredJLExpr instance, Base.Symbol name) {
            Instance = instance;
            Name = name;
        }
        
        public static GetProperty Create(ILoweredJLExpr instance, Base.Symbol name) => new(instance, name);
        
        public uint? Visit(NonRecursiveGraphVisitor<ILoweredJLExpr> visit, NodeVisitorState<ILoweredJLExpr> state) {
            if (state.State == EvalInstance) 
                visit.PushNewVisit(Instance);
            return null;
        }
    }
    
    //Optimized SetProperty
    public class SetProperty : ILoweredJLExpr {
        public const int EvalValue = 0, EvalInstance = 1;
        public readonly ILoweredJLExpr Instance, Value;
        public readonly Base.Symbol Name;
        public Type ReturnType { get; set; }

        internal SetProperty(ILoweredJLExpr instance, Base.Symbol name, ILoweredJLExpr value) {
            Instance = instance;
            Name = name;
            Value = value;
        }

        public static SetProperty Create(ILoweredJLExpr instance, Base.Symbol name, ILoweredJLExpr value) => new(instance, name, value);
        
        public uint? Visit(NonRecursiveGraphVisitor<ILoweredJLExpr> visit, NodeVisitorState<ILoweredJLExpr> state) {
            switch (state.State) {
                case EvalValue:
                    visit.PushNewVisit(Value);
                    return EvalInstance;
                case EvalInstance:
                    visit.PushNewVisit(Instance);
                    break;
            }
            return null;
        }
    }

    public class FunctionInvoke : ILoweredJLExpr {
        public Core.Function Function;
        public LoweredCallsite CallSite;
        public Type ReturnType { get; set; }
        
        protected FunctionInvoke(Core.Function function, LoweredCallsite site) {
            Debug.Assert(function != null);
            Function = function;
            CallSite = site;
        }

        public static FunctionInvoke Create(Core.Function function, LoweredCallsite site) {
            return new FunctionInvoke(function, site);
        }

        public uint? Visit(NonRecursiveGraphVisitor<ILoweredJLExpr> visit, NodeVisitorState<ILoweredJLExpr> state) {
            if (state.State >= CallSite.Values.Count) {
                if(state.State >= CallSite.KeyValues.Count) return null;
                visit.PushNewVisit(CallSite.KeyValues[(int) state.State!.Value].Value);
                return state.State + 1 >= CallSite.KeyValues.Count ? null : state.State + 1;
            }
            visit.PushNewVisit(CallSite.Values[(int) state.State!.Value]);
            return state.State + 1 >= CallSite.Values.Count ? null : state.State + 1;
        }
    } 

    public class BinaryOperatorInvoke : FunctionInvoke {
        protected BinaryOperatorInvoke(Core.Function function, LoweredCallsite site) : base(function, site)
        {
            Debug.Assert(function != null);
            Debug.Assert(site.Values.Count == 2);
            Debug.Assert(site.KeyValues.Count == 0);
            Debug.Assert(site.KeyArgNames.Count == 0);
        }
        
        public static BinaryOperatorInvoke Create(Core.Function function, ILoweredJLExpr left, ILoweredJLExpr right) {
            return new BinaryOperatorInvoke(function, NewCallsite(left, right));
        }
    }

    public class Assignment : ILoweredJLExpr {
        public readonly Variable Variable;
        public readonly ILoweredJLExpr Expr;
        public readonly bool IsBinaryCompound;
        public BinaryOperatorInvoke BinaryOperator => (BinaryOperatorInvoke) Expr;
        public Type ReturnType { get; set; }
        private Assignment(Variable variable, ILoweredJLExpr expr, bool isBinaryCompound) {
            Variable = variable;
            Expr = expr;
            IsBinaryCompound = isBinaryCompound;
            if (IsBinaryCompound && Expr is not BinaryOperatorInvoke)
                throw new NotSupportedException("Expr must be BinaryOperatorInvoke if IsBinaryCompound=true");
        }
        
        public static Assignment Create(Variable variable, ILoweredJLExpr expr, bool isBinaryCompound = false) => new(variable, expr, isBinaryCompound);
        
        public uint? Visit(NonRecursiveGraphVisitor<ILoweredJLExpr> visit, NodeVisitorState<ILoweredJLExpr> state){
            visit.PushNewVisit(Expr);
            return null;
        }
    }
}