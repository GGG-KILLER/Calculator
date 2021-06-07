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
        /// <param name="diagnostics"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool TryParse(
            IPrattParser<CalculatorTokenType, CalculatorTreeNode> parser,
            DiagnosticList diagnostics,
            out CalculatorTreeNode node)
        {
            if (parser is null)
                throw new ArgumentNullException(nameof(parser));

            if (diagnostics is null)
                throw new ArgumentNullException(nameof(diagnostics));

            if (!parser.TokenReader.Accept(CalculatorTokenType.LParen, out var lparen) || !parser.TryParseExpression(out var expr))
            {
                node = null;
                return false;
            }

            if (!parser.TokenReader.Accept(CalculatorTokenType.RParen, out var rparen))
            {
                var errorRange = ((CalculatorParser) parser).PositionContainer.GetLocation(parser.TokenReader.Lookahead().Range);
                diagnostics.Report(CalculatorDiagnostics.SyntaxError.ThingExpectedFor(errorRange, "closing parenthesis", $"opening parenthesis at {lparen.Range.Start}"));
                rparen = ASTHelper.Token(")", CalculatorTokenType.RParen, ")");
            }

            node = new GroupedExpression(lparen, expr, rparen);
            return true;
        }
    }
}
