using System;
using System.Collections.Generic;
using System.Text;

namespace Calculator.Lexing
{
    public enum CalculatorTokenType
    {
        EndOfExpression,
        Number,
        Identifier,
        Keyword,
        Operator,
        LParen,
        RParen,
        Comma,
        Whitespace
    }
}
