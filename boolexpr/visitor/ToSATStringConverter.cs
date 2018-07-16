using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace boolexpr.visitor
{
    /// <summary>
    /// Converter that BoolExpr instance to SAT string
    /// </summary>
    public class ToSATStringConverter : Visitor
    {
        private StringBuilder _sb;

        /// <summary>
        /// Convert to SAT string
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
            _sb.Append("-");
            Visit(e.Operands.ElementAt(0));
        }

        public override void VisitAnd(BoolExpr e)
        {
            for (var i = 0; i < e.Operands.Count(); i++)
            {
                Visit(e.Operands.ElementAt(i));
                if (i + 1 != e.Operands.Count()) _sb.Append(Environment.NewLine);
            }
        }

        public override void VisitOr(BoolExpr e)
        {
            for (var i = 0; i < e.Operands.Count(); i++)
            {
                Visit(e.Operands.ElementAt(i));
                if (i + 1 != e.Operands.Count()) _sb.Append(" ");
            }
        }

        public override void VisitVar(BoolExpr e)
        {
            _sb.Append(e.Name);
        }
    }
}
