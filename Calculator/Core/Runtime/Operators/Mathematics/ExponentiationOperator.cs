using Calculator.Core.Runtime.Base;
using System;

namespace Calculator.Core.Runtime.Operators.Mathematics
{
	public class ExponentiationOperator : Operator
	{
		public ExponentiationOperator ( Int32 Priority, Associativity associativity ) : base ( Priority, associativity )
		{
		}

		public override Double Solve ( Double lhs, Double rhs ) => Math.Pow ( lhs, rhs );
	}
}
