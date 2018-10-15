using System;

namespace Calculator.Definitions
{
    /// <summary>
    /// Operator associativity
    /// </summary>
    public enum OperatorAssociativity
    {
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

        /// <summary>
        /// Constitutes an error when used in sequence
        /// </summary>
        None
    }

    public readonly struct BinaryOperatorDef
    {
        /// <summary>
        /// Operator associativity
        /// </summary>
        public readonly OperatorAssociativity Associativity;

        /// <summary>
        /// The operator itself
        /// </summary>
        public readonly String Operator;

        /// <summary>
        /// Operator precedence (higher = executed first)
        /// </summary>
        public readonly Int32 Precedence;

        /// <summary>
        /// The action performed by the operator
        /// </summary>
        public readonly Func<Double, Double, Double> Action;

        public BinaryOperatorDef ( OperatorAssociativity associativity, String @operator, Int32 precedence, Func<Double, Double, Double> action )
        {
            this.Associativity = associativity;
            this.Operator = @operator ?? throw new ArgumentNullException ( nameof ( @operator ) );
            this.Precedence = precedence;
            this.Action = action ?? throw new ArgumentNullException ( nameof ( action ) );
        }
    }
}
