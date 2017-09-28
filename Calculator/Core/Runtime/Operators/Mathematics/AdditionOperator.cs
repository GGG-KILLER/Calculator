using Calculator.Core.Runtime.Base;
using System;

namespace Calculator.Core.Runtime.Operators.Mathematics
{
	public class AdditionOperator : Operator
	{
		public AdditionOperator ( Int32 Priority, Associativity associativity ) : base ( Priority, associativity )
		{
		}

		public override Double Solve ( Double lhs, Double rhs ) => lhs + rhs;
	}
}
