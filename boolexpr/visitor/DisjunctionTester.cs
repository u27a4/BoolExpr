namespace boolexpr.visitor
{
    /// <summary>
    /// Tester that test whether boolexpr is disjunction
    /// </summary>
    public class DisjunctionTester : Visitor
    {
        /// <summary>
        /// Test whether boolexpr is disjunction
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public bool Test(BoolExpr expr)
        {
            Visit(expr);

            return !Aborted;
        }

        public override void VisitAnd(BoolExpr b)
        {
            Abort();
        }

        public override void VisitImp(BoolExpr b)
        {
            Abort();
        }

        public override void VisitIff(BoolExpr b)
        {
            Abort();
        }
    }
}
