using Calculator.Core.Runtime.Base;
using System;

namespace Calculator.Core.Runtime.Operators
{
	public class MultiplicationOperator : Operator
	{
		public MultiplicationOperator ( Int32 Priority, Associativity associativity ) : base ( Priority, associativity )
		{
		}

		public override Double Solve ( Double lhs, Double rhs ) => lhs * rhs;
	}
}
