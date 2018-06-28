using System;
using Calculator.Lib.Abstractions;
using Calculator.Lib.Definitions;
using GParse.Common;
using GParse.Common.AST;

namespace Calculator.Lib.AST
{
    public class UnaryOperatorExpression : CASTNode
    {
        public readonly String Operator;
        public readonly CASTNode Operand;
        public readonly UnaryOperatorFix Fix;

        public UnaryOperatorExpression ( String Operator, CASTNode Operand, UnaryOperatorFix Fix )
        {
            this.Operator = Operator;
            this.Operand = Operand;
            this.Fix = Fix;
        }

        public override void Accept ( ICNodeTreeVisitor visitor ) => visitor.Visit ( this );

        public override T Accept<T> ( ICNodeTreeVisitor<T> visitor ) => visitor.Visit ( this );

        public override String ToString ( ) => $"Unary<{this.Fix}, {this.Operator}, {this.Operand}>";
    }
}
