using System;
using System.Collections.Generic;
using Calculator.Runtime.Definitions;
using Calculator.Runtime.AST;
using GParse.Lexing;
using GParse.Parsing;
using GParse.Parsing.Errors;

namespace Calculator.Runtime
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
        private ASTNode ParsePrimaryExpression ( )
        {
            if ( this.NextIs ( "(" ) )
            {
                this.Expect<Token> ( "(" );
                ASTNode expr = this.ParseExpression ( );
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

            var args = new List<ASTNode> ( );
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
        private ASTNode ParseConstantOrFunctionCall ( )
        {
            ASTNode primary = this.ParsePrimaryExpression ( );
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
        private ASTNode ParseLiteral ( ) =>
            this.NextIs ( TokenType.Number )
                ? this.ParseNumberExpression ( )
                : this.ParseConstantOrFunctionCall ( );

        // Unary operators and binary operators
        private ASTNode ParseOperatorExpression ( Int32 lastPrecedence )
        {
            ASTNode expr;

            // Prefix operators
            if ( this.NextIs ( TokenType.Operator ) )
            {
                Token unOp = this.Expect<Token> ( TokenType.Operator );
                expr = new UnaryOperatorExpression ( unOp.Raw, this.ParseOperatorExpression ( 0 ), UnaryOperatorFix.Prefix );
            }
            // No prefix operators
            else
            {
                expr = this.ParseLiteral ( );
            }

            // Postfix operators
            while ( this.Language.HasUnaryOperator ( this.Peek ( ).ID )
                && ( !this.Language.HasBinaryOperator ( this.Peek ( ).ID ) || this.Peek ( 1 ).Type != TokenType.Number ) )
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
                    ASTNode rhs = this.ParseOperatorExpression ( precedence );
                    expr = new BinaryOperatorExpression ( opDef.Operator, expr, rhs );
                }
                else
                    break;
            }

            return expr;
        }

        // All things
        private ASTNode ParseExpression ( ) => this.ParseOperatorExpression ( 0 );

        // Alias
        public ASTNode Parse ( ) => this.ParseExpression ( );
    }
}
