namespace Calculator.Lexing
{
    /// <summary>
    /// The types of token emmited by the lexer
    /// </summary>
    public enum CalculatorTokenType
    {
        /// <summary>
        /// The EOF (default value, GParse convention).
        /// </summary>
        EndOfExpression,

        /// <summary>
        /// A number.
        /// </summary>
        Number,

        /// <summary>
        /// An identifier.
        /// </summary>
        Identifier,

        /// <summary>
        /// A keyword(?, not used for now).
        /// </summary>
        Keyword,

        /// <summary>
        /// An operator.
        /// </summary>
        Operator,

        /// <summary>
        /// A left parenthesis.
        /// </summary>
        LParen,

        /// <summary>
        /// A right parenthesis.
        /// </summary>
        RParen,

        /// <summary>
        /// A comma.
        /// </summary>
        Comma,

        /// <summary>
        /// Whitespace.
        /// </summary>
        Whitespace,

        /// <summary>
        /// Superscript (⁰, ¹, ², ³, ⁴, ⁵, ⁶, ⁷, ⁸, ⁹, ⁺, ⁻).
        /// </summary>
        Superscript,

        /// <summary>
        /// An uknown character.
        /// </summary>
        Unknown,
    }
}