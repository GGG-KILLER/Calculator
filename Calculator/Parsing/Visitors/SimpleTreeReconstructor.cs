using System;
using System.Linq;
using Calculator.Parsing.Abstractions;
using Calculator.Parsing.AST;

namespace Calculator.Parsing.Visitors
{
    /// <summary>
    /// A simple <see cref="CalculatorTreeNode" /> tree reconstructor that doesn't take into account operators precedences and stuff (mostly because it doesn't have any information on them)
    /// </summary>
    public class SimpleTreeReconstructor : ITreeVisitor<String>
    {
        /// <inheritdoc />
        public String Visit ( BinaryOperatorExpression binaryOperator ) =>
            $"({binaryOperator.LeftHandSide.Accept ( this )}) {binaryOperator.Operator.Raw} ({binaryOperator.RightHandSide.Accept ( this )})";

        /// <inheritdoc />
        public String Visit ( IdentifierExpression identifier ) =>
            identifier.Identifier.Raw;

        /// <inheritdoc />
        public String Visit ( FunctionCallExpression functionCall ) =>
            $"{functionCall.Identifier.Accept ( this )}({String.Join ( ", ", functionCall.Arguments.Select ( arg => arg.Accept ( this ) ) )})";

        /// <inheritdoc />
        public String Visit ( NumberExpression number ) => number.Value.Value.ToString ( );

        /// <inheritdoc />
        public String Visit ( UnaryOperatorExpression unaryOperator ) =>
            unaryOperator.OperatorFix == Definitions.UnaryOperatorFix.Prefix
                ? $"{unaryOperator.Operator.Raw}({unaryOperator.Operand.Accept ( this )})"
                : $"({unaryOperator.Operand.Accept ( this )}){unaryOperator.Operator.Raw}";

        /// <inheritdoc />
        public String Visit ( ImplicitMultiplicationExpression implicitMultiplication ) =>
            $"({implicitMultiplication.LeftHandSide.Accept ( this )})({implicitMultiplication.RightHandSide.Accept ( this )})";

        /// <inheritdoc />
        public String Visit ( GroupedExpression grouped ) =>
            $"({grouped.Inner.Accept ( this )})";

        /// <inheritdoc />
        public String Visit ( SuperscriptExponentiationExpression superscriptExponentiation ) =>
            $"({superscriptExponentiation.Base.Accept ( this )}){superscriptExponentiation.Exponent.Raw}";
    }
}