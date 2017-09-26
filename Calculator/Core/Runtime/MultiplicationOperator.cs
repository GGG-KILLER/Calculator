using System;

namespace Calculator.Core.Runtime
{
	public class MultiplicationOperator : Operator
	{
		public override Double Solve ( Double lhs, Double rhs )
		{
			return lhs * rhs;
		}
	}
}
