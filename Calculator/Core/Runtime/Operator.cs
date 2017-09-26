using System;
using Calculator.Core.Parsing.Nodes.Base;

namespace Calculator.Core.Runtime
{
	public abstract class Operator
	{
		/// <summary>
		/// The actual priority of the operator
		/// </summary>
		public Int32 OwnPriority;

		/// <summary>
		/// Prevents infinite loops
		/// </summary>
		public Int32 BackupPriority;

		/// <summary>
		/// Solves the operation
		/// </summary>
		/// <param name="lhs">Left hand side</param>
		/// <param name="rhs">Right hand side</param>
		/// <returns></returns>
		public abstract Double Solve ( Double lhs, Double rhs );

		internal ASTNode Solve ( ASTNode leftHandSide, ASTNode rightHandSide )
		{
			throw new NotImplementedException ( );
		}
	}
}