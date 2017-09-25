namespace Calculator.Core.Nodes
{
	public class AdditionOperatorExpression : BinaryOperatorExpression
	{
		public AdditionOperatorExpression ( ASTNode LeftHandSide, ASTNode RightHandSide ) : base ( LeftHandSide, RightHandSide )
		{
		}

		public override ASTNode Resolve ( )
			=> new NumberLiteral (
				( ( NumberLiteral ) this.LeftHandSide.Resolve ( ) ).Value +
				( ( NumberLiteral ) this.RightHandSide.Resolve ( ) ).Value );
	}
}
