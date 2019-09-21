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
    /// Represents an exponentiation through superscript expression
    /// </summary>
    public class SuperscriptExponentiationExpressionParselet : IInfixParselet<CalculatorTokenType, CalculatorTreeNode>
    {
        /// <summary>
        /// Initializes this <see cref="SuperscriptExponentiationExpressionParselet"/>
        /// </summary>
        /// <param name="precedence"></param>
        public SuperscriptExponentiationExpressionParselet ( Int32 precedence )
        {
            this.Precedence = precedence;
        }

        /// <inheritdoc />
        public Int32 Precedence { get; }

        /// <inheritdoc />
        public Boolean TryParse ( IPrattParser<CalculatorTokenType, CalculatorTreeNode> parser, CalculatorTreeNode @base, IProgress<Diagnostic> diagnosticEmitter, out CalculatorTreeNode parsedExpression )
        {
            ITokenReader<CalculatorTokenType> reader = parser.TokenReader;
            if ( !reader.Accept ( CalculatorTokenType.Superscript, out Token<CalculatorTokenType> exponent ) )
            {
                parsedExpression = null;
                return false;
            }

            parsedExpression = new SuperscriptExponentiationExpression ( @base, exponent );
            return true;
        }
    }
}