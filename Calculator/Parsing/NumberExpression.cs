using System;
using Calculator.Lexing;
using GParse.Lexing;
using GParse.Math;

namespace Calculator.Parsing
{
    /// <summary>
    /// Represents a number
    /// </summary>
    public class NumberExpression : CalculatorTreeNode
    {
        /// <summary>
        /// The value of the expression
        /// </summary>
        public Token<CalculatorTokenType> Value { get; }

        /// <inheritdoc />
        public override Range<int> Range => Value.Range;

        /// <summary>
        /// Initializes this <see cref="NumberExpression" />
        /// </summary>
        /// <param name="value"></param>
        public NumberExpression(Token<CalculatorTokenType> value) => Value = value;

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
            node is NumberExpression numberExpression
            && Value.Value.Equals(numberExpression.Value.Value);
    }
}