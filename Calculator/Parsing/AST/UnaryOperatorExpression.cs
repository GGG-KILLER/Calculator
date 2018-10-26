using System;
using System.Collections.Generic;
using Calculator.Definitions;
using Calculator.Lexing;
using Calculator.Parsing.Abstractions;
using GParse.Common.Lexing;

namespace Calculator.Parsing.AST
{
    public class UnaryOperatorExpression : CalculatorASTNode
    {
        public readonly Token<CalculatorTokenType> Operator;
        public readonly CalculatorASTNode Operand;
        public readonly UnaryOperatorFix OperatorFix;

        public UnaryOperatorExpression ( Token<CalculatorTokenType> Operator, CalculatorASTNode Operand, UnaryOperatorFix fix )
        {
            this.Operator    = Operator;
            this.Operand     = Operand;
            this.OperatorFix = fix;
        }

        public override IEnumerable<CalculatorASTNode> Children
        {
            get
            {
                yield return this.Operand;
            }
        }

        public override void Accept ( ITreeVisitor visitor ) => visitor.Visit ( this );

        public override T Accept<T> ( ITreeVisitor<T> visitor ) => visitor.Visit ( this );

        public override Boolean StructurallyEquals ( CalculatorASTNode node ) =>
            node is UnaryOperatorExpression unaryOperatorExpression
                && this.Operator.Raw.Equals ( unaryOperatorExpression.Operator.Raw )
                && this.OperatorFix.Equals ( unaryOperatorExpression.OperatorFix )
                && this.Operand.StructurallyEquals ( unaryOperatorExpression.Operand );
    }
}
