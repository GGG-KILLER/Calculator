using System;
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
        /// <param name="tokenReader"></param>
        /// <param name="prefixModules"></param>
        /// <param name="infixModules"></param>
        /// <param name="diagnosticEmitter"></param>
        protected internal CalculatorParser (
            ITokenReader<CalculatorTokenType> tokenReader,
            PrattParserModuleTree<CalculatorTokenType, IPrefixParselet<CalculatorTokenType, CalculatorTreeNode>> prefixModules,
            PrattParserModuleTree<CalculatorTokenType, IInfixParselet<CalculatorTokenType, CalculatorTreeNode>> infixModules,
            IProgress<Diagnostic> diagnosticEmitter ) : base ( tokenReader, prefixModules, infixModules, diagnosticEmitter )
        {
        }

        /// <summary>
        /// Parses an expression
        /// </summary>
        /// <returns></returns>
        public CalculatorTreeNode Parse ( )
        {
            if ( !this.TryParseExpression ( out CalculatorTreeNode expr ) )
                throw new FatalParsingException ( this.TokenReader.Location, "Unable to parse this expression." );

            if ( !this.TokenReader.Accept ( CalculatorTokenType.EndOfExpression ) )
                this.diagnosticReporter.Report ( CalculatorDiagnostics.SyntaxError.ThingExpected ( this.TokenReader.Location, "EOF" ) );

            return expr;
        }
    }
}
