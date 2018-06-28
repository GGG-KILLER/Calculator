using System;
using Calculator.Lib.Definitions;
using GParse.Common;
using GParse.Common.AST;

namespace Calculator.Lib.AST
{
    public class UnaryOperatorExpression : ASTNode
    {
        public readonly String Operator;
        public readonly ASTNode Operand;
        public readonly UnaryOperatorFix Fix;

        public UnaryOperatorExpression ( String Operator, ASTNode Operand, UnaryOperatorFix Fix )
        {
            this.Operator = Operator;
            this.Operand = Operand;
            this.Fix = Fix;
        }

        public override String ToString ( ) => $"Unary<{this.Fix}, {this.Operator}, {this.Operand}>";
    }
}
