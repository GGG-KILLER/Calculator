using Calculator.Core.Runtime.Base;
using System;

namespace Calculator.Core.Runtime.Operators.Mathematics
{
	public class DivisionOperator : Operator
	{
		public DivisionOperator ( Int32 Priority, Associativity associativity ) : base ( Priority, associativity )
		{
		}

		public override Double Solve ( Double lhs, Double rhs ) => lhs / rhs;
	}
}
