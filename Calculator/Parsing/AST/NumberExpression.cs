using System;
using Calculator.Lexing;
using Calculator.Parsing.Abstractions;
using GParse.Common.Lexing;

namespace Calculator.Parsing.AST
{
    public class NumberExpression : CalculatorASTNode
    {
        private readonly Token<CalculatorTokenType> Token;
        public readonly Double Value;

        public NumberExpression ( Token<CalculatorTokenType> token )
        {
            this.Token = token;
            this.Value = ( Double ) token.Value;
        }

        public override void Accept ( ITreeVisitor visitor ) => visitor.Visit ( this );

        public override T Accept<T> ( ITreeVisitor<T> visitor ) => visitor.Visit ( this );

        public override Boolean StructurallyEquals ( CalculatorASTNode node ) =>
            node is NumberExpression numberExpression
                && this.Value.Equals ( numberExpression.Value );
    }
}
