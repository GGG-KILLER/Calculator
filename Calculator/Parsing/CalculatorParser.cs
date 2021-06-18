using Calculator.Lexing;
using GParse;
using GParse.Errors;
using GParse.Lexing;
using GParse.Parsing;
using GParse.Parsing.Parselets;

namespace Calculator.Parsing
{
    /// <summary>
    /// The parser used by the calculator
    /// </summary>
    public class CalculatorParser : PrattParser<CalculatorTokenType, CalculatorTreeNode>
    {
        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="tokenReader"></param>
        /// <param name="prefixModules"></param>
        /// <param name="infixModules"></param>
        /// <param name="diagnostics"></param>
        protected internal CalculatorParser(
            ITokenReader<CalculatorTokenType> tokenReader,
            PrattParserModuleTree<CalculatorTokenType, IPrefixParselet<CalculatorTokenType, CalculatorTreeNode>> prefixModules,
            PrattParserModuleTree<CalculatorTokenType, IInfixParselet<CalculatorTokenType, CalculatorTreeNode>> infixModules,
            DiagnosticList diagnostics)
            : base(tokenReader, prefixModules, infixModules, diagnostics)
        {
        }

        /// <summary>
        /// Parses an expression
        /// </summary>
        /// <returns></returns>
        public CalculatorTreeNode Parse()
        {
            var expr = ParseExpression().UnwrapOrElse(() => throw new FatalParsingException(TokenReader.Lookahead().Range, "Unable to parse this expression."));

            if (!TokenReader.Accept(CalculatorTokenType.EndOfExpression, out _))
                Diagnostics.Report(CalculatorDiagnostics.SyntaxError.ThingExpected(TokenReader.Lookahead().Range, "EOF"));

            return expr;
        }
    }
}