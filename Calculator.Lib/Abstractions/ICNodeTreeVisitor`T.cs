using Calculator.Lib.AST;

namespace Calculator.Lib.Abstractions
{
    public interface ICNodeTreeVisitor<T>
    {
        T Visit ( BinaryOperatorExpression binaryOperator );

        T Visit ( ConstantExpression constant );

        T Visit ( FunctionCallExpression functionCall );

        T Visit ( NumberExpression number );

        T Visit ( ParenthesisExpression parenthesis );

        T Visit ( UnaryOperatorExpression unaryOperator );
    }
}
