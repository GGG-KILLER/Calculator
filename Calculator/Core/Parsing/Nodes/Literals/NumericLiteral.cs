using Calculator.Core.Lexing;
using Calculator.Core.Parsing.Nodes.Base;
using System;

namespace Calculator.Core.Parsing.Nodes.Literals
{
	public class NumericLiteral : ValueExpression
	{
		internal NumericLiteral ( Token token, Sign sign ) : base ( sign )
		{
			if ( token.Type != TokenType.Number )
				throw new ArgumentException ( "Token should have a Number type.", nameof ( token ) );

			this.Value = Double.Parse ( token.Raw );
		}

		internal NumericLiteral ( Double value ) : base ( Sign.Nothing )
		{
			this.Value = value;
		}

		public override String ToString ( )
		{
			return $"NumberLiteral<{this.Value}>";
		}
	}
}
