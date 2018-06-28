using System;
using Calculator.Lib.Abstractions;

namespace Calculator.Lib.AST
{
    public class BinaryOperatorExpression : CASTNode
    {
        public readonly CASTNode LeftHandSide;
        public readonly CASTNode RightHandSide;
        public readonly String Operator;

        public BinaryOperatorExpression ( String Operator, CASTNode lhs, CASTNode rhs )
        {
            this.Operator = Operator;
            this.LeftHandSide = lhs;
            this.RightHandSide = rhs;
        }

        public override void Accept ( ICNodeTreeVisitor visitor ) => visitor.Visit ( this );

        public override T Accept<T> ( ICNodeTreeVisitor<T> visitor ) => visitor.Visit ( this );

        public override String ToString ( ) => $"BinOp<{this.LeftHandSide}, {this.Operator}, {this.RightHandSide}>";
    }
}
