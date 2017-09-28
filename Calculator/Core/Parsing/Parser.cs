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
				throw new ExpressionException ( $"Expected {Type} but received {next.Type}" );
			return next;
		}

		public Token Expect ( TokenType Type1, TokenType Type2 )
		{
			Token next = this.TokenReader.Read ( );
			if ( next.Type != Type1 && next.Type != Type2 )
				throw new ExpressionException ( $"Expected {Type1} or {Type2} but received {next.Type}" );
			return next;
		}

		public Token Expect ( TokenType Type1, TokenType Type2, TokenType Type3 )
		{
			Token next = this.TokenReader.Read ( );
			if ( next.Type != Type1 && next.Type != Type2 && next.Type != Type3 )
				throw new ExpressionException ( $"Expected {Type1} or {Type2} but received {next.Type}" );
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

		#region 2. Parsing

		private FunctionCallExpression ParseFunctionCallExpression ( Token identifier )
		{
			this.Log ( $"\tParseFunctionCallExpression ( {identifier} )" );
			if ( !Language.IsFunction ( identifier.Raw ) )
				throw new ExpressionException ( $"Invalid function call, {identifier.Raw} is not a function!" );
			var args = new List<ASTNode> ( );

			this.Expect ( TokenType.LParen );
			do
			{
				args.Add ( this.ParseLiteral ( ) );
				this.Consume ( TokenType.Comma );
			}
			while ( this.IsNext ( TokenType.Comma ) );
			this.Expect ( TokenType.RParen );

			if ( args.Count > Language.Functions[identifier.Raw].ArgumentCount )
				throw new ExpressionException ( $"Too many arguments passed to {identifier.Raw}." );

			return new FunctionCallExpression ( identifier, args );
		}

		private ASTNode ParseParenthesisExpresion ( )
		{
			this.Log ( $"\tParseParenthesisExpresion ( )" );
			ASTNode expr = this.ParseExpression ( "", 0 );
			this.Expect ( TokenType.RParen );
			return expr;
		}

		//literal ::= [op], ident | [op], number | [op], '(', expr, ')' ;
		public ASTNode ParseLiteral ( )
		{
			this.Log ( $"\tParseLiteral ( )" );
			Token unary = this.Consume ( TokenType.UnaryOp );
			ASTNode value = null;
			this.Log ( $"\t\tUnary operator {( unary != null ? "was" : "wasn't" )} found" );

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
					this.Log ( $"\t\tReading constant: {tok.Raw}" );
					if ( !Language.IsConstant ( tok.Raw ) )
						throw new ExpressionException ( $"Unknown constant {tok.Raw}." );
					value = new ConstantLiteral ( tok, sign );
				}
			}
			else if ( tok.Type == TokenType.Number )
			{
				this.Log ( $"\t\tReading number: {tok.Raw}" );
				value = new NumericLiteral ( tok, sign );
			}
			else if ( tok.Type == TokenType.LParen )
			{
				this.Log ( $"\t\tReading parenthesized expression." );
				value = this.ParseParenthesisExpresion ( );
			}
			else
			{
				throw new ExpressionException ( $"Unexpected token type: {tok.Type}" );
			}

			this.Log ( $"Final expression: {value}" );

			return value;
		}

		//expr	::= expr, operator, literal | literal ;
		public ASTNode ParseExpression ( String PreviousOperatorRaw, Int32 PreviousOperatorPriority )
		{
			this.Log ( $"\tParseExpression ( {PreviousOperatorRaw}, {PreviousOperatorPriority} )" );
			//expr ::= expr, operator, literal | literal
			if ( this.HasNext ( ) && this.IsNext ( TokenType.UnaryOp, TokenType.Number, TokenType.Identifier, TokenType.LParen ) )
			{
				// Everything starts with a literal
				ASTNode expression = this.ParseLiteral ( );

				while ( true )
				{
					if ( this.HasNext ( ) && this.IsNext ( TokenType.BinaryOp ) )
					{
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

				return expression;
			}
			else
			{
				throw new ExpressionException ( $"Unexpected {this.TokenReader.Peek ( ).Raw} near {this.TokenReader.Peek ( 0 ).Raw}" );
			}
		}

		#endregion 2. Parsing

		#region 3. Entry Point

		public ASTNode Parse ( ) => this.ParseExpression ( "", -1 );

		#endregion 3. Entry Point
	}
}
