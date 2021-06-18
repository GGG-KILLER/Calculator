using System;
using System.Collections.Generic;
using Calculator.Lexing;
using Calculator.Parsing;
using GParse;
using GParse.Lexing;
using GParse.Parsing;
using GParse.Parsing.Parselets;
using Tsu;

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
        public int Precedence { get; }

        /// <summary>
        /// Initializes this <see cref="FunctionCallExpressionParselet" /> with the provided
        /// <paramref name="precedence" />
        /// </summary>
        /// <param name="precedence"></param>
        public FunctionCallExpressionParselet(int precedence) => Precedence = precedence;

        /// <summary>
        /// Attempts to parse a <see cref="FunctionCallExpression" />
        /// </summary>
        /// <param name="parser"></param>
        /// <param name="identifier"></param>
        /// <param name="diagnostics"></param>
        /// <returns></returns>
        public Option<CalculatorTreeNode> Parse(
            IPrattParser<CalculatorTokenType, CalculatorTreeNode> parser,
            CalculatorTreeNode identifier,
            DiagnosticList diagnostics)
        {
            if (parser is null)
                throw new ArgumentNullException(nameof(parser));

            if (identifier is null)
                throw new ArgumentNullException(nameof(identifier));

            if (diagnostics is null)
                throw new ArgumentNullException(nameof(diagnostics));

            var reader = parser.TokenReader;
            if (!(identifier is IdentifierExpression) || !reader.Accept(CalculatorTokenType.LParen, out var lparen))
                return Option.None<CalculatorTreeNode>();

            var toks = new List<Token<CalculatorTokenType>>
            {
                lparen
            };
            Token<CalculatorTokenType> rparen;
            var args = new List<CalculatorTreeNode>();
            while (!reader.Accept(CalculatorTokenType.RParen, out rparen))
            {
                if (parser.ParseExpression() is not { IsSome: true, Value: var expr })
                {
                    diagnostics.Report(CalculatorDiagnostics.SyntaxError.ThingExpected(reader.Lookahead().Range, "argument"));
                    rparen = ASTHelper.Token(")", CalculatorTokenType.RParen, ")");
                    break;
                }

                args.Add(expr);

                if (reader.Accept(CalculatorTokenType.Comma, out var comma))
                {
                    toks.Add(comma);
                }
                else if (reader.Accept(CalculatorTokenType.RParen, out rparen))
                {
                    break;
                }
                else
                {
                    diagnostics.Report(CalculatorDiagnostics.SyntaxError.ThingExpectedAfter(reader.Lookahead().Range, "')'", "argument list"));
                    rparen = ASTHelper.Token(")", CalculatorTokenType.RParen, ")");
                    break;
                }
            }
            toks.Add(rparen);

            return new FunctionCallExpression(identifier as IdentifierExpression, args, toks);
        }
    }
}