using System;
using Calculator.Lib.Abstractions;
using Calculator.Lib.AST;

namespace Calculator.Lib.Visitors
{
    public class MathExpressionReconstructor : ICNodeTreeVisitor<String>
    {
        public String Visit ( BinaryOperatorExpression binaryOperator )
            => $"({binaryOperator.LeftHandSide.Accept ( this )} {binaryOperator.Operator} {binaryOperator.RightHandSide.Accept ( this )})";

        public String Visit ( ConstantExpression constant )
            => constant.Identifier;

        public String Visit ( FunctionCallExpression functionCall )
            => $"{functionCall.Identifier} ( {String.Join ( ", ", Array.ConvertAll ( functionCall.Arguments, node => node.Accept ( this ) ) )} )";

        public String Visit ( NumberExpression number )
            => number.Value.ToString ( );

        public String Visit ( ParenthesisExpression parenthesis )
            => $"({parenthesis.InnerExpression.Accept ( this )})";

        public String Visit ( UnaryOperatorExpression unaryOperator )
            => unaryOperator.Fix == Definitions.UnaryOperatorFix.Postfix
                ? $"{unaryOperator.Operand.Accept ( this )}{unaryOperator.Operator}"
                : $"{unaryOperator.Operator}{unaryOperator.Operand.Accept ( this )}";
    }
}
