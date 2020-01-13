using System;
using Calculator.Lexing;
using Calculator.Parsing.Abstractions;
using GParse;
using GParse.Lexing;

namespace Calculator.Parsing.AST
{
    /// <summary>
    /// Represents a parenthesized expression
    /// </summary>
    public class GroupedExpression : CalculatorTreeNode
    {
        /// <summary>
        /// The left parenthesis
        /// </summary>
        public Token<CalculatorTokenType> LParen { get; }

        /// <summary>
        /// The right parenthesis
        /// </summary>
        public Token<CalculatorTokenType> RParen { get; }

        /// <summary>
        /// The inner expression
        /// </summary>
        public CalculatorTreeNode Inner { get; }

        /// <inheritdoc />
        public override SourceRange Range { get; }

        /// <summary>
        /// Initializes this <see cref="GroupedExpression" />
        /// </summary>
        /// <param name="lparen"></param>
        /// <param name="inner"></param>
        /// <param name="rparen"></param>
        public GroupedExpression ( Token<CalculatorTokenType> lparen, CalculatorTreeNode inner, Token<CalculatorTokenType> rparen )
        {
            this.LParen = lparen;
            this.Inner = inner;
            this.RParen = rparen;
            this.Range = lparen.Range.Start.To ( rparen.Range.End );
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
            node is GroupedExpression grouped
            && this.LParen.Raw.Equals ( grouped.LParen.Raw, StringComparison.OrdinalIgnoreCase )
            && this.Inner.StructurallyEquals ( grouped.Inner )
            && this.RParen.Raw.Equals ( grouped.RParen.Raw, StringComparison.OrdinalIgnoreCase );
    }
}
