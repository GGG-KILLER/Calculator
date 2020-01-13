using System;
using System.Collections.Generic;

namespace Calculator.Definitions
{
    /// <summary>
    /// Represents a binary operator that cannot be expressed with a string because of its behavior
    /// (like implicit multiplication or exponentiation by superscript)
    /// </summary>
    public readonly struct SpecialBinaryOperator : IEquatable<SpecialBinaryOperator>
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

        #region Object

        /// <inheritdoc/>
        public override Boolean Equals ( Object obj ) => obj is SpecialBinaryOperator @operator && this.Equals ( @operator );

        /// <inheritdoc/>
        public override Int32 GetHashCode ( )
        {
            var hashCode = 634421626;
            hashCode = hashCode * -1521134295 + this.Type.GetHashCode ( );
            hashCode = hashCode * -1521134295 + this.Precedence.GetHashCode ( );
            hashCode = hashCode * -1521134295 + this.Associativity.GetHashCode ( );
            hashCode = hashCode * -1521134295 + EqualityComparer<Func<Double, Double, Double>>.Default.GetHashCode ( this.Body );
            return hashCode;
        }

        #endregion Object

        #region IEquatable<SpecialBinaryOperator>

        /// <inheritdoc/>
        public Boolean Equals ( SpecialBinaryOperator other ) => this.Type == other.Type && this.Precedence == other.Precedence && this.Associativity == other.Associativity && EqualityComparer<Func<Double, Double, Double>>.Default.Equals ( this.Body, other.Body );

        #endregion IEquatable<SpecialBinaryOperator>

        /// <summary>
        /// Checks two special binary operators for equality
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator == ( SpecialBinaryOperator left, SpecialBinaryOperator right ) => left.Equals ( right );

        /// <summary>
        /// Checks two special binary operators for inequality
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Boolean operator != ( SpecialBinaryOperator left, SpecialBinaryOperator right ) => !( left == right );
    }
}