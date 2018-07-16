using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace boolexpr
{
    class Program
    {
        static void Main(string[] args)
        {
            var parser = new Parser();
            var b1 = parser.Parse("1 & x | 0");
            var b2 = parser.Parse("1 & (x | 0)");
            var b3 = parser.Parse("x & (!x | y)");

            System.Console.WriteLine(b1.ToString());
            System.Console.WriteLine(b2.ToString());
            System.Console.WriteLine(b3.ToSATString());
        }
    }
}
