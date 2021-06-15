using Calculator.Lexing;
using Calculator.Parsing.AST;
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
        /// <param name="positionContainer"></param>
        /// <param name="tokenReader"></param>
        /// <param name="prefixModules"></param>
        /// <param name="infixModules"></param>
        /// <param name="diagnostics"></param>
        protected internal CalculatorParser(
            IPositionContainer positionContainer,
            ITokenReader<CalculatorTokenType> tokenReader,
            PrattParserModuleTree<CalculatorTokenType, IPrefixParselet<CalculatorTokenType, CalculatorTreeNode>> prefixModules,
            PrattParserModuleTree<CalculatorTokenType, IInfixParselet<CalculatorTokenType, CalculatorTreeNode>> infixModules,
            DiagnosticList diagnostics)
            : base(tokenReader, prefixModules, infixModules, diagnostics) =>
            PositionContainer = positionContainer;

        /// <summary>
        /// The position container to use when resolving diagnostic locations.
        /// </summary>
        public IPositionContainer PositionContainer { get; }

        /// <summary>
        /// Parses an expression
        /// </summary>
        /// <returns></returns>
        public CalculatorTreeNode Parse()
        {
            if (!TryParseExpression(out var expr))
                throw new FatalParsingException(PositionContainer.GetLocation(TokenReader.Lookahead().Range.Start), "Unable to parse this expression.");

            if (!TokenReader.Accept(CalculatorTokenType.EndOfExpression, out _))
                Diagnostics.Report(CalculatorDiagnostics.SyntaxError.ThingExpected(PositionContainer.GetLocation(TokenReader.Lookahead().Range.Start), "EOF"));

            return expr;
        }
    }
}