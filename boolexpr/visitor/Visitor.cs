using System;
using System.Collections.Generic;
using System.Linq;

namespace boolexpr.visitor
{
    /// <summary>
    /// Visitor for BoolExpr
    /// </summary>
    public class Visitor
    {
        public bool Aborted
        {
            get; private set;
        }
        
        /// <summary>
        /// Traverse each nodes in depth first search
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        private bool TraverseDFSPreorder(BoolExpr expr)
        {
            var stack = new Stack<BoolExpr>();
            stack.Push(expr);
            Aborted = false;
            do
            {
                Visit(expr = stack.Pop());

                foreach (var operand in expr.Operands)
                {
                    stack.Push(operand);
                }
            } while (stack.Count() > 0 && !Aborted);

            return Aborted;
        }
        
        /// <summary>
        /// Traverse each nodes in breadth first search
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        private bool TraverseBFS(BoolExpr expr)
        {
            var stack = new Stack<BoolExpr>();
            stack.Push(expr);
            Aborted = false;
            do
            {
                var newStack = new Stack<BoolExpr>();
                foreach (var b in stack)
                {
                    Visit(b);

                    foreach (var operand in b.Operands)
                    {
                        stack.Push(operand);
                    }
                }
                stack = newStack;
            } while (stack.Count() > 0 && !Aborted);

            return Aborted;
        }

        /// <summary>
        /// To abort traversing, call from visit method
        /// </summary>
        public void Abort()
        {
            Aborted = true;
        }

        /// <summary>
        /// Call visitor method for each node type
        /// </summary>
        /// <param name="b"></param>
        public void Visit(BoolExpr b)
        {
            switch (b.Type)
            {
                case BoolExpr.NodeType.IFF:
                    VisitIff(b);
                    break;
                case BoolExpr.NodeType.IMP:
                    VisitImp(b);
                    break;
                case BoolExpr.NodeType.AND:
                    VisitAnd(b);
                    break;
                case BoolExpr.NodeType.OR:
                    VisitOr(b);
                    break;
                case BoolExpr.NodeType.NOT:
                    VisitNot(b);
                    break;
                case BoolExpr.NodeType.VAR:
                    VisitVar(b);
                    break;
                case BoolExpr.NodeType.TRUE:
                    VisitTrue(b);
                    break;
                case BoolExpr.NodeType.FALSE:
                    VisitFalse(b);
                    break;
                default:
                    throw new Exception("Unknown NodeType found: " + b.Type);
            }
        }

        /// <summary>
        /// Do something for True Node
        /// </summary>
        /// <param name="b"></param>
        public virtual void VisitTrue(BoolExpr b)
        {

        }

        /// <summary>
        /// Do something for False Node
        /// </summary>
        /// <param name="b"></param>
        public virtual void VisitFalse(BoolExpr b)
        {

        }

        /// <summary>
        /// Do something for Var Node
        /// </summary>
        /// <param name="b"></param>
        public virtual void VisitVar(BoolExpr b)
        {

        }

        /// <summary>
        /// Do something for And Node
        /// </summary>
        /// <param name="b"></param>
        public virtual void VisitAnd(BoolExpr b)
        {

        }

        /// <summary>
        /// Do something for Or Node
        /// </summary>
        /// <param name="b"></param>
        public virtual void VisitOr(BoolExpr b)
        {

        }

        /// <summary>
        /// Do something for Imp Node
        /// </summary>
        /// <param name="b"></param>
        public virtual void VisitImp(BoolExpr b)
        {

        }

        /// <summary>
        /// Do something for Iff Node
        /// </summary>
        /// <param name="b"></param>
        public virtual void VisitIff(BoolExpr b)
        {

        }

        /// <summary>
        /// Do something for Not Node
        /// </summary>
        /// <param name="b"></param>
        public virtual void VisitNot(BoolExpr b)
        {

        }
    }
}
