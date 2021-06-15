using System;
using Calculator.Definitions;
using Calculator.Lexing;
using Calculator.Parsing.Abstractions;
using GParse.Lexing;
using GParse.Math;

namespace Calculator.Parsing.AST
{
    /// <summary>
    /// Represents a prefix/postfix operation
    /// </summary>
    public class UnaryOperatorExpression : CalculatorTreeNode
    {
        /// <summary>
        /// The operator's location
        /// </summary>
        public UnaryOperatorFix OperatorFix { get; }

        /// <summary>
        /// The operator itself
        /// </summary>
        public Token<CalculatorTokenType> Operator { get; }

        /// <summary>
        /// The operand expression
        /// </summary>
        public CalculatorTreeNode Operand { get; }

        /// <inheritdoc />
        public override Range<int> Range { get; }

        /// <summary>
        /// Initializes this <see cref="UnaryOperatorExpression" />
        /// </summary>
        /// <param name="fix"></param>
        /// <param name="operator"></param>
        /// <param name="operand"></param>
        public UnaryOperatorExpression(UnaryOperatorFix fix, Token<CalculatorTokenType> @operator, CalculatorTreeNode operand)
        {
            OperatorFix = fix;
            Operator = @operator;
            Operand = operand ?? throw new ArgumentNullException(nameof(operand));

            if (fix == UnaryOperatorFix.Postfix)
                Range = new Range<int>(operand.Range.Start, @operator.Range.End);
            else
                Range = new Range<int>(@operator.Range.Start, operand.Range.End);
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="visitor"></param>
        public override void Accept(ITreeVisitor visitor)
        {
            if (visitor is null)
                throw new ArgumentNullException(nameof(visitor));

            visitor.Visit(this);
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="visitor"></param>
        /// <returns></returns>
        public override T Accept<T>(ITreeVisitor<T> visitor)
        {
            if (visitor is null)
                throw new ArgumentNullException(nameof(visitor));

            return visitor.Visit(this);
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public override bool StructurallyEquals(CalculatorTreeNode node) =>
            node is UnaryOperatorExpression unaryOperatorExpression
            && StringComparer.OrdinalIgnoreCase.Equals(Operator.Text, unaryOperatorExpression.Operator.Text)
            && OperatorFix.Equals(unaryOperatorExpression.OperatorFix)
            && Operand.StructurallyEquals(unaryOperatorExpression.Operand);
    }
}