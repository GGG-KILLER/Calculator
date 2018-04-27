using System;

namespace Calculator.Runtime.Definitions
{
    /// <summary>
    /// Operator associativity
    /// </summary>
    public enum OperatorAssociativity
    {
        /// <summary>
        /// a &lt;op&gt; b &lt;op&gt; c === (a &lt;op&gt; b)
        /// &lt;op&gt; c
        /// </summary>
        Left,

        /// <summary>
        /// a &lt;op&gt; b &lt;op&gt; c === a &lt;op&gt; (b
        /// &lt;op&gt; c)
        /// </summary>
        Right
    }

    public struct BinaryOperatorDef
    {
        /// <summary>
        /// Operator associativity
        /// </summary>
        public OperatorAssociativity Associativity;

        /// <summary>
        /// The operator itself
        /// </summary>
        public String Operator;

        /// <summary>
        /// Operator precedence (higher = executed first)
        /// </summary>
        public Int32 Precedence;

        /// <summary>
        /// The action performed by the operator
        /// </summary>
        public Func<Double, Double, Double> Action;

        public BinaryOperatorDef ( OperatorAssociativity associativity, String @operator, Int32 precedence, Func<Double, Double, Double> action )
        {
            this.Associativity = associativity;
            this.Operator = @operator ?? throw new ArgumentNullException ( nameof ( @operator ) );
            this.Precedence = precedence;
            this.Action = action ?? throw new ArgumentNullException ( nameof ( action ) );
        }
    }
}
