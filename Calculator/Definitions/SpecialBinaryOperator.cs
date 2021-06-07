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
        public SpecialBinaryOperator(SpecialBinaryOperatorType type, Associativity associativity, int precedence, Func<double, double, double> body)
        {
            Type = type;
            Precedence = precedence;
            Associativity = associativity;
            Body = body ?? throw new ArgumentNullException(nameof(body));
        }

        /// <summary>
        /// The type of special operator
        /// </summary>
        public SpecialBinaryOperatorType Type { get; }

        /// <summary>
        /// The precedence of the special operator
        /// </summary>
        public int Precedence { get; }

        /// <summary>
        /// The associativity of the special operator
        /// </summary>
        public Associativity Associativity { get; }

        /// <summary>
        /// The body of the special operator
        /// </summary>
        public Func<double, double, double> Body { get; }

        #region Object

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is SpecialBinaryOperator @operator && Equals(@operator);

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            var hashCode = 634421626;
            hashCode = hashCode * -1521134295 + Type.GetHashCode();
            hashCode = hashCode * -1521134295 + Precedence.GetHashCode();
            hashCode = hashCode * -1521134295 + Associativity.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Func<double, double, double>>.Default.GetHashCode(Body);
            return hashCode;
        }

        #endregion Object

        #region IEquatable<SpecialBinaryOperator>

        /// <inheritdoc/>
        public bool Equals(SpecialBinaryOperator other) => Type == other.Type && Precedence == other.Precedence && Associativity == other.Associativity && EqualityComparer<Func<double, double, double>>.Default.Equals(Body, other.Body);

        #endregion IEquatable<SpecialBinaryOperator>

        /// <summary>
        /// Checks two special binary operators for equality
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(SpecialBinaryOperator left, SpecialBinaryOperator right) => left.Equals(right);

        /// <summary>
        /// Checks two special binary operators for inequality
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(SpecialBinaryOperator left, SpecialBinaryOperator right) => !(left == right);
    }
}