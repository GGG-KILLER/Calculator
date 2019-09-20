using System;
using System.Linq;
using Calculator.Definitions;
using Calculator.Lexing;
using Calculator.Parsing.AST;
using Calculator.Parsing.Parselets;
using GParse;
using GParse.Lexing;
using GParse.Parsing;

namespace Calculator.Parsing
{
    /// <summary>
    /// A <see cref="CalculatorParser" /> builder
    /// </summary>
    public class CalculatorParserBuilder : PrattParserBuilder<CalculatorTokenType, CalculatorTreeNode>
    {
        private static Boolean IsValidIdentifier ( in String str ) =>
            Char.IsLetter ( str[0] ) && str.Skip ( 1 ).All ( Char.IsLetterOrDigit );

        /// <summary>
        /// Initializes this <see cref="CalculatorParserBuilder" /> with a
        /// <see cref="CalculatorLanguage" />
        /// </summary>
        /// <param name="language"></param>
        public CalculatorParserBuilder ( CalculatorLanguage language )
        {
#pragma warning disable CC0067 // Virtual Method Called On Constructor
            var maxPrecedence = 0;
            this.RegisterLiteral ( CalculatorTokenType.Identifier, ( Token<CalculatorTokenType> tok, out CalculatorTreeNode node ) =>
            {
                node = new IdentifierExpression ( tok );
                return true;
            } );
            this.RegisterLiteral ( CalculatorTokenType.Number, ( Token<CalculatorTokenType> tok, out CalculatorTreeNode node ) =>
            {
                node = new NumberExpression ( tok );
                return true;
            } );

            foreach ( UnaryOperator unaryOp in language.UnaryOperators.Values )
            {
                if ( unaryOp.Precedence > maxPrecedence )
                    maxPrecedence = unaryOp.Precedence;

                if ( unaryOp.Fix == UnaryOperatorFix.Postfix )
                    this.RegisterSingleTokenPostfixOperator (
                        IsValidIdentifier ( unaryOp.Operator ) ? CalculatorTokenType.Identifier : CalculatorTokenType.Operator,
                        unaryOp.Operator,
                        unaryOp.Precedence,
                        ( CalculatorTreeNode expr, Token<CalculatorTokenType> unop, out CalculatorTreeNode node ) =>
                        {
                            node = new UnaryOperatorExpression ( UnaryOperatorFix.Postfix, unop, expr );
                            return true;
                        } );
                else
                    this.RegisterSingleTokenPrefixOperator (
                        IsValidIdentifier ( unaryOp.Operator ) ? CalculatorTokenType.Identifier : CalculatorTokenType.Operator,
                        unaryOp.Operator,
                        unaryOp.Precedence,
                        ( Token<CalculatorTokenType> unop, CalculatorTreeNode expr, out CalculatorTreeNode node ) =>
                        {
                            node = new UnaryOperatorExpression ( UnaryOperatorFix.Prefix, unop, expr );
                            return true;
                        } );
            }

            Int32 implMulPrecedence = -1;
            foreach ( BinaryOperator binaryOp in language.BinaryOperators.Values )
            {
                if ( binaryOp.Precedence > maxPrecedence )
                    maxPrecedence = binaryOp.Precedence;

                if ( String.IsNullOrWhiteSpace ( binaryOp.Operator ) )
                    implMulPrecedence = binaryOp.Precedence;

                this.RegisterSingleTokenInfixOperator (
                    IsValidIdentifier ( binaryOp.Operator ) ? CalculatorTokenType.Identifier : CalculatorTokenType.Operator,
                    binaryOp.Operator,
                    binaryOp.Precedence,
                    binaryOp.Associativity == Associativity.Right,
                    ( CalculatorTreeNode left, Token<CalculatorTokenType> op, CalculatorTreeNode right, out CalculatorTreeNode node ) =>
                    {
                        node = new BinaryOperatorExpression ( op, left, right );
                        return true;
                    } );
            }

            this.Register ( CalculatorTokenType.LParen, new GroupedExpressionParselet ( ) );
            this.Register ( CalculatorTokenType.LParen, new FunctionCallExpressionParselet ( maxPrecedence + 2 ) );
            var implicitMultiplicationParselet = new ImplicitMultiplicationExpressionParselet ( maxPrecedence + 1 );
            foreach ( CalculatorTokenType tokenType in new[] { CalculatorTokenType.Identifier, CalculatorTokenType.LParen, CalculatorTokenType.Number } )
                this.Register ( tokenType, implicitMultiplicationParselet );
#pragma warning restore CC0067 // Virtual Method Called On Constructor
        }

        /// <summary>
        /// Creates a <see cref="CalculatorParser" />
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="diagnosticEmitter"></param>
        /// <returns></returns>
        public override IPrattParser<CalculatorTokenType, CalculatorTreeNode> CreateParser ( ITokenReader<CalculatorTokenType> reader, IProgress<Diagnostic> diagnosticEmitter ) =>
            new CalculatorParser ( reader, this.prefixModuleTree, this.infixModuleTree, diagnosticEmitter );
    }
}
