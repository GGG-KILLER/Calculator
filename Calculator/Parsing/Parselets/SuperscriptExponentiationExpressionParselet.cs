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
    /// Represents an exponentiation through superscript expression
    /// </summary>
    public class SuperscriptExponentiationExpressionParselet : IInfixParselet<CalculatorTokenType, CalculatorTreeNode>
    {
        /// <summary>
        /// Initializes this <see cref="SuperscriptExponentiationExpressionParselet"/>
        /// </summary>
        /// <param name="precedence"></param>
        public SuperscriptExponentiationExpressionParselet(int precedence) =>
            Precedence = precedence;

        /// <inheritdoc />
        public int Precedence { get; }

        /// <inheritdoc />
        public Option<CalculatorTreeNode> Parse(
            IPrattParser<CalculatorTokenType, CalculatorTreeNode> parser,
            CalculatorTreeNode @base,
            DiagnosticList diagnostics)
        {
            if (parser is null)
                throw new ArgumentNullException(nameof(parser));

            if (@base is null)
                throw new ArgumentNullException(nameof(@base));

            if (diagnostics is null)
                throw new ArgumentNullException(nameof(diagnostics));

            var reader = parser.TokenReader;
            if (reader.Accept(CalculatorTokenType.Superscript, out var exponent))
                return new SuperscriptExponentiationExpression(@base, exponent);

            return Option.None<CalculatorTreeNode>();
        }
    }
}