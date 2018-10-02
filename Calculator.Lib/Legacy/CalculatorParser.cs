using System;
using System.Collections.Generic;
using Calculator.Lib.AST;
using Calculator.Lib.Definitions;
using GParse.Common.AST;
using GParse.Common.Errors;
using GParse.Parsing;

namespace Calculator.Lib
{
    public class CalculatorParser : ParserBase
    {
        private readonly CalculatorLang Language;

        public CalculatorParser ( CalculatorLang lang, LexerBase lexer ) : base ( lexer )
        {
            this.Language = lang;
        }

        // Numeric expressions
        private NumberExpression ParseNumberExpression ( ) =>
            new NumberExpression ( ( Double ) this.Expect<Token> ( TokenType.Number ).Value );

        // Constant expressions
        private ConstantExpression ParseConstantExpression ( ) =>
            new ConstantExpression ( this.Expect<Token> ( TokenType.Identifier ).Raw );

        // Parenthesised expressions and numeric expressions
        private CASTNode ParsePrimaryExpression ( )
        {
            if ( this.NextIs ( "(" ) )
            {
                this.Expect<Token> ( "(" );
                CASTNode expr = this.ParseExpression ( );
                this.Expect<Token> ( ")" );
                return new ParenthesisExpression ( expr );
            }
            else if ( this.NextIs ( TokenType.Identifier ) )
                return this.ParseConstantExpression ( );
            else
                throw new ParseException ( this.GetLocation ( ), "Expected primary expression but got none." );
        }

        // Function calls
        private FunctionCallExpression ParseFunctionCall ( String ident )
        {
            if ( !this.Language.HasFunction ( ident ) )
                throw new ParseException ( this.GetLocation ( ), $"Unknown function {ident}" );

            var args = new List<CASTNode> ( );
            this.Expect<Token> ( "(" );
            while ( !this.Consume ( ")", out Token _ ) )
            {
                args.Add ( this.ParseExpression ( ) );
                if ( !this.Consume ( ",", out Token _ ) )
                {
                    this.Expect<Token> ( ")" );
                    break;
                }
            }

            return new FunctionCallExpression ( ident, args.ToArray ( ) );
        }

        // Constant expressions or function calls
        private CASTNode ParseConstantOrFunctionCall ( )
        {
            CASTNode primary = this.ParsePrimaryExpression ( );
            if ( this.NextIs ( "(" ) )
            {
                if ( !( primary is ConstantExpression ) )
                    throw new Exception ( "Invalid function call." );
                return this.ParseFunctionCall ( ( primary as ConstantExpression ).Identifier );
            }
            else
                return primary;
        }

        // Number expressions, constant expressions or function calls
        private CASTNode ParseLiteral ( ) =>
            this.NextIs ( TokenType.Number )
                ? this.ParseNumberExpression ( )
                : this.ParseConstantOrFunctionCall ( );

        // Unary operators and binary operators
        private CASTNode ParseOperatorExpression ( Int32 lastPrecedence )
        {
            CASTNode expr;

            // Prefix operators
            if ( this.NextIs ( TokenType.Operator ) )
            {
                Token unOp = this.Expect<Token> ( TokenType.Operator );
                if ( !this.Language.HasUnaryOperator ( unOp.ID, UnaryOperatorFix.Prefix ) )
                    throw new ParseException ( unOp.Range.Start, "Unknown prefix operator " + unOp.ID );
                expr = new UnaryOperatorExpression ( unOp.Raw, this.ParseOperatorExpression ( 0 ), UnaryOperatorFix.Prefix );
            }
            // No prefix operators
            else
            {
                expr = this.ParseLiteral ( );
            }

            // Postfix operators
            while ( this.Language.HasUnaryOperator ( this.Peek ( ).ID, UnaryOperatorFix.Postfix )
                && ( !this.Language.HasBinaryOperator ( this.Peek ( ).ID )
                    || ( this.Peek ( 1 ).Type != TokenType.Number && this.Peek ( 1 ).Type != TokenType.LParen
                        && this.Peek ( 1 ).Type != TokenType.Identifier ) ) )
            {
                Token unOp = this.Expect<Token> ( TokenType.Operator );
                expr = new UnaryOperatorExpression ( unOp.Raw, expr, UnaryOperatorFix.Postfix );
            }

            // Right associativity operators loop
            while ( this.Language.HasBinaryOperator ( this.Peek ( ).ID ) )
            {
                BinaryOperatorDef opDef = this.Language.GetBinaryOperator ( this.Peek ( ).ID );
                var precedence = opDef.Associativity == OperatorAssociativity.Right ? opDef.Precedence + 1 : opDef.Precedence;
                if ( precedence > lastPrecedence )
                {
                    this.Get<Token> ( );
                    CASTNode rhs = this.ParseOperatorExpression ( precedence );
                    expr = new BinaryOperatorExpression ( opDef.Operator, expr, rhs );
                }
                else
                    break;
            }

            return expr;
        }

        // All things
        private CASTNode ParseExpression ( ) => this.ParseOperatorExpression ( 0 );

        // Alias
        public CASTNode Parse ( )
        {
            CASTNode expr = this.ParseExpression ( );
            if ( this.Position < this.TokenList.Count - 1 )
                throw new ParseException ( this.TokenList[this.Position + 1].Range.Start, "Unfinished expression." );
            return expr;
        }
    }
}
