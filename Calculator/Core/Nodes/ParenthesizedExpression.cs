namespace Calculator.Core.Nodes
{
	internal class ParenthesizedExpression : ASTNode
	{
		public ASTNode Value { get; protected set; }

		public ParenthesizedExpression ( ASTNode Value )
		{
			this.Value = Value;
		}

		public override ASTNode Resolve ( ) => this.Value.Resolve ( );
	}
}
