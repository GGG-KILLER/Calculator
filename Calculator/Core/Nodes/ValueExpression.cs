using System;

namespace Calculator.Core.Nodes
{
	public abstract class ValueExpression : ASTNode
	{
		public Double Value { get; set; }

		public override ASTNode Resolve ( )
		{
			return this;
		}
	}
}
