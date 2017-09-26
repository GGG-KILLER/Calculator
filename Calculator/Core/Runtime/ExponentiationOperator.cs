using System;

namespace Calculator.Core.Runtime
{
	public class ExponentiationOperator : Operator
	{
		public override Double Solve ( Double lhs, Double rhs )
		{
			return Math.Pow ( lhs, rhs );
		}
	}
}
