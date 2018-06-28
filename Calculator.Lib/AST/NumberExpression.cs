using System;
using Calculator.Lib.Abstractions;

namespace Calculator.Lib.AST
{
    public class NumberExpression : CASTNode
    {
        public readonly Double Value;

        public NumberExpression ( Double num )
        {
            this.Value = num;
        }

        public override void Accept ( ICNodeTreeVisitor visitor ) => visitor.Visit ( this );

        public override T Accept<T> ( ICNodeTreeVisitor<T> visitor ) => visitor.Visit ( this );

        public override String ToString ( ) => $"Num<{this.Value}>";
    }
}
