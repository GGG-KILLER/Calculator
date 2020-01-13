using System;
using Calculator.Lexing;
using Calculator.Parsing.Abstractions;
using GParse;
using GParse.Lexing;

namespace Calculator.Parsing.AST
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
        public override SourceRange Range => this.Identifier.Range;

        /// <summary>
        /// Initializes this <see cref="IdentifierExpression" />
        /// </summary>
        /// <param name="identifier"></param>
        public IdentifierExpression ( Token<CalculatorTokenType> identifier )
        {
            this.Identifier = identifier;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="visitor"></param>
        public override void Accept ( ITreeVisitor visitor )
        {
            if ( visitor is null )
                throw new ArgumentNullException ( nameof ( visitor ) );

            visitor.Visit ( this );
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="visitor"></param>
        /// <returns></returns>
        public override T Accept<T> ( ITreeVisitor<T> visitor )
        {
            if ( visitor is null )
                throw new ArgumentNullException ( nameof ( visitor ) );

            return visitor.Visit ( this );
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public override Boolean StructurallyEquals ( CalculatorTreeNode node ) =>
            node is IdentifierExpression identifierExpression
                && this.Identifier.Raw.Equals ( identifierExpression.Identifier.Raw, StringComparison.OrdinalIgnoreCase );
    }
}
