using GParse.Common;
using GParse.Common.AST;

namespace Calculator.Lib.AST
{
    internal class ParenthesisExpression : ASTNode
    {
        public readonly ASTNode InnerExpression;

        public ParenthesisExpression ( ASTNode innerExpr )
        {
            this.InnerExpression = innerExpr;
        }

        public override System.String ToString ( ) => $"Paren<{this.InnerExpression}>";
    }
}
