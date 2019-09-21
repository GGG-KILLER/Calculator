using System;
using Calculator.Lexing;
using Calculator.Parsing.Abstractions;
using GParse;
using GParse.Lexing;

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
        public override SourceRange Range { get; }

        /// <summary>
        /// Initializes this <see cref="SuperscriptExponentiationExpression"/>
        /// </summary>
        /// <param name="base"></param>
        /// <param name="exponent"></param>
        public SuperscriptExponentiationExpression ( CalculatorTreeNode @base, Token<CalculatorTokenType> exponent )
        {
            this.Base = @base;
            this.Exponent = exponent;
            this.Range = @base.Range.Start.To ( exponent.Range.End );
        }

        /// <inheritdoc />
        public override void Accept ( ITreeVisitor visitor ) => visitor.Visit ( this );

        /// <inheritdoc />
        public override T Accept<T> ( ITreeVisitor<T> visitor ) => visitor.Visit ( this );

        /// <inheritdoc />
        public override Boolean StructurallyEquals ( CalculatorTreeNode node ) =>
            node is SuperscriptExponentiationExpression other
            && this.Base.StructurallyEquals ( other.Base )
            && this.Exponent.Raw.Equals ( other.Exponent.Raw );
    }
}