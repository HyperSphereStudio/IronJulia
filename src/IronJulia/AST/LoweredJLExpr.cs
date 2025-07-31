using System.Collections;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using IronJulia.CoreLib;
using SpinorCompiler.Utils;

namespace IronJulia.AST;

public static class LoweredJLExpr {
    public interface ILoweredJLExpr : Base.Any, INodeVisitor<ILoweredJLExpr> {
        public Type ReturnType { get; set; }
    }
    
    public class Constant : ILoweredJLExpr {
        private static readonly Dictionary<Base.Any, Constant> _constantCache = new();
        
        public Base.Any Value;
        public Type ReturnType { get => Value.GetType(); set => throw new NotImplementedException(); }
        private Constant(Base.Any value) => Value = value;

        public static Constant Create(Base.Any value) => _constantCache.TryGetValue(value, out var v) ? v : new(value);
        public static Constant Create<T>(T value) where T: Base.Any => Create(jlapi.jl_box(value));
        
        public uint? Visit(NonRecursiveGraphVisitor<ILoweredJLExpr> visit, NodeVisitorState<ILoweredJLExpr> state) => null;

        static Constant() {
            AddToCache(new Base.Int(0));
            AddToCache(new Base.Int(1));
            AddToCache(new Base.Int(-1));
            AddToCache(Base.Bool.True);
            AddToCache(Base.Bool.False);
            AddToCache(Base.Nothing.Instance);

            void AddToCache<T>(T value) where T: Base.Any{
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
            State = parent.CreateVariable(typeof(Base.Any), "state");
            Iterable = parent.CreateBlock();
            Initialization = FunctionInvoke.Create(Base.iterate, [Iterable]);
            IterateNext = FunctionInvoke.Create(Base.iterate, [Iterable, State]);
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
        public System.Type ReturnType { get; set; }

        private Block(Block? parent) {
            Parent = parent;
        }
        
        public static Block CreateRootBlock() {
            return new(null);
        }

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

    public class FunctionInvoke : ILoweredJLExpr {
        public Core.Function Function; 
        public ILoweredJLExpr[] Arguments;
        public Type ReturnType { get; set; }
        
        protected FunctionInvoke(Core.Function function, ILoweredJLExpr[] arguments) {
            Debug.Assert(function != null);
            Function = function;
            Arguments = arguments;
        }

        public static FunctionInvoke Create(Core.Function function, ILoweredJLExpr[] arguments) {
            return new FunctionInvoke(function, arguments);
        }

        public uint? Visit(NonRecursiveGraphVisitor<ILoweredJLExpr> visit, NodeVisitorState<ILoweredJLExpr> state) {
            if (state.State >= Arguments.Length) return null;
            visit.PushNewVisit(Arguments[state.State!.Value]);
            return state.State + 1 >= Arguments.Length ? null : state.State + 1;
        }
    } 

    public class BinaryOperatorInvoke : FunctionInvoke {
        protected BinaryOperatorInvoke(Core.Function function, params ILoweredJLExpr[] arguments) : base(function, arguments) {}
        public static BinaryOperatorInvoke Create(Core.Function function, ILoweredJLExpr left, ILoweredJLExpr right) {
            return new BinaryOperatorInvoke(function, left, right);
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