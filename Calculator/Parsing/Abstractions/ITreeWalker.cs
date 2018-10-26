using Calculator.Parsing.AST;

namespace Calculator.Parsing.Abstractions
{
    public interface ITreeWalker
    {
        void Walk ( BinaryOperatorExpression binaryOperator );

        void Walk ( IdentifierExpression identifier );

        void Walk ( FunctionCallExpression functionCall );

        void Walk ( NumberExpression number );

        void Walk ( UnaryOperatorExpression unaryOperator );
    }

    public interface ITreeWalker<T>
    {
        T Walk ( BinaryOperatorExpression binaryOperator );

        T Walk ( IdentifierExpression identifier );

        T Walk ( FunctionCallExpression functionCall );

        T Walk ( NumberExpression number );

        T Walk ( UnaryOperatorExpression unaryOperator );
    }
}
