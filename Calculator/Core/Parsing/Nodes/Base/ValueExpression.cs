using Calculator.Core.Parsing.Nodes.Literals;
using System;

namespace Calculator.Core.Parsing.Nodes.Base
{
	public abstract class ValueExpression : ASTNode
	{
		public Double Value { get; set; }

		public Sign Sign { get; set; }

		protected ValueExpression ( Sign Sign )
		{
			this.Sign = Sign;
		}

		public override ASTNode Resolve ( )
		{
			if ( this.Sign == Sign.Negative )
				return new NumericLiteral ( -this.Value );
			return this;
		}
	}
}
