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
        /// <returns></returns>
        public Option<CalculatorTreeNode> Parse(
            IPrattParser<CalculatorTokenType, CalculatorTreeNode> parser,
            CalculatorTreeNode expression,
            DiagnosticList diagnostics)
        {
            if (parser is null)
                throw new ArgumentNullException(nameof(parser));

            if (expression is null)
                throw new ArgumentNullException(nameof(expression));

            if (diagnostics is null)
                throw new ArgumentNullException(nameof(diagnostics));

            if (parser.ParseExpression(Precedence) is { IsSome: true, Value: var right })
                return new ImplicitMultiplicationExpression(expression, right);

            return Option.None<CalculatorTreeNode>();
        }
    }
}