namespace boolexpr.visitor
{
    /// <summary>
    /// Tester that test whether boolexpr is conjunction
    /// </summary>
    public class ConjunctionTester : Visitor
    {
        /// <summary>
        /// test whether boolexpr is conjunction
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public bool Test(BoolExpr expr)
        {
            Visit(expr);

            return !Aborted;
        }

        public override void VisitOr(BoolExpr b)
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
