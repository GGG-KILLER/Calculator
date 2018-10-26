using System;
using Calculator.Parsing.Abstractions;
using GParse.Common.AST;

namespace Calculator.Parsing.AST
{
    public abstract class CalculatorASTNode : ASTNode
    {
        public abstract void Accept ( ITreeVisitor visitor );

        public abstract T Accept<T> ( ITreeVisitor<T> visitor );

        public abstract Boolean StructurallyEquals ( CalculatorASTNode node );
    }
}
