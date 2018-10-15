﻿using System;
using System.Collections.Generic;
using System.Linq;
using Calculator.Lexing;
using Calculator.Parsing.Abstractions;
using GParse.Common.Lexing;

namespace Calculator.Parsing.AST
{
    public class NumberExpression : CalculatorASTNode
    {
        private readonly Token<CalculatorTokenType> Token;
        public Double Value => ( Double ) this.Token.Value;

        public NumberExpression ( Token<CalculatorTokenType> token )
        {
            this.Token = token;
        }

        public override IEnumerable<Token<CalculatorTokenType>> Tokens
        {
            get
            {
                yield return this.Token;
            }
        }

        public override IEnumerable<CalculatorASTNode> Children => Enumerable.Empty<CalculatorASTNode> ( );

        public override void Accept ( ITreeVisitor visitor ) => visitor.Visit ( this );

        public override T Accept<T> ( ITreeVisitor<T> visitor ) => visitor.Visit ( this );

        public override Boolean StructurallyEquals ( CalculatorASTNode node ) =>
            node is NumberExpression numberExpression
                && this.Value.Equals ( numberExpression.Value );
    }
}
