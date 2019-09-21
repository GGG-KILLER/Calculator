using System;
using Calculator.Lexing;
using Calculator.Parsing.Abstractions;
using GParse;
using GParse.Lexing;

namespace Calculator.Parsing.AST
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
        public override SourceRange Range => this.Value.Range;

        /// <summary>
        /// Initializes this <see cref="NumberExpression" />
        /// </summary>
        /// <param name="value"></param>
        public NumberExpression ( Token<CalculatorTokenType> value )
        {
            this.Value = value;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="visitor"></param>
        public override void Accept ( ITreeVisitor visitor ) => visitor.Visit ( this );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="visitor"></param>
        /// <returns></returns>
        public override T Accept<T> ( ITreeVisitor<T> visitor ) => visitor.Visit ( this );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public override Boolean StructurallyEquals ( CalculatorTreeNode node ) =>
            node is NumberExpression numberExpression
                && this.Value.Value.Equals ( numberExpression.Value.Value );
    }
}
