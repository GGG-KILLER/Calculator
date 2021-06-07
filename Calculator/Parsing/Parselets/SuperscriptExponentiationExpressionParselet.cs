using System;
using Calculator.Lexing;
using Calculator.Parsing.AST;
using GParse;
using GParse.Parsing;
using GParse.Parsing.Parselets;

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
        public SuperscriptExponentiationExpressionParselet(int precedence) => Precedence = precedence;

        /// <inheritdoc />
        public int Precedence { get; }

        /// <inheritdoc />
        public bool TryParse(
            IPrattParser<CalculatorTokenType, CalculatorTreeNode> parser,
            CalculatorTreeNode @base,
            DiagnosticList diagnostics,
            out CalculatorTreeNode parsedExpression)
        {
            if (parser is null)
                throw new ArgumentNullException(nameof(parser));

            if (@base is null)
                throw new ArgumentNullException(nameof(@base));

            if (diagnostics is null)
                throw new ArgumentNullException(nameof(diagnostics));

            var reader = parser.TokenReader;
            if (!reader.Accept(CalculatorTokenType.Superscript, out var exponent))
            {
                parsedExpression = null;
                return false;
            }

            parsedExpression = new SuperscriptExponentiationExpression(@base, exponent);
            return true;
        }
    }
}