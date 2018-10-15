using Calculator.Parsing.Abstractions;
using Calculator.Parsing.AST;

namespace Calculator.Parsing.Visitors
{
    public class TreeWalker : ITreeVisitor, ITreeWalker
    {
        #region ITreeVisitor

        public virtual void Visit ( BinaryOperatorExpression binaryOperator )
        {
            this.Walk ( binaryOperator );
            binaryOperator.LeftHandSide.Accept ( this );
            binaryOperator.RightHandSide.Accept ( this );
        }

        public virtual void Visit ( IdentifierExpression identifier ) => this.Walk ( identifier );

        public virtual void Visit ( FunctionCallExpression functionCall )
        {
            this.Walk ( functionCall );
            for ( var i = 0; i < functionCall.Arguments.Length; i++ )
                functionCall.Arguments[i].Accept ( this );
        }

        public virtual void Visit ( NumberExpression number ) => this.Walk ( number );

        public virtual void Visit ( UnaryOperatorExpression unaryOperator )
        {
            this.Walk ( unaryOperator );
            unaryOperator.Operand.Accept ( this );
        }

        #endregion ITreeVisitor

        #region ITreeWalker

        public virtual void Walk ( BinaryOperatorExpression binaryOperator )
        {
        }

        public virtual void Walk ( IdentifierExpression identifier )
        {
        }

        public virtual void Walk ( FunctionCallExpression functionCall )
        {
        }

        public virtual void Walk ( NumberExpression number )
        {
        }

        public virtual void Walk ( UnaryOperatorExpression unaryOperator )
        {
        }

        #endregion ITreeWalker
    }
}
