using Calculator.Core.Tokens;
using System;
using System.Collections.Generic;

namespace Calculator.Core.Nodes
{
	public class ConstantValue : ValueExpression
	{
		public static readonly IDictionary<String, Double> Values = new Dictionary<String, Double> ( );

		public ConstantValue ( Token token )
		{
			this.Value = Values[token.Raw];
		}
	}
}
