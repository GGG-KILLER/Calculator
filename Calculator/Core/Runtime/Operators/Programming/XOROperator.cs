﻿using Calculator.Core.Runtime.Base;
using System;

namespace Calculator.Core.Runtime.Operators.Programming
{
	internal class XOROperator : Operator
	{
		public XOROperator ( Int32 Priority, Associativity associativity ) : base ( Priority, associativity )
		{
		}

		public override Double Solve ( Double lhs, Double rhs ) => Convert.ToDouble ( Convert.ToUInt64 ( lhs ) ^ Convert.ToUInt64 ( rhs ) );
	}
}