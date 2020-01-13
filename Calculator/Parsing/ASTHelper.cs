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
        /// Generates a <see cref="Token{TokenTypeT}"/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="raw"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Token<CalculatorTokenType> Token ( String id, CalculatorTokenType type, String raw = null, Object value = null ) =>
            new Token<CalculatorTokenType> ( id, raw ?? id, value ?? id, type, SourceRange.Zero );

        /// <summary>
        /// Returns a token for an operator string
        /// </summary>
        /// <param name="operator"></param>
        /// <returns></returns>
        public static Token<CalculatorTokenType> OperatorToken ( String @operator )
        {
            static Boolean IsIdentifier ( String str )
            {
                if ( String.IsNullOrEmpty ( str ) )
                    throw new ArgumentException ( "The string cannot be null or empty.", nameof ( str ) );

                if ( !Char.IsLetter ( str[0] ) && str[0] != '_' )
                    return false;

                for ( var i = 1; i < str.Length; i++ )
                {
                    if ( !Char.IsLetterOrDigit ( str[i] ) && str[i] != '_' )
                        return false;
                }

                return true;
            }

            if ( IsIdentifier ( @operator ) )
                return Token ( @operator, CalculatorTokenType.Identifier, @operator, @operator );
            else
                return Token ( @operator, CalculatorTokenType.Operator, @operator );
        }

        /// <summary>
        /// Generates an <see cref="IdentifierExpression"/>
        /// </summary>
        /// <param name="ident"></param>
        /// <returns></returns>
        public static IdentifierExpression Identifier ( String ident ) =>
            new IdentifierExpression ( Token ( ident, CalculatorTokenType.Identifier, ident, ident ) );

        /// <summary>
        /// Generates a <see cref="NumberExpression"/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static NumberExpression Number ( Double value ) =>
            new NumberExpression ( Token ( "number", CalculatorTokenType.Number, value.ToString ( ), value ) );

        /// <summary>
        /// Creates a <see cref="CalculatorTreeNode"/> from an <see cref="Object"/>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static CalculatorTreeNode Node ( Object obj )
        {
            if ( obj is null )
                throw new ArgumentNullException ( nameof ( obj ) );

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
        /// Generates a <see cref="FunctionCallExpression"/>
        /// </summary>
        /// <param name="strIdent"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        public static FunctionCallExpression Function ( String strIdent, params Object[] @params )
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
        /// Generates a prefix <see cref="UnaryOperatorExpression"/>
        /// </summary>
        /// <param name="operator"></param>
        /// <param name="operand"></param>
        /// <returns></returns>
        public static UnaryOperatorExpression Prefix ( String @operator, Object operand ) =>
            new UnaryOperatorExpression ( UnaryOperatorFix.Prefix, OperatorToken ( @operator ), Node ( operand ) );

        /// <summary>
        /// Generates a postfix <see cref="UnaryOperatorExpression"/>
        /// </summary>
        /// <param name="operand"></param>
        /// <param name="operator"></param>
        /// <returns></returns>
        public static UnaryOperatorExpression Postfix ( Object operand, String @operator ) =>
            new UnaryOperatorExpression ( UnaryOperatorFix.Postfix, OperatorToken ( @operator ), Node ( operand ) );

        /// <summary>
        /// Generates a <see cref="BinaryOperatorExpression"/>
        /// </summary>
        /// <param name="leftHandSide"></param>
        /// <param name="operator"></param>
        /// <param name="rightHandSide"></param>
        /// <returns></returns>
        public static BinaryOperatorExpression Binary ( Object leftHandSide, String @operator, Object rightHandSide ) =>
            new BinaryOperatorExpression (
                Token ( @operator, CalculatorTokenType.Operator, @operator ),
                Node ( leftHandSide ),
                Node ( rightHandSide )
            );

        /// <summary>
        /// Generates a <see cref="GroupedExpression"/>
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static GroupedExpression Grouped ( Object expression ) =>
            new GroupedExpression ( Token ( "(", CalculatorTokenType.LParen, "(" ), Node ( expression ), Token ( ")", CalculatorTokenType.RParen, ")" ) );

        /// <summary>
        /// Generates an <see cref="ImplicitMultiplicationExpression"/>
        /// </summary>
        /// <param name="leftHandSide"></param>
        /// <param name="rightHandSide"></param>
        /// <returns></returns>
        public static ImplicitMultiplicationExpression Implicit ( Object leftHandSide, Object rightHandSide ) =>
            new ImplicitMultiplicationExpression ( Node ( leftHandSide ), Node ( rightHandSide ) );
        
        /// <summary>
        /// Generates an <see cref="SuperscriptExponentiationExpression"/>
        /// </summary>
        /// <param name="base"></param>
        /// <param name="exponent"></param>
        /// <returns></returns>
        public static SuperscriptExponentiationExpression Superscript ( Object @base, Int32 exponent ) =>
            new SuperscriptExponentiationExpression ( Node ( @base ), Token ( "number-dec", CalculatorTokenType.Number, SuperscriptChars.TranslateNumber ( exponent ), exponent ) );
    }
}