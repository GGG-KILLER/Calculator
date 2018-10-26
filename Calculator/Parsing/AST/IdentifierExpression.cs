using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Calculator.Lexing;
using Calculator.Parsing.Abstractions;
using GParse.Common.Lexing;

namespace Calculator.Parsing.AST
{
    public class IdentifierExpression : CalculatorASTNode
    {
        public readonly Token<CalculatorTokenType> Identifier;

        public IdentifierExpression ( Token<CalculatorTokenType> ident )
        {
            this.Identifier = ident;
        }

        public override IEnumerable<CalculatorASTNode> Children => Enumerable.Empty<CalculatorASTNode> ( );

        public override void Accept ( ITreeVisitor visitor ) => visitor.Visit ( this );

        public override T Accept<T> ( ITreeVisitor<T> visitor ) => visitor.Visit ( this );

        public override Boolean StructurallyEquals ( CalculatorASTNode node ) =>
            node is IdentifierExpression identifierExpression
                && this.Identifier.Raw.Equals ( identifierExpression.Identifier.Raw );
    }
}
