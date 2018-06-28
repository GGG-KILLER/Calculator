using System;
using Calculator.Lib.Abstractions;
using GParse.Common.AST;

namespace Calculator.Lib.AST
{
    public class ConstantExpression : CASTNode
    {
        public readonly String Identifier;

        public ConstantExpression ( String Id )
        {
            this.Identifier = Id;
        }

        public override void Accept ( ICNodeTreeVisitor visitor ) => visitor.Visit ( this );

        public override T Accept<T> ( ICNodeTreeVisitor<T> visitor ) => visitor.Visit ( this );

        public override String ToString ( ) => $"Const<{this.Identifier}>";
    }
}
