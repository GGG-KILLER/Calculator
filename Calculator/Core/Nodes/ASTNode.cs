using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.Core.Nodes
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
