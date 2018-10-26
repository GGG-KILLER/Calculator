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
