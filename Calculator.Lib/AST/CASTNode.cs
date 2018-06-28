using System;
using System.Collections.Generic;
using System.Text;
using Calculator.Lib.Abstractions;
using GParse.Common.AST;

namespace Calculator.Lib.AST
{
    public abstract class CASTNode : ASTNode
    {
        public abstract void Accept ( ICNodeTreeVisitor visitor );
        public abstract T Accept<T> ( ICNodeTreeVisitor<T> visitor );
    }
}
