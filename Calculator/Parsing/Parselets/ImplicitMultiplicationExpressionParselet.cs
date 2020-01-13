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
        public Int32 Precedence { get; }

        /// <summary>
        /// Initializes this <see cref="ImplicitMultiplicationExpressionParselet" />
        /// </summary>
        /// <param name="precedence"></param>
        public ImplicitMultiplicationExpressionParselet ( Int32 precedence )
        {
            this.Precedence = precedence;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="parser"></param>
        /// <param name="expression"></param>
        /// <param name="diagnosticEmitter"></param>
        /// <param name="parsedExpression"></param>
        /// <returns></returns>
        public Boolean TryParse ( IPrattParser<CalculatorTokenType, CalculatorTreeNode> parser, CalculatorTreeNode expression, IProgress<Diagnostic> diagnosticEmitter, out CalculatorTreeNode parsedExpression )
        {
            if ( parser is null )
                throw new ArgumentNullException ( nameof ( parser ) );
            
            if ( expression is null )
                throw new ArgumentNullException ( nameof ( expression ) );
            
            if ( diagnosticEmitter is null )
                throw new ArgumentNullException ( nameof ( diagnosticEmitter ) );

            if ( parser.TryParseExpression ( this.Precedence, out CalculatorTreeNode right ) )
            {
                parsedExpression = new ImplicitMultiplicationExpression ( expression, right );
                return true;
            }

            parsedExpression = null;
            return false;
        }
    }
}
