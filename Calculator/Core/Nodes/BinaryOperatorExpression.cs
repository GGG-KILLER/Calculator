using System;

namespace Calculator.Core.Nodes
{
	public abstract class BinaryOperatorExpression : ASTNode
	{
		/// <summary>
		/// Left hand side of the expression
		/// </summary>
		public ASTNode LeftHandSide { get; protected set; }

		/// <summary>
		/// Right hand side of the expression
		/// </summary>
		public ASTNode RightHandSide { get; protected set; }

		protected BinaryOperatorExpression ( ASTNode LeftHandSide, ASTNode RightHandSide )
		{
			this.LeftHandSide = LeftHandSide;
			this.RightHandSide = RightHandSide;
		}

		public override String ToString ( )
		{
			return $"{this.GetType ( ).Name}<{this.LeftHandSide} | {this.RightHandSide}>";
		}
	}
}
