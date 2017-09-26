using Calculator.Core.Lexing;
using Calculator.Core.Parsing.Nodes.Base;
using System;

namespace Calculator.Core.Parsing.Nodes.Literals
{
	public class ConstantLiteral : ValueExpression
	{
		public ConstantLiteral ( Token token, Sign sign ) : base ( sign )
		{
			if ( !Language.IsConstant ( token.Raw ) )
				throw new Exception ( $"Unrecognized {token.Raw}" );
			this.Value = Language.Constants[token.Raw];
			this.Raw = token.Raw;
		}

		public override String ToString ( )
		{
			return $"ConstantLiteral<{this.Raw}>";
		}
	}
}
