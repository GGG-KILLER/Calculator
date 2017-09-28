using Calculator.Core.Runtime.Base;
using System;

namespace Calculator.Core.Runtime.Operators.Mathematics
{
	internal class ModuloOperator : Operator
	{
		public ModuloOperator ( Int32 Priority, Associativity associativity ) : base ( Priority, associativity )
		{
		}

		public override Double Solve ( Double lhs, Double rhs ) => lhs % rhs;
	}
}
