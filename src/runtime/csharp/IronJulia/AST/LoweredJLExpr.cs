using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using SpinorCompiler.Utils;

namespace IronJulia.AST;

public static class LoweredJLExpr {
    public interface ILoweredJLExpr : Base.Any, INodeVisitor<ILoweredJLExpr> {
        public Type ReturnType { get; set; }
    }
    
    public class Constant : ILoweredJLExpr {
        public static readonly Constant Nothing = new(Base.Nothing.BoxedInstance), Int0 = new(new Base.Int(0));
        
        public Base.Any Value;
        public Type ReturnType { get => Value.GetType(); set => throw new NotImplementedException(); }
        private Constant(Base.Any value) => Value = value;

        public static Constant Create(Base.Any value) {
            if (value == Base.Nothing.BoxedInstance)
                return Nothing;
            if (value == Int0)
                return Int0;
            return new(value);
        }
        
        public uint? Visit(NonRecursiveGraphVisitor<ILoweredJLExpr> visit, NodeVisitorState<ILoweredJLExpr> state) => null;
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
        public readonly Block Conditional;
        public readonly Block Body;
        public readonly Label Break, Continue;
        public Type ReturnType { get; set; }
        
        internal While(Block parent) {
            Conditional = parent.CreateBlock();
            Body = parent.CreateBlock();
            Break = parent.CreateLabel();
            Continue = parent.CreateLabel();
        }
        
        public uint? Visit(NonRecursiveGraphVisitor<ILoweredJLExpr> visit, NodeVisitorState<ILoweredJLExpr> state) {
            switch (state.State) {
                case EvalCondState:
                    visit.PushNewVisit(Conditional);
                    return EvalBodyState;
                case EvalBodyState:
                    visit.PushNewVisit(Body);
                    return null;
            }
            return null;
        }
    }

    public class If : ILoweredJLExpr {
        public ILoweredJLExpr IfTrue, Conditional;
        public Type ReturnType { get; set; }
        
        private If(ILoweredJLExpr ifTrue, ILoweredJLExpr conditional) {
            IfTrue = ifTrue;
            Conditional = conditional;
        }

        public static If Create(ILoweredJLExpr ifTrue, ILoweredJLExpr conditional) => new(ifTrue, conditional);
        
        public uint? Visit(NonRecursiveGraphVisitor<ILoweredJLExpr> visit, NodeVisitorState<ILoweredJLExpr> state) {
            if (state.State == 0) {
                visit.PushNewVisit(IfTrue);  
                return 1;
            }
            visit.PushNewVisit(Conditional);
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
        public readonly List<Variable> Variables;
        public readonly List<ILoweredJLExpr> Statements;
        public readonly List<Label> Labels;
        public System.Type ReturnType { get; set; }

        private Block(Block? parent, List<Variable> variables) {
            Parent = parent;
            Variables = variables;
            Statements = new();
            Labels = new();
        }
        
        public static Block CreateRootBlock() {
            return new(null, new());
        }

        public Block CreateBlock() => new(this, new());

        public Label CreateLabel(Base.Symbol? name = null) {
            var lbl = Label.Create(name);
            Labels.Add(lbl);
            return lbl;
        }

        public While CreateWhile() => new (this);
        
        public Variable? FindVariableInScope(Base.Symbol name, bool allscopes = false) {
            var v = Variables.FirstOrDefault(v => v.Name == name);
            if (v != null || !allscopes)
                return v;
            return Parent.FindVariableInScope(name, true);
        }
        
        public Variable CreateVariable(Type bindingType, Base.Symbol? name = null) {
              var v = new Variable(bindingType, this) {
                  Name = name
              };
              return v;
        }

        public uint? Visit(NonRecursiveGraphVisitor<ILoweredJLExpr> visit, NodeVisitorState<ILoweredJLExpr> state){
            if (state.State >= Statements.Count) return null;
            visit.PushNewVisit(Statements[(int) state.State!.Value]);
            return state.State + 1 >= Statements.Count ? null : state.State + 1;
        }
    }

    public class Variable : ILoweredJLExpr{
        public Base.Symbol? Name;
        public Block Parent;
        public Type ReturnType { get; set; }

        public uint? Visit(NonRecursiveGraphVisitor<ILoweredJLExpr> visit, NodeVisitorState<ILoweredJLExpr> state) => null;
        internal Variable(Type returnType, Block parent) {
            ReturnType = returnType;
            Parent = parent;
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