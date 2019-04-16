namespace Calculator.Definitions
{
    /// <summary>
    /// Represents the location of an unary operator
    /// </summary>
    public enum UnaryOperatorFix
    {
        /// <summary>
        /// unaryop = op, exp
        /// </summary>
        Prefix,

        /// <summary>
        /// unaryop = exp, op
        /// </summary>
        Postfix
    }
}
