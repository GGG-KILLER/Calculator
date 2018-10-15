using System;
using Calculator.Definitions;
using Calculator.Lexing;
using Calculator.Parsing.AST;
using GParse.Common;
using GParse.Common.Lexing;

namespace Calculator.Tests
{
    internal static class ASTHelper
    {
        public static Token<CalculatorTokenType> GetTok ( String id, CalculatorTokenType type, String raw = null, Object value = null ) =>
            new Token<CalculatorTokenType> ( id, raw ?? id, value ?? id, type, SourceRange.Zero );

        public static IdentifierExpression Identifier ( String ident ) =>
            new IdentifierExpression ( GetTok ( ident, CalculatorTokenType.Identifier, ident, ident ) );

        public static NumberExpression Number ( Double value ) =>
            new NumberExpression ( GetTok ( "number", CalculatorTokenType.Number, value.ToString ( ), value ) );

        public static CalculatorASTNode Node ( Object obj )
        {
            switch ( obj )
            {
                case CalculatorASTNode node:
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

        public static FunctionCallExpression FunctionCall ( String strIdent, params Object[] @params )
        {
            var toks = new Token<CalculatorTokenType>[@params.Length + 3];
            toks[0] = GetTok ( strIdent, CalculatorTokenType.Identifier, strIdent, strIdent );
            toks[1] = GetTok ( "(", CalculatorTokenType.LParen, "(" );
            var i = 2;
            CalculatorASTNode[] args = Array.ConvertAll ( @params, param =>
            {
                toks[i++] = GetTok ( ",", CalculatorTokenType.Comma, "," );
                return Node ( param );
            } );
            toks[i] = GetTok ( ")", CalculatorTokenType.RParen, ")" );

            return new FunctionCallExpression ( toks, args );
        }

        public static UnaryOperatorExpression UnaryOperator ( String @operator, Object operand, UnaryOperatorFix operatorFix ) =>
            new UnaryOperatorExpression (
                GetTok ( @operator, CalculatorTokenType.Operator, @operator ),
                Node ( operand ),
                operatorFix
            );

        public static BinaryOperatorExpression BinaryOperator ( Object leftHandSide, String @operator, Object rightHandSide ) =>
            new BinaryOperatorExpression (
                GetTok ( @operator, CalculatorTokenType.Operator, @operator ),
                Node ( leftHandSide ),
                Node ( rightHandSide )
            );
    }
}
