using Calculator.Parsing.AST;

namespace Calculator.Parsing.Abstractions
{
    public interface ITreeVisitor
    {
        void Visit ( BinaryOperatorExpression binaryOperator );

        void Visit ( IdentifierExpression identifier );

        void Visit ( FunctionCallExpression functionCall );

        void Visit ( NumberExpression number );

        void Visit ( UnaryOperatorExpression unaryOperator );
    }

    public interface ITreeVisitor<T>
    {
        T Visit ( BinaryOperatorExpression binaryOperator );

        T Visit ( IdentifierExpression identifier );

        T Visit ( FunctionCallExpression functionCall );

        T Visit ( NumberExpression number );

        T Visit ( UnaryOperatorExpression unaryOperator );
    }
}
