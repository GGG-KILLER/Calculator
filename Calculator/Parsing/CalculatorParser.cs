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
        private readonly ILexer<CalculatorTokenType> _lexer;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="lexer"></param>
        /// <param name="tokenReader"></param>
        /// <param name="prefixModules"></param>
        /// <param name="infixModules"></param>
        /// <param name="diagnostics"></param>
        protected internal CalculatorParser(
            ILexer<CalculatorTokenType> lexer,
            ITokenReader<CalculatorTokenType> tokenReader,
            PrattParserModuleTree<CalculatorTokenType, IPrefixParselet<CalculatorTokenType, CalculatorTreeNode>> prefixModules,
            PrattParserModuleTree<CalculatorTokenType, IInfixParselet<CalculatorTokenType, CalculatorTreeNode>> infixModules,
            DiagnosticList diagnostics)
            : base(tokenReader, prefixModules, infixModules, diagnostics) =>
            _lexer = lexer;

        /// <summary>
        /// Parses an expression
        /// </summary>
        /// <returns></returns>
        public CalculatorTreeNode Parse()
        {
            if (!TryParseExpression(out var expr))
                throw new FatalParsingException(_lexer.GetLocation(TokenReader.Lookahead().Range.Start), "Unable to parse this expression.");

            if (!TokenReader.Accept(CalculatorTokenType.EndOfExpression, out _))
                Diagnostics.Report(CalculatorDiagnostics.SyntaxError.ThingExpected(_lexer.GetLocation(TokenReader.Lookahead().Range.Start), "EOF"));

            return expr;
        }
    }
}
