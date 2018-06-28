using System;
using Calculator.Lib.Abstractions;
using Calculator.Lib.AST;
using Calculator.Lib.Definitions;
using Calculator.Lib.Exceptions;

namespace Calculator.Lib.Visitors
{
    public class CNodeTreeExecutor : ICNodeTreeVisitor<Double>
    {
        private readonly CalculatorLang Language;

        public CNodeTreeExecutor ( CalculatorLang lang )
        {
            this.Language = lang;
        }

        public Double Visit ( BinaryOperatorExpression binaryOperator )
            => this.Language.HasBinaryOperator ( binaryOperator.Operator )
                ? this.Language.GetBinaryOperator ( binaryOperator.Operator )
                    .Action ( binaryOperator.LeftHandSide.Accept ( this ), binaryOperator.RightHandSide.Accept ( this ) )
                : throw new CalculatorRuntimeException ( $"Binary operator {binaryOperator.Operator} does not exist in this language." );

        public Double Visit ( ConstantExpression constant )
            => this.Language.HasConstant ( constant.Identifier )
                ? this.Language.GetConstant ( constant.Identifier ).Value
                : throw new CalculatorRuntimeException ( $"Constant {constant.Identifier} does not exist in this language." );

        public Double Visit ( FunctionCallExpression functionCall )
            => this.Language.HasFunction ( functionCall.Identifier )
                ? ( Double ) this.Language.GetFunction ( functionCall.Identifier )
                    .DynamicInvoke ( Array.ConvertAll ( functionCall.Arguments, node => node.Accept ( this ) ) )
                : throw new CalculatorRuntimeException ( $"Function {functionCall.Identifier} does not exist in this language." );

        public Double Visit ( NumberExpression number )
            => number.Value;

        public Double Visit ( ParenthesisExpression parenthesis )
            => parenthesis.InnerExpression.Accept ( this );

        public Double Visit ( UnaryOperatorExpression unaryOperator )
            => this.Language.HasUnaryOperator ( unaryOperator.Operator )
                ? this.Language.GetUnaryOperator ( unaryOperator.Operator )
                    .Action ( unaryOperator.Operand.Accept ( this ) )
                : throw new CalculatorRuntimeException ( $"Unary operator {unaryOperator.Operator} does not exist in this language." );
    }
}
