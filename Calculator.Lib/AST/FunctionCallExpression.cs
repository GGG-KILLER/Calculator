using System;
using System.Linq;
using Calculator.Lib.Abstractions;
using GParse.Common.AST;

namespace Calculator.Lib.AST
{
    public class FunctionCallExpression : CASTNode
    {
        public readonly String Identifier;
        public readonly CASTNode[] Arguments;

        public FunctionCallExpression ( String ident, CASTNode[] args )
        {
            this.Identifier = ident;
            this.Arguments = args;
        }

        public override void Accept ( ICNodeTreeVisitor visitor ) => visitor.Visit ( this );

        public override T Accept<T> ( ICNodeTreeVisitor<T> visitor ) => visitor.Visit ( this );

        public override String ToString ( ) => $"FuncCall<{this.Identifier}, {String.Join ( ", ", this.Arguments.Select ( x => x.ToString ( ) ) )}>";
    }
}
