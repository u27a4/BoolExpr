using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace boolexpr
{
    public class Parser
    {
        /// <summary>
        /// Symbols used in logical formula
        /// </summary>
        private enum Symbol
        {
            LP = 1, // (
            RP,     // )
            NOT,    // !
            AND,    // &
            OR,     // |
            IMP,    // =>
            IFF,    // <=>
            VAL,    // One of '1', '0' or Identifier
            EXP,    // Expression
            END,    // End of input
        };

        /// <summary>
        /// Conditions of Reduce for stacked 3 symbols and next one symbol
        /// </summary>
        private enum ReduceCondition
        {
            __V_RP  =                                      Symbol.VAL << 4 | Symbol.RP,
            __V_AND =                                      Symbol.VAL << 4 | Symbol.AND,
            __V_OR  =                                      Symbol.VAL << 4 | Symbol.OR,
            __V_IMP =                                      Symbol.VAL << 4 | Symbol.IMP,
            __V_IFF =                                      Symbol.VAL << 4 | Symbol.IFF,
            __V_END =                                      Symbol.VAL << 4 | Symbol.END,
            _NE_RP  =                    Symbol.NOT << 8 | Symbol.EXP << 4 | Symbol.RP,
            _NE_AND =                    Symbol.NOT << 8 | Symbol.EXP << 4 | Symbol.AND,
            _NE_OR  =                    Symbol.NOT << 8 | Symbol.EXP << 4 | Symbol.OR,
            _NE_IMP =                    Symbol.NOT << 8 | Symbol.EXP << 4 | Symbol.IMP,
            _NE_IFF =                    Symbol.NOT << 8 | Symbol.EXP << 4 | Symbol.IFF,
            _NE_END =                    Symbol.NOT << 8 | Symbol.EXP << 4 | Symbol.END,
            LER_RP  = Symbol.LP  << 12 | Symbol.EXP << 8 | Symbol.RP  << 4 | Symbol.RP,
            LER_AND = Symbol.LP  << 12 | Symbol.EXP << 8 | Symbol.RP  << 4 | Symbol.AND,
            LER_OR  = Symbol.LP  << 12 | Symbol.EXP << 8 | Symbol.RP  << 4 | Symbol.OR,
            LER_IMP = Symbol.LP  << 12 | Symbol.EXP << 8 | Symbol.RP  << 4 | Symbol.IMP,
            LER_IFF = Symbol.LP  << 12 | Symbol.EXP << 8 | Symbol.RP  << 4 | Symbol.IFF,
            LER_END = Symbol.LP  << 12 | Symbol.EXP << 8 | Symbol.RP  << 4 | Symbol.END,
            EAE_RP  = Symbol.EXP << 12 | Symbol.AND << 8 | Symbol.EXP << 4 | Symbol.RP,
            EAE_AND = Symbol.EXP << 12 | Symbol.AND << 8 | Symbol.EXP << 4 | Symbol.AND,
            EAE_OR  = Symbol.EXP << 12 | Symbol.AND << 8 | Symbol.EXP << 4 | Symbol.OR,
            EAE_IMP = Symbol.EXP << 12 | Symbol.AND << 8 | Symbol.EXP << 4 | Symbol.IMP,
            EAE_IFF = Symbol.EXP << 12 | Symbol.AND << 8 | Symbol.EXP << 4 | Symbol.IFF,
            EAE_END = Symbol.EXP << 12 | Symbol.AND << 8 | Symbol.EXP << 4 | Symbol.END,
            EOE_RP  = Symbol.EXP << 12 | Symbol.OR  << 8 | Symbol.EXP << 4 | Symbol.RP,
            EOE_OR  = Symbol.EXP << 12 | Symbol.OR  << 8 | Symbol.EXP << 4 | Symbol.OR,
            EOE_IMP = Symbol.EXP << 12 | Symbol.OR  << 8 | Symbol.EXP << 4 | Symbol.IMP,
            EOE_IFF = Symbol.EXP << 12 | Symbol.OR  << 8 | Symbol.EXP << 4 | Symbol.IFF,
            EOE_END = Symbol.EXP << 12 | Symbol.OR  << 8 | Symbol.EXP << 4 | Symbol.END,
            EIE_RP  = Symbol.EXP << 12 | Symbol.IMP << 8 | Symbol.EXP << 4 | Symbol.RP,
            EIE_IMP = Symbol.EXP << 12 | Symbol.IMP << 8 | Symbol.EXP << 4 | Symbol.IMP,
            EIE_IFF = Symbol.EXP << 12 | Symbol.IMP << 8 | Symbol.EXP << 4 | Symbol.IFF,
            EIE_END = Symbol.EXP << 12 | Symbol.IMP << 8 | Symbol.EXP << 4 | Symbol.END,
            EFE_RP  = Symbol.EXP << 12 | Symbol.IFF << 8 | Symbol.EXP << 4 | Symbol.RP,
            EFE_IFF = Symbol.EXP << 12 | Symbol.IFF << 8 | Symbol.EXP << 4 | Symbol.IFF,
            EFE_END = Symbol.EXP << 12 | Symbol.IFF << 8 | Symbol.EXP << 4 | Symbol.END,
        };

        /// <summary>
        /// Pattern of valid identifier
        /// </summary>
        private readonly Regex IdentifierSyntax = new Regex(@"^[a-zA-Z0-9\$_]+['""ʹʺ′″‴⁗]*");

        /// <summary>
        /// Parse specified string to 
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        public BoolExpr Parse(string expr)
        {
            var source = new StringBuilder(expr);
            var symbols = new Stack<Tuple<Symbol, BoolExpr>>();

            do
            {
                var token = Tokenize(source);
                while (Reduce(symbols, token.Item1)) ;
                symbols.Push(token);
            }
            while (symbols.Peek().Item1 != Symbol.END);

            if (symbols.Count() > 2)
            {
                throw new Exception("Syntax Error -2");
            }

            return symbols.ElementAt(1).Item2;
        }

        /// <summary>
        /// reduce symbols in stack
        /// </summary>
        /// <param name="next"></param>
        /// <param name="stack"></param>
        /// <returns></returns>
        private bool Reduce(Stack<Tuple<Symbol, BoolExpr>> stack, Symbol next)
        {
            Tuple<Symbol, BoolExpr> t1, t2, t3;

            // generate test condition for reduce
            long cond = (long)next;
            for (var i = 0; i < Math.Min(3, stack.Count()); i++)
            {
                cond |= (long)stack.ElementAt(i).Item1 << 4 * (i + 1);
            }

            switch ((ReduceCondition)cond)
            {   // stacked symbols <- next symbol
                // [EXP, AND, EXP] <- RP | AND | OR | IMP | IFF | END
                case ReduceCondition.EAE_RP:
                case ReduceCondition.EAE_AND:
                case ReduceCondition.EAE_OR:
                case ReduceCondition.EAE_IMP:
                case ReduceCondition.EAE_IFF:
                case ReduceCondition.EAE_END:
                // [EXP, OR, EXP]  <- RP | OR | IMP | IFF | END
                case ReduceCondition.EOE_RP:
                case ReduceCondition.EOE_OR:
                case ReduceCondition.EOE_IMP:
                case ReduceCondition.EOE_IFF:
                case ReduceCondition.EOE_END:
                // [EXP, IMP, EXP] <- RP | IMP | IFF | END
                case ReduceCondition.EIE_RP:
                case ReduceCondition.EIE_IMP:
                case ReduceCondition.EIE_IFF:
                case ReduceCondition.EIE_END:
                // [EXP, IFF, EXP] <- RP | IFF | END
                case ReduceCondition.EFE_RP:
                case ReduceCondition.EFE_IFF:
                case ReduceCondition.EFE_END:
                    t1 = stack.Pop();
                    t2 = stack.Pop();
                    t3 = stack.Pop();
                    t2.Item2.AddOperand(t3.Item2);
                    t2.Item2.AddOperand(t1.Item2);
                    stack.Push(Tuple.Create(Symbol.EXP, t2.Item2));
                    return true;
                // [ LP, EXP,  RP] <- RP | AND | OR | IMP | IFF | END
                case ReduceCondition.LER_RP:
                case ReduceCondition.LER_AND:
                case ReduceCondition.LER_OR:
                case ReduceCondition.LER_IMP:
                case ReduceCondition.LER_IFF:
                case ReduceCondition.LER_END:
                    t1 = stack.Pop();
                    t2 = stack.Pop();
                    t3 = stack.Pop();
                    stack.Push(t2);
                    return true;
            }

            switch ((ReduceCondition)(cond & 0xFFF))
            {   // stacked symbols <- next symbol
                // [NOT, EXP]      <- RP | AND | OR | IMP | IFF | END
                case ReduceCondition._NE_RP:
                case ReduceCondition._NE_AND:
                case ReduceCondition._NE_OR:
                case ReduceCondition._NE_IMP:
                case ReduceCondition._NE_IFF:
                case ReduceCondition._NE_END:
                    t1 = stack.Pop();
                    t2 = stack.Pop();
                    t2.Item2.AddOperand(t1.Item2);
                    stack.Push(Tuple.Create(Symbol.EXP, t2.Item2));
                    return true;
            }

            switch ((ReduceCondition)(cond & 0x00FF))
            {   // stacked symbols <- next symbol
                // [VAR]           <- RP | AND | OR | IMP | IFF | END
                case ReduceCondition.__V_RP:
                case ReduceCondition.__V_AND:
                case ReduceCondition.__V_OR:
                case ReduceCondition.__V_IMP:
                case ReduceCondition.__V_IFF:
                case ReduceCondition.__V_END:
                    t1 = stack.Pop();
                    stack.Push(Tuple.Create(Symbol.EXP, t1.Item2));
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Get pair of next token and boolexpr,
        /// and then remove that characters and whitespaces
        /// </summary>
        private Tuple<Symbol, BoolExpr> Tokenize(StringBuilder src)
        {
            int nRead = 1;
            Symbol symbol = Symbol.END;
            BoolExpr expr = null;

            // remove whitespaces
            while (src.Length > 0 && @" \t\n\r\f".IndexOf(src[0]) != -1) src.Remove(0, 1);

            // end of source?
            if (src.Length == 0)
            {
                return Tuple.Create(symbol, expr);
            }

            switch (src[0])
            {
                case '(':
                    symbol = Symbol.LP;
                    break;
                case ')':
                    symbol = Symbol.RP;
                    break;
                case '1':
                    symbol = Symbol.VAL;
                    expr = new BoolExpr(BoolExpr.NodeType.TRUE);
                    break;
                case '0':
                    symbol = Symbol.VAL;
                    expr = new BoolExpr(BoolExpr.NodeType.FALSE);
                    break;
                case '!':
                    symbol = Symbol.NOT;
                    expr = new BoolExpr(BoolExpr.NodeType.NOT);
                    break;
                case '&':
                    symbol = Symbol.AND;
                    expr = new BoolExpr(BoolExpr.NodeType.AND);
                    break;
                case '|':
                    symbol = Symbol.OR;
                    expr = new BoolExpr(BoolExpr.NodeType.OR);
                    break;
                case '=':
                    if (src.Length >= 2 && src[1] == '>')
                    {
                        nRead = 2;
                        symbol = Symbol.IMP;
                        expr = new BoolExpr(BoolExpr.NodeType.IMP);
                    }
                    break;
                case '<':
                    if (src.Length >= 3 && src[1] == '=' && src[2] == '>')
                    {
                        nRead = 3;
                        symbol = Symbol.IFF;
                        expr = new BoolExpr(BoolExpr.NodeType.IFF);
                    }
                    break;
                default: // identifier
                    var m = IdentifierSyntax.Match(src.ToString());
                    if (m.Success)
                    {
                        nRead = m.Value.Length;
                        symbol = Symbol.VAL;
                        expr = new BoolExpr(BoolExpr.NodeType.VAR, m.Value);
                    }
                    else
                    {
                        throw new Exception("Syntax Error -1");
                    }
                    break;
            }

            // remove tokenized characters
            src.Remove(0, nRead);

            return Tuple.Create(symbol, expr);
        }
    }
}
