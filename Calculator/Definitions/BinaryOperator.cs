using System;
using System.Collections.Generic;

namespace Calculator.Definitions
{
    /// <summary>
    /// The definition of a binary operator
    /// </summary>
    public readonly struct BinaryOperator : IEquatable<BinaryOperator>
    {
        /// <summary>
        /// Operator associativity
        /// </summary>
        public Associativity Associativity { get; }

        /// <summary>
        /// The operator itself
        /// </summary>
        public string Operator { get; }

        /// <summary>
        /// Operator precedence (higher = executed first)
        /// </summary>
        public int Precedence { get; }

        /// <summary>
        /// The action performed by the operator
        /// </summary>
        public Func<double, double, double> Body { get; }

        /// <summary>
        /// Initializes a new binary operator
        /// </summary>
        /// <param name="associativity"></param>
        /// <param name="operator"></param>
        /// <param name="precedence"></param>
        /// <param name="body"></param>
        public BinaryOperator(Associativity associativity, string @operator, int precedence, Func<double, double, double> body)
        {
            if (associativity is < Associativity.None or > Associativity.Right)
                throw new ArgumentOutOfRangeException(nameof(associativity));
            if (string.IsNullOrWhiteSpace(@operator))
                throw new ArgumentException("message", nameof(@operator));

            Associativity = associativity;
            Operator = @operator;
            Precedence = precedence;
            Body = body ?? throw new ArgumentNullException(nameof(body));
        }

        #region Generated Code

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) =>
            obj is BinaryOperator op && Equals(op);

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(BinaryOperator other) =>
            Associativity == other.Associativity
            && StringComparer.OrdinalIgnoreCase.Equals(Operator, other.Operator)
            && Precedence == other.Precedence
            && EqualityComparer<Func<double, double, double>>.Default.Equals(Body, other.Body);

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() =>
            HashCode.Combine(Associativity, Operator, Precedence, Body);

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="operator1"></param>
        /// <param name="operator2"></param>
        /// <returns></returns>
        public static bool operator ==(BinaryOperator operator1, BinaryOperator operator2) => operator1.Equals(operator2);

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="operator1"></param>
        /// <param name="operator2"></param>
        /// <returns></returns>
        public static bool operator !=(BinaryOperator operator1, BinaryOperator operator2) => !(operator1 == operator2);

        #endregion Generated Code
    }
}