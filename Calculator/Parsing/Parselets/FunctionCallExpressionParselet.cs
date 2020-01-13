using System;
using System.Collections.Generic;
using Calculator.Lexing;
using Calculator.Parsing.AST;
using GParse;
using GParse.Lexing;
using GParse.Parsing;
using GParse.Parsing.Parselets;

namespace Calculator.Parsing.Parselets
{
    /// <summary>
    /// The parselet responsible for attempting to parse <see cref="FunctionCallExpression" />
    /// </summary>
    public class FunctionCallExpressionParselet : IInfixParselet<CalculatorTokenType, CalculatorTreeNode>
    {
        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public Int32 Precedence { get; }

        /// <summary>
        /// Initializes this <see cref="FunctionCallExpressionParselet" /> with the provided
        /// <paramref name="precedence" />
        /// </summary>
        /// <param name="precedence"></param>
        public FunctionCallExpressionParselet ( Int32 precedence )
        {
            this.Precedence = precedence;
        }

        /// <summary>
        /// Attempts to parse a <see cref="FunctionCallExpression" />
        /// </summary>
        /// <param name="parser"></param>
        /// <param name="identifier"></param>
        /// <param name="diagnosticEmitter"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public Boolean TryParse ( IPrattParser<CalculatorTokenType, CalculatorTreeNode> parser, CalculatorTreeNode identifier, IProgress<Diagnostic> diagnosticEmitter, out CalculatorTreeNode node )
        {
            if ( parser is null )
                throw new ArgumentNullException ( nameof ( parser ) );

            if ( identifier is null )
                throw new ArgumentNullException ( nameof ( identifier ) );

            if ( diagnosticEmitter is null )
                throw new ArgumentNullException ( nameof ( diagnosticEmitter ) );

            ITokenReader<CalculatorTokenType> reader = parser.TokenReader;
            if ( !( identifier is IdentifierExpression ) || !reader.Accept ( CalculatorTokenType.LParen, out Token<CalculatorTokenType> lparen ) )
            {
                node = null;
                return false;
            }

            var toks = new List<Token<CalculatorTokenType>>
            {
                lparen
            };
            Token<CalculatorTokenType> rparen;
            var args = new List<CalculatorTreeNode> ( );
            while ( !reader.Accept ( CalculatorTokenType.RParen, out rparen ) )
            {
                if ( !parser.TryParseExpression ( out CalculatorTreeNode expr ) )
                {
                    diagnosticEmitter.Report ( CalculatorDiagnostics.SyntaxError.ThingExpected ( reader.Location, "argument" ) );
                    rparen = ASTHelper.Token ( ")", CalculatorTokenType.RParen, ")" );
                    break;
                }

                args.Add ( expr );

                if ( reader.Accept ( CalculatorTokenType.Comma, out Token<CalculatorTokenType> comma ) )
                {
                    toks.Add ( comma );
                }
                else if ( reader.Accept ( CalculatorTokenType.RParen, out rparen ) )
                {
                    break;
                }
                else
                {
                    diagnosticEmitter.Report ( CalculatorDiagnostics.SyntaxError.ThingExpectedAfter ( reader.Location, "')'", "argument list" ) );
                    rparen = ASTHelper.Token ( ")", CalculatorTokenType.RParen, ")" );
                    break;
                }
            }
            toks.Add ( rparen );

            node = new FunctionCallExpression ( identifier as IdentifierExpression, args, toks );
            return true;
        }
    }
}
