using boolexpr.visitor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace boolexpr
{
    using OperandArray = List<BoolExpr>;

    /// <summary>
    /// Represents a logical formulas abstract syntax tree node
    /// </summary>
    public class BoolExpr
    {
        /// <summary>
        /// Node types in abstract syntax tree
        /// </summary>
        public enum NodeType
        {
            IFF, IMP, AND, OR, NOT, VAR, TRUE, FALSE,
        };

        /// <summary>
        /// Type of this node
        /// </summary>
        public NodeType Type
        {
            get; private set;
        }

        /// <summary>
        /// Name of this node
        /// </summary>
        public string Name
        {
            get; private set;
        }

        /// <summary>
        /// Operands of this node
        /// </summary>
        public IEnumerable<BoolExpr> Operands
        {
            get; private set;
        }

        /// <summary>
        /// Left-hand side operand
        /// </summary>
        public BoolExpr Left
        {
            get
            {
                return Operands.ElementAt(0);
            }
        }

        /// <summary>
        /// Right-hand side operand
        /// </summary>
        public BoolExpr Right
        {
            get
            {
                return Operands.ElementAt(1);
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        public BoolExpr (NodeType type, String name = "")
        {
            Type = type;
            Name = name;
            Operands = new OperandArray();
        }

        /// <summary>
        /// Return true if this node has operands, otherwise return false
        /// </summary>
        /// <returns></returns>
        public bool HasOperands()
        {
            return Operands.Count() > 0;
        }

        /// <summary>
        /// Add Operand to this node
        /// </summary>
        /// <param name="operand"></param>
        public void AddOperand(BoolExpr operand)
        {
            ((OperandArray)Operands).Add(operand);
        }

        /// <summary>
        /// Return true if this node is conjunction, otherwise return false
        /// </summary>
        /// <returns></returns>
        public bool IsConjunction()
        {
            return new ConjunctionTester().Test(this);
        }

        /// <summary>
        /// Return true if this node is disjunction, otherwise return false
        /// </summary>
        /// <returns></returns>
        public bool IsDisjunction()
        {
            return new DisjunctionTester().Test(this);
        }

        /// <summary>
        /// Convert to SAT string
        /// </summary>
        /// <returns></returns>
        public string ToSATString()
        {
            return new ToSATStringConverter().Convert(this);
        }

        /// <summary>
        /// Convert to string
        /// </summary>
        /// <returns></returns>
        public new string ToString()
        {
            return new ToStringConverter().Convert(this);
        }
    }
}
