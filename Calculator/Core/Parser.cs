using Calculator.Core.Nodes;
using Calculator.Core.Tokens;
using GUtils.Text;
using System;
using System.Collections.Generic;

namespace Calculator.Core
{
	internal class Parser
	{
		private readonly TReader<Token> TokenReader;

		public Parser ( IEnumerable<Token> tokens )
		{
			this.TokenReader = new TReader<Token> ( tokens );
		}

		public Token Consume ( TokenType Type )
			=> this.TokenReader.Peek ( ).Type != Type ? null : this.TokenReader.Read ( );

		public Token Consume ( TokenType Type1, TokenType Type2 )
		{
			TokenType typ = this.TokenReader.Peek ( ).Type;
			if ( typ != Type1 && typ != Type2 )
				return null;
			return this.TokenReader.Read ( );
		}

		public Token Expect ( TokenType Type )
		{
			Token next = this.TokenReader.Read ( );
			if ( next.Type != Type )
				throw new Exception ( $"Expected {Type} but received {next.Type}" );
			return next;
		}

		public Token Expect ( TokenType Type1, TokenType Type2 )
		{
			Token next = this.TokenReader.Read ( );
			if ( next.Type != Type1 && next.Type != Type2 )
				throw new Exception ( $"Expected {Type1} or {Type2} but received {next.Type}" );
			return next;
		}

		public Boolean HasNext ( ) => this.TokenReader.Peek ( ) != null;

		public Boolean IsNext ( TokenType Type ) => this.TokenReader.Peek ( ).Type == Type;

		public Boolean IsNext ( TokenType Type1, TokenType Type2 )
		{
			return this.TokenReader.Peek ( ).Type == Type1 || this.TokenReader.Peek ( ).Type == Type2;
		}

		/*
		 * expr	::= literal, operator, expr
		 *			| '(', expr, ')'
		 *			| literal;
		 */

		public ASTNode ParseExpression ( Action<String> Log )
		{
			if ( this.HasNext ( ) && this.IsNext ( TokenType.LParen ) )
			{
				Log ( "Parsing parenthesised expression." );
				ASTNode expr = this.ParseExpression ( Log );
				this.Expect ( TokenType.RParen );
				return expr;
			}
			else if ( this.HasNext ( ) && this.IsNext ( TokenType.UnaryOp, TokenType.Number ) )
			{
				Log ( "Parsing number or operation" );
				ValueExpression lhs = this.ParseLiteral ( Log );

				if ( this.HasNext ( ) && this.IsNext ( TokenType.BinaryOp ) )
				{
					Log ( "Operator found, parsing number" );
					Token op = this.Expect ( TokenType.BinaryOp );
					ASTNode rhs = this.ParseExpression ( Log );

					switch ( op.Raw )
					{
						case "+":
							return new AdditionOperatorExpression ( lhs, rhs );

						case "-":
							return new SubtractionOperatorExpression ( lhs, rhs );
					}
				}

				return lhs;
			}
			else
				throw new Exception ( $"Unexpected {this.TokenReader.Peek ( ).Raw} near {this.TokenReader.Peek ( 0 ).Raw}" );
		}

		// literal ::= [op], ident | [op], number;
		public ValueExpression ParseLiteral ( Action<String> Log )
		{
			Token unary = this.Consume ( TokenType.UnaryOp );
			ValueExpression value = null;
			Log ( $"Unary operator {( unary != null ? "was" : "wasn't" )} found" );

			Token tok = this.Expect ( TokenType.Identifier, TokenType.Number );
			if ( tok.Type == TokenType.Identifier )
			{
				Log ( $"Reading constant: {tok.Raw}" );
				value = new ConstantValue ( tok );
			}
			else if ( tok.Type == TokenType.Number )
			{
				Log ( $"Reading number: {tok.Raw}" );
				value = new NumberLiteral ( tok );
			}
			else
				throw new Exception ( $"Unexpected token type: {tok.Type}" );

			if ( unary != null && unary.Raw == "-" )
				value.Value *= -1;
			Log ( $"Final ValueExpression value: {value.Value}" );

			return value;
		}
	}
}
