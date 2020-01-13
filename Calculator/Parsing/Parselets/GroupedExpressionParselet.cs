using System;
using Calculator.Lexing;
using Calculator.Parsing.AST;
using GParse;
using GParse.Lexing;
using GParse.Parsing;
using GParse.Parsing.Parselets;

namespace Calculator.Parsing.Parselets
{
    /// <summary>
    /// The parselet responsible for attempting to parse <see cref="GroupedExpression" />
    /// </summary>
    public class GroupedExpressionParselet : IPrefixParselet<CalculatorTokenType, CalculatorTreeNode>
    {
        /// <summary>
        /// Attempts to parse a <see cref="GroupedExpression" />
        /// </summary>
        /// <param name="parser"></param>
        /// <param name="diagnosticEmitter"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public Boolean TryParse ( IPrattParser<CalculatorTokenType, CalculatorTreeNode> parser, IProgress<Diagnostic> diagnosticEmitter, out CalculatorTreeNode node )
        {
            if ( parser is null )
                throw new ArgumentNullException ( nameof ( parser ) );
            
            if ( diagnosticEmitter is null )
                throw new ArgumentNullException ( nameof ( diagnosticEmitter ) );

            if ( !parser.TokenReader.Accept ( CalculatorTokenType.LParen, out Token<CalculatorTokenType> lparen ) || !parser.TryParseExpression ( out CalculatorTreeNode expr ) )
            {
                node = null;
                return false;
            }

            if ( !parser.TokenReader.Accept ( CalculatorTokenType.RParen, out Token<CalculatorTokenType> rparen ) )
            {
                diagnosticEmitter.Report ( CalculatorDiagnostics.SyntaxError.ThingExpectedFor ( parser.TokenReader.Lookahead ( ).Range, "closing parenthesis", $"opening parenthesis at {lparen.Range.Start}" ) );
                rparen = ASTHelper.Token ( ")", CalculatorTokenType.RParen, ")" );
            }

            node = new GroupedExpression ( lparen, expr, rparen );
            return true;
        }
    }
}
