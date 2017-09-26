using Calculator.Core.Parsing.Nodes;
using Calculator.Core.Parsing.Nodes.Base;
using Calculator.Core.Parsing.Nodes.Literals;
using Calculator.Core.Lexing;
using GUtils.Text;
using System;
using System.Collections.Generic;
using Calculator.Core.Runtime;

namespace Calculator.Core.Parsing
{
	internal class Parser
	{
		private readonly TReader<Token> TokenReader;
		private readonly Action<String> Log;

		public Parser ( IEnumerable<Token> tokens, Action<String> Log )
		{
			this.TokenReader = new TReader<Token> ( tokens );
			this.Log = Log;
		}

		#region 1. Token Reading

		public Token Consume ( TokenType Type )
			=> this.IsNext ( Type ) ? this.TokenReader.Read ( ) : null;

		public Token Consume ( TokenType Type1, TokenType Type2 )
			=> this.IsNext ( Type1, Type2 ) ? this.TokenReader.Read ( ) : null;

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

		public Boolean IsNext ( TokenType Type )
			=> this.TokenReader.Peek ( ).Type == Type;

		public Boolean IsNext ( TokenType Type1, TokenType Type2 )
			=> this.IsNext ( Type1 ) || this.IsNext ( Type2 );

		public Boolean IsNext ( TokenType Type1, TokenType Type2, TokenType Type3 )
			=> this.IsNext ( Type1, Type2 ) || this.IsNext ( Type3 );

		#endregion 1. Token Reading

		private ASTNode ParseParenthesisExpresion ( )
		{
			this.Expect ( TokenType.LParen );
			ASTNode expr = this.ParseExpression ( 0 );
			this.Expect ( TokenType.RParen );
			return expr;
		}

		// literal ::= [op], ident | [op], number;
		public ValueExpression ParseLiteral ( )
		{
			Token unary = this.Consume ( TokenType.UnaryOp );
			ValueExpression value = null;
			this.Log?.Invoke ( $"Unary operator {( unary != null ? "was" : "wasn't" )} found" );

			Token tok = this.Expect ( TokenType.Identifier, TokenType.Number );
			Sign sign = unary != null && unary.Raw == "-" ? Sign.Negative : Sign.Positive;
			if ( tok.Type == TokenType.Identifier )
			{
				this.Log?.Invoke ( $"Reading constant: {tok.Raw}" );
				value = new ConstantLiteral ( tok, sign );
			}
			else if ( tok.Type == TokenType.Number )
			{
				this.Log?.Invoke ( $"Reading number: {tok.Raw}" );
				value = new NumericLiteral ( tok, sign );
			}
			else
				throw new Exception ( $"Unexpected token type: {tok.Type}" );

			this.Log?.Invoke ( $"Final ValueExpression value: {value.Value}" );

			return value;
		}

		/*
		 * expr	::= expr, operator, literal
		 *			| '(', expr, ')'
		 *			| literal;
		 */

		public ASTNode ParseExpression ( Int32 PreviousOperatorPriority )
		{
			// expr ::= '(', expr, ')'
			if ( this.HasNext ( ) && this.IsNext ( TokenType.LParen ) )
			{
				this.Log?.Invoke ( "Parsing parenthesised expression." );
				return this.ParseParenthesisExpresion ( );
			}
			// expr ::= expr, operator, literal | literal
			else if ( this.HasNext ( ) && this.IsNext ( TokenType.UnaryOp, TokenType.Number, TokenType.Identifier ) )
			{
				this.Log?.Invoke ( "Parsing number or operation" );
				// Everything starts with a literal
				ASTNode expression = this.ParseLiteral ( );

				while ( true )
				{
					if ( this.HasNext ( ) && this.IsNext ( TokenType.BinaryOp ) )
					{
						this.Log?.Invoke ( "Reading operator" );
						Operator op = Language.Operators[this.TokenReader.Peek ( ).Raw];
						if ( op.OwnPriority > PreviousOperatorPriority )
						{
							// Read the operator as we only peeked it
							this.Consume ( TokenType.BinaryOp );
							ASTNode rhs = this.ParseExpression ( op.BackupPriority );
							expression = new BinaryOperatorExpression (
								LeftHandSide: expression,
								RightHandSide: rhs,
								Operator: op
							);
						}
						else
						{
							break;
						}
					}
					else
					{
						break;
					}
				}

				// expr ::= literal
				return expression;
			}
			else
			{
				throw new Exception ( $"Unexpected {this.TokenReader.Peek ( ).Raw} near {this.TokenReader.Peek ( 0 ).Raw}" );
			}
		}

		#region 3. Entry Point

		public ASTNode Parse ( ) => this.ParseExpression ( -1 );

		#endregion 3. Entry Point
	}
}
