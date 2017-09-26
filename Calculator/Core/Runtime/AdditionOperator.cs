using System;

namespace Calculator.Core.Runtime
{
	public class AdditionOperator : Operator
	{
		public override Double Solve ( Double lhs, Double rhs ) => lhs + rhs;
	}
}
