using System;

namespace Calculator.Core.Runtime.Base
{
	public enum Associativity
	{
		Left,
		Right
	}

	public abstract class Operator
	{
		/// <summary>
		/// The associativity of the operator
		/// </summary>
		public readonly Associativity Associativity;

		/// <summary>
		/// The actual priority of the operator
		/// </summary>
		public readonly Int32 Priority;

		/// <summary>
		/// Priority used for associativity
		/// </summary>
		public readonly Int32 AssociativityPriority;

		protected Operator ( Int32 Priority, Associativity associativity )
		{
			this.Priority = Priority;
			this.Associativity = associativity;
		}

		/// <summary>
		/// Solves the operation
		/// </summary>
		/// <param name="lhs">Left hand side</param>
		/// <param name="rhs">Right hand side</param>
		/// <returns></returns>
		public abstract Double Solve ( Double lhs, Double rhs );
	}
}