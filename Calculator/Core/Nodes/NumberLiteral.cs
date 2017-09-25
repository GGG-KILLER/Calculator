using Calculator.Core.Tokens;
using System;

namespace Calculator.Core.Nodes
{
	public class NumberLiteral : ValueExpression
	{
		internal NumberLiteral ( Token token )
		{
			if ( token.Type != TokenType.Number )
				throw new ArgumentException ( "Token should have a Number type.", nameof ( token ) );

			this.Value = Double.Parse ( token.Raw );
		}

		internal NumberLiteral ( Double value )
		{
			this.Value = value;
		}

		public override String ToString ( )
		{
			return $"NumberLiteral<{this.Value}>";
		}
	}
}
