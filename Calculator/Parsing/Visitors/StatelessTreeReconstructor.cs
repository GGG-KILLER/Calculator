using System;
using System.Linq;
using Calculator.Parsing.Abstractions;
using Calculator.Parsing.AST;

namespace Calculator.Parsing.Visitors
{
    /// <summary>
    /// A stateless <see cref="CalculatorTreeNode" /> tree reconstructor
    /// </summary>
    public class StatelessTreeReconstructor : ITreeVisitor<String>
    {
        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="binaryOperator"></param>
        /// <returns></returns>
        public String Visit ( BinaryOperatorExpression binaryOperator ) =>
            $"({binaryOperator.LeftHandSide.Accept ( this )}) {binaryOperator.Operator.Raw} ({binaryOperator.RightHandSide.Accept ( this )})";

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public String Visit ( IdentifierExpression identifier ) =>
            identifier.Identifier.Raw;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="functionCall"></param>
        /// <returns></returns>
        public String Visit ( FunctionCallExpression functionCall ) =>
            $"{functionCall.Identifier.Accept ( this )}({String.Join ( ", ", functionCall.Arguments.Select ( arg => arg.Accept ( this ) ) )})";

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public String Visit ( NumberExpression number ) => number.Value.Value.ToString ( );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="unaryOperator"></param>
        /// <returns></returns>
        public String Visit ( UnaryOperatorExpression unaryOperator ) =>
            unaryOperator.OperatorFix == Definitions.UnaryOperatorFix.Prefix
                ? $"{unaryOperator.Operator.Raw}({unaryOperator.Operand.Accept ( this )})"
                : $"({unaryOperator.Operand.Accept ( this )}){unaryOperator.Operator.Raw}";

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="implicitMultiplication"></param>
        /// <returns></returns>
        public String Visit ( ImplicitMultiplicationExpression implicitMultiplication ) =>
            $"({implicitMultiplication.LeftHandSide.Accept ( this )})({implicitMultiplication.RightHandSide.Accept ( this )})";

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="grouped"></param>
        /// <returns></returns>
        public String Visit ( GroupedExpression grouped ) =>
            $"({grouped.Inner.Accept ( this )})";
    }
}
