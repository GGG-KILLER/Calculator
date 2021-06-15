using System;
using Calculator.Lexing;
using Calculator.Parsing.AST;
using GParse;
using GParse.Parsing;
using GParse.Parsing.Parselets;

namespace Calculator.Parsing.Parselets
{
    /// <summary>
    /// The parselet responsible for attempting to parse <see cref="ImplicitMultiplicationExpression" />
    /// </summary>
    public class ImplicitMultiplicationExpressionParselet : IInfixParselet<CalculatorTokenType, CalculatorTreeNode>
    {
        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public int Precedence { get; }

        /// <summary>
        /// Initializes this <see cref="ImplicitMultiplicationExpressionParselet" />
        /// </summary>
        /// <param name="precedence"></param>
        public ImplicitMultiplicationExpressionParselet(int precedence) => Precedence = precedence;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="parser"></param>
        /// <param name="expression"></param>
        /// <param name="diagnostics"></param>
        /// <param name="parsedExpression"></param>
        /// <returns></returns>
        public bool TryParse(
            IPrattParser<CalculatorTokenType, CalculatorTreeNode> parser,
            CalculatorTreeNode expression,
            DiagnosticList diagnostics,
            out CalculatorTreeNode parsedExpression)
        {
            if (parser is null)
                throw new ArgumentNullException(nameof(parser));

            if (expression is null)
                throw new ArgumentNullException(nameof(expression));

            if (diagnostics is null)
                throw new ArgumentNullException(nameof(diagnostics));

            if (parser.TryParseExpression(Precedence, out var right))
            {
                parsedExpression = new ImplicitMultiplicationExpression(expression, right);
                return true;
            }

            parsedExpression = null;
            return false;
        }
    }
}