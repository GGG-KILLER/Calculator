using Calculator.Lib.AST;

namespace Calculator.Lib.Abstractions
{
    public interface ICNodeTreeVisitor
    {
        void Visit ( BinaryOperatorExpression binaryOperator );

        void Visit ( ConstantExpression constant );

        void Visit ( FunctionCallExpression functionCall );

        void Visit ( NumberExpression number );

        void Visit ( ParenthesisExpression parenthesis );

        void Visit ( UnaryOperatorExpression unaryOperator );
    }
}
