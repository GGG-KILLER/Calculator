using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Calculator.Core.Lexing
{
	internal class Lexer
	{
		private readonly GUtils.Text.StringReader _reader;
		private Token LastToken;
		private List<Token> Tokens;

		public Lexer ( String input )
		{
			this._reader = new GUtils.Text.StringReader ( input );
		}

		public static IEnumerable<Token> Process ( String Input )
		{
			return new Lexer ( Input )
				.Process ( );
		}

		private IEnumerable<Token> Process ( )
		{
			this.Tokens = new List<Token> ( );

			Char ch = this._reader.Peek ( );
			do
			{
				if ( Char.IsDigit ( ch ) || ch == '.' )
				{
					// Sets the new token as the last as we add it
					// onto the list
					this.Tokens.Add ( this.LastToken = GetNumber ( ) );
				}
				else if ( Char.IsLetter ( ch ) )
				{
					// Sets the new token as the last as we add it
					// onto the list
					this.Tokens.Add ( this.LastToken = GetIdentifier ( ) );
				}
				// + and - will always be unary operators as they
				//   need to work with unary operations
				else if ( ( ch == '+' || ch == '-' ) && ( this.LastToken == null || !this.LastToken.IsPossibleValue ( ) ) )
				{
					var op = this._reader.ReadString ( 1 );
					// We'll only consider - unary operators as +
					// is virtually useless
					if ( op == "-" )
					{
						// If last was a - too, then remove the
						// last token and ignore current one and
						// continue the lexing loop
						if ( this.LastToken != null && this.LastToken.Type == TokenType.UnaryOp && this.LastToken.Raw == "-" )
						{
							this.Tokens.RemoveAt ( this.Tokens.Count - 1 );
							this.LastToken = this.Tokens.Count > 1
								? this.Tokens[this.Tokens.Count - 1]
								: null;
							continue;
						}
						// Otherwise add the - to the token list
						this.Tokens.Add ( this.LastToken = new Token ( TokenType.UnaryOp, op ) );
					}
				}
				else if ( Language.IsOperator ( ch ) )
				{
					var op = this._reader.ReadString ( 1 );
					this.Tokens.Add ( this.LastToken = new Token ( TokenType.BinaryOp, op ) );
				}
				else
				{
					switch ( ch )
					{
						case ' ':
							this._reader.Read ( );
							break;

						case '(':
							this.Tokens.Add ( this.LastToken = new Token ( TokenType.LParen,
								this._reader.ReadString ( 1 ) ) );
							break;

						case ')':
							this.Tokens.Add ( this.LastToken = new Token ( TokenType.RParen,
								this._reader.ReadString ( 1 ) ) );
							break;

						default:
							throw new InvalidDataException ( $"Unexpected \"{ch}\" after \"{this.LastToken.Raw}\"" );
					}
				}
			}
			while ( ( ch = this._reader.Peek ( ) ) != '\0' );

			return this.Tokens;
		}

		//ident	 ::= letter, { letter | number } ;
		private Token GetIdentifier ( )
		{
			return new Token ( TokenType.Identifier, this._reader.ReadWhile ( Char.IsLetterOrDigit ) );
		}

		//number ::= { digit }, [ ".",  { digit } ] | ".", { digit };
		private Token GetNumber ( )
		{
			var num = new StringBuilder ( );

			num.Append ( this._reader.ReadWhile ( Char.IsNumber ) );

			if ( this._reader.Peek ( ) == '.' )
			{
				num.Append ( this._reader.Read ( ) );
				num.Append ( this._reader.ReadWhile ( Char.IsNumber ) );
			}

			if ( num.ToString ( ) == "" || num.ToString ( ) == "." )
				throw new Exception ( $"Unexpected \"{num}\"." );
			return new Token ( TokenType.Number, num.ToString ( ) );
		}
	}
}
