namespace Calculator.Definitions
{
    /// <summary>
    /// Operator associativity
    /// </summary>
    public enum Associativity
    {
        /// <summary>
        /// Constitutes an error when used in sequence
        /// </summary>
        None,

        /// <summary>
        /// a &lt;op&gt; b &lt;op&gt; c ≡ (a &lt;op&gt; b)
        /// &lt;op&gt; c
        /// </summary>
        Left,

        /// <summary>
        /// a &lt;op&gt; b &lt;op&gt; c ≡ a &lt;op&gt; (b
        /// &lt;op&gt; c)
        /// </summary>
        Right,
    }
}
