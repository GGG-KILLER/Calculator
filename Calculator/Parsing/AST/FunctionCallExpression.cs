using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Calculator.Lexing;
using Calculator.Parsing.Abstractions;
using GParse;
using GParse.Lexing;

namespace Calculator.Parsing.AST
{
    /// <summary>
    /// Represents a function call
    /// </summary>
    public class FunctionCallExpression : CalculatorTreeNode
    {
        /// <summary>
        /// The function identifier
        /// </summary>
        public IdentifierExpression Identifier { get; }

        /// <summary>
        /// The arguments provided for the function call
        /// </summary>
        public ImmutableArray<CalculatorTreeNode> Arguments { get; }

        /// <summary>
        /// The tokens that make up the function call
        /// </summary>
        public IEnumerable<Token<CalculatorTokenType>> Tokens { get; }

        /// <inheritdoc />
        public override SourceRange Range { get; }

        /// <summary>
        /// Initializes this <see cref="FunctionCallExpression"/>
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="arguments"></param>
        /// <param name="tokens"></param>
        public FunctionCallExpression ( IdentifierExpression identifier, IEnumerable<CalculatorTreeNode> arguments, IEnumerable<Token<CalculatorTokenType>> tokens )
        {
            this.Identifier = identifier;
            this.Arguments = arguments.ToImmutableArray ( );
            this.Tokens = tokens;
            this.Range = identifier.Range.Start.To ( tokens.Last ( ).Range.End );
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
        public override Boolean StructurallyEquals ( CalculatorTreeNode node )
        {
            if ( !( node is FunctionCallExpression functionCall )
                 || !this.Identifier.StructurallyEquals ( functionCall.Identifier )
                 || this.Arguments.Length != functionCall.Arguments.Length )
            {
                return false;
            }

            for ( var i = 0; i < this.Arguments.Length; i++ )
            {
                if ( !this.Arguments[i].StructurallyEquals ( functionCall.Arguments[i] ) )
                    return false;
            }

            return true;
        }
    }
}
