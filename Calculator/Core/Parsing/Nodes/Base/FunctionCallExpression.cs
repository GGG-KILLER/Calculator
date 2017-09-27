using Calculator.Core.Lexing;
using Calculator.Core.Parsing.Nodes.Literals;
using Calculator.Core.Runtime.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.Core.Parsing.Nodes.Base
{
	public class FunctionCallExpression : ASTNode
	{
		private readonly MathFunction Function;
		public readonly ASTNode[] Arguments;

		public FunctionCallExpression ( Token identifier, IEnumerable<ASTNode> args )
		{
			this.Function = Language.Functions[identifier.Raw];
			this.Arguments = args.ToArray ( );
		}

		public override ASTNode Resolve ( )
		{
			return new NumericLiteral ( this.Function.Execute ( this.Arguments
				.Select ( arg => ( ( ValueExpression ) arg.Resolve ( ) ).Value )
				.ToArray ( ) ) );
		}
	}
}
