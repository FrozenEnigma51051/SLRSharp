using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLR_parser
{
    class Rules
    {

        private static string rule;
        private static string semantics;

        public static string getRule()
        {
            rule = "" + "1) exp -> exp + term\n" +
                        "2) exp -> exp - term\n" +
                        "3) exp -> term\n" +
                        "4) term -> term * factor\n" +
                        "5) term -> factor\n" +
                        "6) factor -> ( exp )\n" +
                        "7) factor -> number";

            return rule;
        }

        public static string getSemantics()
        {
            semantics = "" + "1) exp1.val = exp2.val + term.val\n" +
                        "2) exp1.val = exp2.val - term.val\n" +
                        "3) exp.val = term.val\n" +
                        "4) term1.val = term2.val*factor.val\n" +
                        "5) term.val = factor.val\n" +
                        "6) factor.val = exp.val\n" +
                        "7) factor.val = num.val\n" +
                        "8) num.val = 0\n" +
                        "9) num.val = 1\n" +
                        "10) num.val = 2\n" +
                        "11) num.val = 3\n" +
                        "12) num.val = 4\n" +
                        "13) num.val = 5\n" +
                        "14) num.val = 6\n" +
                        "15) num.val = 7\n" +
                        "16) num.val = 8\n" +
                        "17) num.val = 9";

            return semantics;
        }


    }
}
