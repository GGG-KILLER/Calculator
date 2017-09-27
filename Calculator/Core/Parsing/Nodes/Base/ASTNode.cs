using System;

namespace Calculator.Core.Parsing.Nodes.Base
{
	public abstract class ASTNode
	{
		/// <summary>
		/// The raw contents of the expression
		/// </summary>
		public String Raw { get; protected set; }

		/// <summary>
		/// Resolves the expression
		/// </summary>
		/// <returns></returns>
		public abstract ASTNode Resolve ( );

		public override String ToString ( )
		{
			return $"{this.GetType ( ).Name}<{this.Raw}>";
		}
	}
}
