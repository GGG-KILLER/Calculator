using Calculator.Core.Runtime.Base;
using System;

namespace Calculator.Core.Runtime.Operators.Programming
{
	internal class LShiftOperator : Operator
	{
		public LShiftOperator ( Int32 Priority, Associativity associativity ) : base ( Priority, associativity )
		{
		}

		public override Double Solve ( Double lhs, Double rhs ) => Convert.ToDouble ( Convert.ToUInt64 ( lhs ) << Convert.ToInt32 ( rhs ) );
	}
}
