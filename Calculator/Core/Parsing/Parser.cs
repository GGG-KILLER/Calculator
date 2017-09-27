using Calculator.Core.Lexing;
using Calculator.Core.Parsing.Nodes.Base;
using Calculator.Core.Parsing.Nodes.Literals;
using Calculator.Core.Runtime.Base;
using GUtils.Text;
using System;
using System.Collections.Generic;

namespace Calculator.Core.Parsing
{
	internal class Parser
	{
		private readonly TReader<Token> TokenReader;
		private readonly Action<String> Log;

		public Parser ( IEnumerable<Token> tokens, Action<String> Log )
		{
			this.TokenReader = new TReader<Token> ( tokens );
			this.Log = Log ?? ( str => { } );
		}

		/// <summary>
		/// Parses a sequence of tokens
		/// </summary>
		/// <param name="tokens">tokens to parse</param>
		/// <param name="Log">logging function</param>
		/// <returns></returns>
		public static ASTNode Parse ( IEnumerable<Token> tokens, Action<String> Log = null )
			=> new Parser ( tokens, Log ).Parse ( );

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

		public Token Expect ( TokenType Type1, TokenType Type2, TokenType Type3 )
		{
			Token next = this.TokenReader.Read ( );
			if ( next.Type != Type1 && next.Type != Type2 && next.Type != Type3 )
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

		public Boolean IsNext ( TokenType Type1, TokenType Type2, TokenType Type3, TokenType Type4 )
			=> this.IsNext ( Type1, Type2, Type3 ) || this.IsNext ( Type4 );

		#endregion 1. Token Reading

		private FunctionCallExpression ParseFunctionCallExpression ( Token identifier )
		{
			if ( !Language.IsFunction ( identifier.Raw ) )
				throw new Exception ( $"Invalid function call, {identifier.Raw} is not a function!" );
			var args = new List<ASTNode> ( );

			this.Expect ( TokenType.LParen );
			do
			{
				args.Add ( this.ParseLiteral ( ) );
				this.Consume ( TokenType.Comma );
			}
			while ( this.IsNext ( TokenType.Comma ) );
			this.Expect ( TokenType.RParen );

			return new FunctionCallExpression ( identifier, args );
		}

		private ASTNode ParseParenthesisExpresion ( )
		{
			ASTNode expr = this.ParseExpression ( "", 0 );
			this.Expect ( TokenType.RParen );
			return expr;
		}

		//literal ::= [op], ident | [op], number | [op], '(', expr, ')' ;
		public ASTNode ParseLiteral ( )
		{
			Token unary = this.Consume ( TokenType.UnaryOp );
			ASTNode value = null;
			this.Log ( $"Unary operator {( unary != null ? "was" : "wasn't" )} found" );

			Token tok = this.Expect ( TokenType.Identifier, TokenType.Number, TokenType.LParen );

			Sign sign = unary != null && unary.Raw == "-" ? Sign.Negative : Sign.Positive;

			if ( tok.Type == TokenType.Identifier )
			{
				if ( this.HasNext ( ) && this.IsNext ( TokenType.LParen ) )
				{
					value = this.ParseFunctionCallExpression ( tok );
				}
				else
				{
					this.Log ( $"Reading constant: {tok.Raw}" );
					value = new ConstantLiteral ( tok, sign );
				}
			}
			else if ( tok.Type == TokenType.Number )
			{
				this.Log ( $"Reading number: {tok.Raw}" );
				value = new NumericLiteral ( tok, sign );
			}
			else if ( tok.Type == TokenType.LParen )
			{
				this.Log ( $"Reading parenthesized expression." );
				value = this.ParseParenthesisExpresion ( );
			}
			else
			{
				throw new Exception ( $"Unexpected token type: {tok.Type}" );
			}

			this.Log ( $"Final expression: {value}" );

			return value;
		}

		//expr	::= expr, operator, literal | literal ;
		public ASTNode ParseExpression ( String PreviousOperatorRaw, Int32 PreviousOperatorPriority )
		{
			//expr ::= expr, operator, literal | literal
			if ( this.HasNext ( ) && this.IsNext ( TokenType.UnaryOp, TokenType.Number, TokenType.Identifier, TokenType.LParen ) )
			{
				this.Log ( "Parsing number or operation" );
				// Everything starts with a literal
				ASTNode expression = this.ParseLiteral ( );

				while ( true )
				{
					if ( this.HasNext ( ) && this.IsNext ( TokenType.BinaryOp ) )
					{
						this.Log ( "Reading operator" );
						var opRaw = this.TokenReader.Peek ( ).Raw;
						Operator op = Language.Operators[opRaw];
						if ( op.Priority > PreviousOperatorPriority
							|| ( opRaw == PreviousOperatorRaw && op.Associativity == Associativity.Right ) )
						{
							// Read the operator as we only peeked it
							this.Consume ( TokenType.BinaryOp );
							ASTNode rhs = this.ParseExpression ( opRaw, op.Priority );
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

		public ASTNode Parse ( ) => this.ParseExpression ( "", -1 );

		#endregion 3. Entry Point
	}
}
