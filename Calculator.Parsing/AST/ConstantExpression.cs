using System;

namespace Calculator.Runtime.AST
{
    public class ConstantExpression : ASTNode
    {
        public readonly String Identifier;

        public ConstantExpression ( String Id )
        {
            this.Identifier = Id;
        }

        public override String ToString ( ) => $"Const<{this.Identifier}>";
    }
}
