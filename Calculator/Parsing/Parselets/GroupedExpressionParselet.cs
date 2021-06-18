using System;
using Calculator.Lexing;
using Calculator.Parsing;
using GParse;
using GParse.Parsing;
using GParse.Parsing.Parselets;
using Tsu;

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
        /// <returns></returns>
        public Option<CalculatorTreeNode> Parse(
            IPrattParser<CalculatorTokenType, CalculatorTreeNode> parser,
            DiagnosticList diagnostics)
        {
            if (parser is null)
                throw new ArgumentNullException(nameof(parser));

            if (diagnostics is null)
                throw new ArgumentNullException(nameof(diagnostics));

            if (!parser.TokenReader.Accept(CalculatorTokenType.LParen, out var lparen)
                || parser.ParseExpression() is not { IsSome: true, Value: var expr })
            {
                return Option.None<CalculatorTreeNode>();
            }

            if (!parser.TokenReader.Accept(CalculatorTokenType.RParen, out var rparen))
            {
                var errorRange = parser.TokenReader.Lookahead().Range;
                diagnostics.Report(CalculatorDiagnostics.SyntaxError.ThingExpectedFor(errorRange, "closing parenthesis", $"opening parenthesis at {lparen.Range.Start}"));
                rparen = ASTHelper.Token(")", CalculatorTokenType.RParen, ")", range: (errorRange.Start, errorRange.Start));
            }

            return new GroupedExpression(lparen, expr, rparen);
        }
    }
}