using System;
using System.Collections.Generic;
using Calculator.Definitions;
using Calculator.Lexing;
using Calculator.Parsing.AST;
using Calculator.Parsing.Visitors;
using GParse.Common.Errors;
using GParse.Common.Lexing;
using GParse.Parsing.Abstractions.Lexing;
using GParse.Parsing.Parsing;

namespace Calculator.Parsing
{
    public class CalculatorParser : HandwrittenParserBase<CalculatorTokenType>
    {
        private readonly CalculatorLanguage language;

        public CalculatorParser ( ILexer<CalculatorTokenType> lexer, CalculatorLanguage language ) : base ( lexer )
        {
            this.language = language;
        }

        private FunctionCallExpression ParseFunctionCall ( Token<CalculatorTokenType> ident, Token<CalculatorTokenType> lparen )
        {
            Token<CalculatorTokenType> tok;
            var args = new List<CalculatorASTNode> ( );
            var toks = new List<Token<CalculatorTokenType>>
            {
                ident,
                lparen
            };

            while ( !this.Consume ( CalculatorTokenType.RParen, out tok ) )
            {
                args.Add ( this.ParseExpression ( -1 ) );

                if ( this.Consume ( CalculatorTokenType.RParen, out tok ) )
                {
                    toks.Add ( tok );
                    break;
                }
                toks.Add ( this.Expect ( CalculatorTokenType.Comma ) );
            }

            if ( toks[toks.Count - 1].Type != CalculatorTokenType.RParen )
                toks.Add ( tok );

            return new FunctionCallExpression ( toks, args.ToArray ( ) );
        }

        private CalculatorASTNode ParseAtomic ( )
        {
            if ( this.Consume ( CalculatorTokenType.LParen ) )
            {
                CalculatorASTNode expr = this.ParseExpression ( -1 );
                this.Expect ( CalculatorTokenType.RParen );
                return expr;
            }
            else if ( this.Consume ( CalculatorTokenType.Identifier, out Token<CalculatorTokenType> ident ) )
            {
                return this.Consume ( CalculatorTokenType.LParen, out Token<CalculatorTokenType> lparen )
                    ? this.ParseFunctionCall ( ident, lparen )
                    : ( CalculatorASTNode ) new IdentifierExpression ( ident );
            }
            else if ( this.Consume ( CalculatorTokenType.Number, out Token<CalculatorTokenType> number ) )
                return new NumberExpression ( number );
            else
                throw new LexingException ( this.Lexer.Location, "Invalid expression." );
        }

        private CalculatorASTNode ParsePrefixedExpression ( ) => this.PeekToken ( ).Type == CalculatorTokenType.Operator && this.language.HasUnaryOperator ( this.PeekToken ( ).Raw, UnaryOperatorFix.Prefix )
                ? new UnaryOperatorExpression ( this.ReadToken ( ), this.ParsePrefixedExpression ( ), UnaryOperatorFix.Prefix )
                : this.ParseAtomic ( );

        private CalculatorASTNode ParsePostfixedExpression ( )
        {
            CalculatorASTNode expr = this.ParsePrefixedExpression ( );
            while ( this.PeekToken ( ).Type == CalculatorTokenType.Operator && this.language.HasUnaryOperator ( this.PeekToken ( ).Raw, UnaryOperatorFix.Postfix ) )
                expr = new UnaryOperatorExpression ( this.ReadToken ( ), expr, UnaryOperatorFix.Postfix );
            return expr;
        }

        private CalculatorASTNode ParseExpression ( Int32 parentPrecedence )
        {
            CalculatorASTNode lhs = this.ParsePostfixedExpression ( );

            // Infix loop
            while ( this.PeekToken ( ).Type == CalculatorTokenType.Operator && this.language.HasBinaryOperator ( this.PeekToken ( ).Raw ) )
            {
                BinaryOperatorDef def = this.language.GetBinaryOperator ( this.PeekToken ( ).Raw );

                // Recurse on higher priority operators and right
                // associative operators
                if ( def.Precedence >= parentPrecedence )
                    lhs = new BinaryOperatorExpression ( this.ReadToken ( ), lhs, this.ParseExpression ( def.Associativity == OperatorAssociativity.Left ? def.Precedence + 1 : def.Precedence ) );
                else
                    break;
            }

            return lhs;
        }

        public CalculatorASTNode Parse ( )
        {
            CalculatorASTNode expression = this.ParseExpression ( -1 );
            this.Expect ( CalculatorTokenType.EndOfExpression );
            return expression;
        }
    }
}
