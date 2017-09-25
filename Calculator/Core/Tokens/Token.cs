using System;

namespace Calculator.Core.Tokens
{
	public enum TokenType
	{
		Number,
		Identifier,
		LParen,
		RParen,
		BinaryOp,
		UnaryOp
	}

	public class Token
	{
		public readonly TokenType Type;
		public readonly String Raw;

		public Token ( TokenType Type, String Raw )
		{
			this.Type = Type;
			this.Raw = Raw;
		}

		public Boolean IsPossibleValue ( )
		{
			return this.Type == TokenType.Number || this.Type == TokenType.RParen ||
				   this.Type == TokenType.Identifier;
		}

		public override String ToString ( )
		{
			return $"{this.Type}<{this.Raw}>";
		}
	}
}
