using System;
using Calculator.Definitions;
using Calculator.Lexing;
using Calculator.Parsing.AST;
using GParse;
using GParse.Lexing;

namespace Calculator.Parsing
{
    /// <summary>
    /// A general AST manipulation helper with methods to generate all kinds of things
    /// </summary>
    public static class ASTHelper
    {
        /// <summary>
        /// Generates a <see cref="Token{TokenTypeT}" />
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="raw"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Token<CalculatorTokenType> Token ( String id, CalculatorTokenType type, String raw = null, Object value = null ) =>
            new Token<CalculatorTokenType> ( id, raw ?? id, value ?? id, type, SourceRange.Zero );

        /// <summary>
        /// Generates an <see cref="IdentifierExpression" />
        /// </summary>
        /// <param name="ident"></param>
        /// <returns></returns>
        public static IdentifierExpression Identifier ( String ident ) =>
            new IdentifierExpression ( Token ( ident, CalculatorTokenType.Identifier, ident, ident ) );

        /// <summary>
        /// Generates a <see cref="NumberExpression" />
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static NumberExpression Number ( Double value ) =>
            new NumberExpression ( Token ( "number", CalculatorTokenType.Number, value.ToString ( ), value ) );

        /// <summary>
        /// Creates a <see cref="CalculatorTreeNode" /> from an <see cref="Object" />
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static CalculatorTreeNode Node ( Object obj )
        {
            switch ( obj )
            {
                case CalculatorTreeNode node:
                    return node;

                case SByte _:
                case Byte _:
                case Int32 _:
                case UInt32 _:
                case Int64 _:
                case UInt64 _:
                case Single _:
                case Double _:
                    return Number ( Convert.ToDouble ( obj ) );

                case String str:
                    return Identifier ( str );

                default:
                    throw new ArgumentException ( $"Invalid argument type {obj.GetType ( )}", nameof ( obj ) );
            }
        }

        /// <summary>
        /// Generates a <see cref="FunctionCallExpression" />
        /// </summary>
        /// <param name="strIdent"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        public static FunctionCallExpression FunctionCall ( String strIdent, params Object[] @params )
        {
            var toks = new Token<CalculatorTokenType>[@params.Length + 2];
            IdentifierExpression ident = Identifier ( strIdent );
            var i = 0;
            toks[i++] = Token ( "(", CalculatorTokenType.LParen, "(" );
            CalculatorTreeNode[] args = Array.ConvertAll ( @params, param =>
            {
                toks[i++] = Token ( ",", CalculatorTokenType.Comma, "," );
                return Node ( param );
            } );
            toks[i] = Token ( ")", CalculatorTokenType.RParen, ")" );

            return new FunctionCallExpression ( ident, args, toks );
        }

        /// <summary>
        /// Generates a <see cref="UnaryOperatorExpression" />
        /// </summary>
        /// <param name="operator"></param>
        /// <param name="operand"></param>
        /// <param name="operatorFix"></param>
        /// <returns></returns>
        public static UnaryOperatorExpression UnaryOperator ( String @operator, Object operand, UnaryOperatorFix operatorFix ) =>
            new UnaryOperatorExpression (
                operatorFix
,
                Token ( @operator, CalculatorTokenType.Operator, @operator ),
                Node ( operand ) );

        /// <summary>
        /// Generates a <see cref="BinaryOperatorExpression" />
        /// </summary>
        /// <param name="leftHandSide"></param>
        /// <param name="operator"></param>
        /// <param name="rightHandSide"></param>
        /// <returns></returns>
        public static BinaryOperatorExpression BinaryOperator ( Object leftHandSide, String @operator, Object rightHandSide ) =>
            new BinaryOperatorExpression (
                Token ( @operator, CalculatorTokenType.Operator, @operator ),
                Node ( leftHandSide ),
                Node ( rightHandSide )
            );

        /// <summary>
        /// Generates a <see cref="GroupedExpression" />
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static GroupedExpression Grouped ( Object expression ) =>
            new GroupedExpression ( Token ( "(", CalculatorTokenType.LParen, "(" ), Node ( expression ), Token ( ")", CalculatorTokenType.RParen, ")" ) );

        /// <summary>
        /// Generates an <see cref="ImplicitMultiplicationExpression" />
        /// </summary>
        /// <param name="leftHandSide"></param>
        /// <param name="rightHandSide"></param>
        /// <returns></returns>
        public static ImplicitMultiplicationExpression ImplicitMultiplication ( Object leftHandSide, Object rightHandSide ) =>
            new ImplicitMultiplicationExpression ( Node ( leftHandSide ), Node ( rightHandSide ) );
    }
}
