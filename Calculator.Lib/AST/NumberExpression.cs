using System;
using GParse.Common;

namespace Calculator.Lib.AST
{
    public class NumberExpression : ASTNode
    {
        public Double Value;

        public NumberExpression ( Double num )
        {
            this.Value = num;
        }

        public override String ToString ( ) => $"Num<{this.Value}>";
    }
}
