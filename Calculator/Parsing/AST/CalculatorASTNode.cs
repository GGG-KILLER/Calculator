using System;
using System.Collections;
using System.Collections.Generic;
using Calculator.Lexing;
using Calculator.Parsing.Abstractions;
using GParse.Common.AST;
using GParse.Common.Lexing;

namespace Calculator.Parsing.AST
{
    public abstract class CalculatorASTNode : ASTNode
    {
        public abstract IEnumerable<Token<CalculatorTokenType>> Tokens { get; }

        public abstract IEnumerable<CalculatorASTNode> Children { get; }

        public abstract void Accept ( ITreeVisitor visitor );

        public abstract T Accept<T> ( ITreeVisitor<T> visitor );

        public abstract Boolean StructurallyEquals ( CalculatorASTNode node );
    }
}
