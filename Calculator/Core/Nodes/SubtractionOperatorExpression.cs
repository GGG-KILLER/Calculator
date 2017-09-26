namespace Calculator.Core.Nodes
{
	public class SubtractionOperatorExpression : BinaryOperatorExpression
	{
		public SubtractionOperatorExpression ( ASTNode LeftHandSide, ASTNode RightHandSide ) : base ( LeftHandSide, RightHandSide )
		{
		}

		public override ASTNode Resolve ( )
			=> new NumberLiteral (
				( ( ValueExpression ) this.LeftHandSide.Resolve ( ) ).Value -
				( ( ValueExpression ) this.RightHandSide.Resolve ( ) ).Value );
	}
}
