using Calculator.Core.Lexing;
using Calculator.Core.Parsing.Nodes.Literals;
using Calculator.Core.Runtime.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Calculator.Core.Parsing.Nodes.Base
{
	public class FunctionCallExpression : ASTNode
	{
		public readonly String Name;
		private readonly MathFunction Function;
		public readonly ASTNode[] Arguments;

		public FunctionCallExpression ( Token identifier, IEnumerable<ASTNode> args )
		{
			this.Name = identifier.Raw;
			this.Function = Language.Functions[identifier.Raw];
			this.Arguments = args.ToArray ( );
		}

		public override ASTNode Resolve ( )
		{
			return new NumericLiteral ( this.Function.Execute ( this.Arguments
				.Select ( arg => ( ( ValueExpression ) arg.Resolve ( ) ).Value )
				.ToArray ( ) ) );
		}

		public override System.String ToString ( )
		{
			return $"FunctionCallExpression<{this.Name} | {String.Join<ASTNode> ( ", ", this.Arguments )}";
		}
	}
}
