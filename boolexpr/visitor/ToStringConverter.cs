using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace boolexpr.visitor
{
    /// <summary>
    /// Converter that BoolExpr instance to string
    /// </summary>
    public class ToStringConverter : Visitor
    {
        private StringBuilder _sb;
        
        /// <summary>
        /// Convert to string
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public String Convert(BoolExpr e)
        {
            _sb = new StringBuilder();

            Visit(e);

            return _sb.ToString();
        }

        public override void VisitNot(BoolExpr e)
        {
            _sb.Append("!");
            Visit(e.Operands.ElementAt(0));
        }

        public override void VisitAnd(BoolExpr e)
        {
            _sb.Append("(");
            for (var i = 0; i < e.Operands.Count(); i++)
            {
                Visit(e.Operands.ElementAt(i));
                if (i + 1 != e.Operands.Count()) _sb.Append(" & ");
            }
            _sb.Append(")");
        }

        public override void VisitOr(BoolExpr e)
        {
            _sb.Append("(");
            for (var i = 0; i < e.Operands.Count(); i++)
            {
                Visit(e.Operands.ElementAt(i));
                if (i + 1 != e.Operands.Count()) _sb.Append(" | ");
            }
            _sb.Append(")");
        }

        public override void VisitImp(BoolExpr e)
        {
            _sb.Append("(");
            Visit(e.Left);
            _sb.Append(" => ");
            Visit(e.Right);
            _sb.Append(")");
        }

        public override void VisitIff(BoolExpr e)
        {
            _sb.Append("(");
            Visit(e.Left);
            _sb.Append(" <=> ");
            Visit(e.Right);
            _sb.Append(")");
        }

        public override void VisitVar(BoolExpr e)
        {
            _sb.Append(e.Name);
        }

        public override void VisitTrue(BoolExpr l)
        {
            _sb.Append("1");
        }
        
        public override void VisitFalse(BoolExpr l)
        {
            _sb.Append("0");
        }
    }
}
