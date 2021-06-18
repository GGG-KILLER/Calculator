using System;
using Calculator.Lexing;
using GParse.Lexing;
using GParse.Math;

namespace Calculator.Parsing
{
    /// <summary>
    /// Represents an identifier
    /// </summary>
    public class IdentifierExpression : CalculatorTreeNode
    {
        /// <summary>
        /// The identifier itself
        /// </summary>
        public Token<CalculatorTokenType> Identifier { get; }

        /// <inheritdoc />
        public override Range<int> Range => Identifier.Range;

        /// <summary>
        /// Initializes this <see cref="IdentifierExpression" />
        /// </summary>
        /// <param name="identifier"></param>
        public IdentifierExpression(Token<CalculatorTokenType> identifier) =>
            Identifier = identifier;

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
            node is IdentifierExpression identifierExpression
            && StringComparer.OrdinalIgnoreCase.Equals(Identifier.Text, identifierExpression.Identifier.Text);
    }
}