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

		public static IEnumerable<Token> Process ( String Input, Action<String> Log = null )
		{
			return new Lexer ( Input )
				.Process ( Log );
		}

		private IEnumerable<Token> Process ( Action<String> Log )
		{
			this.Tokens = new List<Token> ( );

			Char ch = this._reader.Peek ( );
			do
			{
				Log?.Invoke ( $"In char {ch}" );
				if ( Char.IsDigit ( ch ) || ch == '.' )
				{
					Log?.Invoke ( "Reading number..." );
					// Sets the new token as the last as we add it
					// onto the list
					this.Tokens.Add ( this.LastToken = GetNumber ( Log ) );
				}
				else if ( Char.IsLetter ( ch ) )
				{
					Log?.Invoke ( "Reading identfier..." );
					// Sets the new token as the last as we add it
					// onto the list
					this.Tokens.Add ( this.LastToken = GetIdentifier ( ) );
				}
				// + and - will always be operators unfortunately
				//   as they need to work with unary operations
				else if ( ch == '+' || ch == '-' )
				{
					Log?.Invoke ( $"Reading operator {ch}..." );
					var op = this._reader.Read ( );
					// Sets the new token as the last as we add it
					// onto the list
					this.Tokens.Add ( this.LastToken = new Token (
						this.LastToken.IsPossibleValue ( ) ? TokenType.BinaryOp : TokenType.UnaryOp,
						op.ToString ( )
					) );
				}
				else if ( Language.IsOperator ( ch ) )
				{
					var op = this._reader.Read ( ).ToString ( );
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

		//ident	::= letter, { letter | number } ;
		private Token GetIdentifier ( )
			=> new Token ( TokenType.Identifier, this._reader.ReadWhile ( Char.IsLetterOrDigit ) );

		//number	::= { digit }, [ ".",  { digit } ] | ".", { digit };
		private Token GetNumber ( Action<String> Log )
		{
			var num = new StringBuilder ( );

			Log?.Invoke ( "Reading first part of number..." );
			Log?.Invoke ( $"Peek ( ) == {this._reader.Peek ( )}" );
			num.Append ( this._reader.ReadWhile ( Char.IsNumber ) );

			if ( this._reader.Peek ( ) == '.' )
			{
				Log?.Invoke ( "Reading floating point part..." );
				num.Append ( this._reader.Read ( ) );
				Log?.Invoke ( "Read dot. Reading rest of the number..." );
				Log?.Invoke ( $"Peek ( ) == {this._reader.Peek ( )}" );
				num.Append ( this._reader.ReadWhile ( ch =>
				{
					Log?.Invoke ( $"Checking {ch} for number-ish" );
					return Char.IsNumber ( ch );
				} ) );
				Log?.Invoke ( "Read number." );
			}

			if ( num.ToString ( ) == "" || num.ToString ( ) == "." )
				throw new Exception ( $"Unexpected \"{num}\"." );

			Log?.Invoke ( $"Entire number: {num}" );
			try
			{
				return new Token ( TokenType.Number, num.ToString ( ) );
			}
			finally
			{
				Log?.Invoke ( "Returned token." );
			}
		}
	}
}
