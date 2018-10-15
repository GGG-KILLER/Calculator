using System;
using System.Collections.Generic;
using Calculator.Lexing;
using Calculator.Parsing.Abstractions;
using GParse.Common.Lexing;

namespace Calculator.Parsing.AST
{
    public class BinaryOperatorExpression : CalculatorASTNode
    {
        public readonly CalculatorASTNode LeftHandSide;
        public readonly CalculatorASTNode RightHandSide;
        public readonly Token<CalculatorTokenType> Operator;

        public BinaryOperatorExpression ( Token<CalculatorTokenType> Operator, CalculatorASTNode lhs, CalculatorASTNode rhs )
        {
            this.Operator = Operator;
            this.LeftHandSide = lhs;
            this.RightHandSide = rhs;
        }

        public override IEnumerable<Token<CalculatorTokenType>> Tokens
        {
            get
            {
                foreach ( Token<CalculatorTokenType> token in this.LeftHandSide.Tokens )
                    yield return token;

                yield return this.Operator;

                foreach ( Token<CalculatorTokenType> token in this.RightHandSide.Tokens )
                    yield return token;
            }
        }

        public override IEnumerable<CalculatorASTNode> Children
        {
            get
            {
                yield return this.LeftHandSide;
                yield return this.RightHandSide;
            }
        }

        public override void Accept ( ITreeVisitor visitor ) => visitor.Visit ( this );

        public override T Accept<T> ( ITreeVisitor<T> visitor ) => visitor.Visit ( this );

        public override Boolean StructurallyEquals ( CalculatorASTNode node ) =>
            node is BinaryOperatorExpression binaryOperatorExpression
                && this.Operator.Raw.Equals ( binaryOperatorExpression.Operator.Raw )
                && this.LeftHandSide.StructurallyEquals ( binaryOperatorExpression.LeftHandSide )
                && this.RightHandSide.StructurallyEquals ( binaryOperatorExpression.RightHandSide );
    }
}
