using System;

namespace Calculator.Definitions
{
    /// <summary>
    /// Represents a binary operator that cannot be expressed with a string because of its behavior
    /// (like implicit multiplication or exponentiation by superscript)
    /// </summary>
    public readonly struct SpecialBinaryOperator
    {
        /// <summary>
        /// Initializes this <see cref="SpecialBinaryOperator"/> with the provided information
        /// </summary>
        /// <param name="type"></param>
        /// <param name="associativity"></param>
        /// <param name="precedence"></param>
        /// <param name="body"></param>
        public SpecialBinaryOperator ( SpecialBinaryOperatorType type, Associativity associativity, Int32 precedence, Func<Double, Double, Double> body )
        {
            this.Type = type;
            this.Precedence = precedence;
            this.Associativity = associativity;
            this.Body = body ?? throw new ArgumentNullException ( nameof ( body ) );
        }

        /// <summary>
        /// The type of special operator
        /// </summary>
        public SpecialBinaryOperatorType Type { get; }

        /// <summary>
        /// The precedence of the special operator
        /// </summary>
        public Int32 Precedence { get; }

        /// <summary>
        /// The associativity of the special operator
        /// </summary>
        public Associativity Associativity { get; }

        /// <summary>
        /// The body of the special operator
        /// </summary>
        public Func<Double, Double, Double> Body { get; }
    }
}