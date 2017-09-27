using Calculator.Core.Parsing.Nodes.Literals;
using Calculator.Core.Runtime.Base;
using System;

namespace Calculator.Core.Parsing.Nodes.Base
{
	public class BinaryOperatorExpression : ASTNode
	{
		/// <summary>
		/// Left hand side of the expression
		/// </summary>
		public ASTNode LeftHandSide { get; protected set; }

		/// <summary>
		/// Right hand side of the expression
		/// </summary>
		public ASTNode RightHandSide { get; protected set; }

		/// <summary>
		/// The operator of this <see cref="BinaryOperatorExpression" />
		/// </summary>
		public Operator Operator { get; protected set; }

		public BinaryOperatorExpression ( ASTNode LeftHandSide, ASTNode RightHandSide, Operator Operator )
		{
			this.LeftHandSide = LeftHandSide;
			this.RightHandSide = RightHandSide;
			this.Operator = Operator;
		}

		public override ASTNode Resolve ( )
		{
			return new NumericLiteral ( this.Operator.Solve (
				( ( ValueExpression ) this.LeftHandSide.Resolve ( ) ).Value,
				( ( ValueExpression ) this.RightHandSide.Resolve ( ) ).Value
			) );
		}

		public override String ToString ( )
		{
			return $"{this.GetType ( ).Name}<{this.LeftHandSide} | {this.RightHandSide}>";
		}
	}
}
