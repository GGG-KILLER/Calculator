using Calculator.Lib.Abstractions;
using GParse.Common;
using GParse.Common.AST;

namespace Calculator.Lib.AST
{
    public class ParenthesisExpression : CASTNode
    {
        public readonly CASTNode InnerExpression;

        public ParenthesisExpression ( CASTNode innerExpr )
        {
            this.InnerExpression = innerExpr;
        }

        public override void Accept ( ICNodeTreeVisitor visitor ) => visitor.Visit ( this );

        public override T Accept<T> ( ICNodeTreeVisitor<T> visitor ) => visitor.Visit ( this );

        public override System.String ToString ( ) => $"Paren<{this.InnerExpression}>";
    }
}
