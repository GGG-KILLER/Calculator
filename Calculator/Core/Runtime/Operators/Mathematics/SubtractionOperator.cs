using Calculator.Core.Runtime.Base;
using System;

namespace Calculator.Core.Runtime.Operators.Mathematics
{
	public class SubtractionOperator : Operator
	{
		public SubtractionOperator ( Int32 Priority, Associativity associativity ) : base ( Priority, associativity )
		{
		}

		public override Double Solve ( Double lhs, Double rhs ) => lhs - rhs;
	}
}
