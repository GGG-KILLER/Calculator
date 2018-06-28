using System;
using GParse.Common;
using GParse.Common.AST;

namespace Calculator.Lib.AST
{
    public class NumberExpression : ASTNode
    {
        public readonly Double Value;

        public NumberExpression ( Double num )
        {
            this.Value = num;
        }

        public override String ToString ( ) => $"Num<{this.Value}>";
    }
}
