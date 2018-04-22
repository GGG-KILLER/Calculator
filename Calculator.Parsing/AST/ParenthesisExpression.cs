namespace Calculator.Runtime.AST
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
