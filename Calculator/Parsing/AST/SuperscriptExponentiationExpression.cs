using System;
using Calculator.Lexing;
using Calculator.Parsing.Abstractions;
using GParse.Lexing;
using GParse.Math;

namespace Calculator.Parsing.AST
{
    /// <summary>
    /// Represents an exponentiation expression using superscript numbers
    /// </summary>
    public class SuperscriptExponentiationExpression : CalculatorTreeNode
    {
        /// <summary>
        /// The base of the exponentiation
        /// </summary>
        public CalculatorTreeNode Base { get; }

        /// <summary>
        /// The exponent
        /// </summary>
        public Token<CalculatorTokenType> Exponent { get; }

        /// <inheritdoc />
        public override Range<int> Range { get; }

        /// <summary>
        /// Initializes this <see cref="SuperscriptExponentiationExpression"/>
        /// </summary>
        /// <param name="base"></param>
        /// <param name="exponent"></param>
        public SuperscriptExponentiationExpression(CalculatorTreeNode @base, Token<CalculatorTokenType> exponent)
        {
            Base = @base ?? throw new ArgumentNullException(nameof(@base));
            Exponent = exponent;
            Range = new Range<int>(@base.Range.Start, exponent.Range.End);
        }

        /// <inheritdoc />
        public override void Accept(ITreeVisitor visitor)
        {
            if (visitor is null)
                throw new ArgumentNullException(nameof(visitor));

            visitor.Visit(this);
        }

        /// <inheritdoc />
        public override T Accept<T>(ITreeVisitor<T> visitor)
        {
            if (visitor is null)
                throw new ArgumentNullException(nameof(visitor));

            return visitor.Visit(this);
        }

        /// <inheritdoc />
        public override bool StructurallyEquals(CalculatorTreeNode node) =>
            node is SuperscriptExponentiationExpression other
            && Base.StructurallyEquals(other.Base)
            && Exponent.Value.Equals(other.Exponent.Value);
    }
}