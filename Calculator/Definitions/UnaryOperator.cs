using System;
using System.Collections.Generic;

namespace Calculator.Definitions
{
    /// <summary>
    /// Represents an unary operator
    /// </summary>
    public readonly struct UnaryOperator : IEquatable<UnaryOperator>
    {
        /// <summary>
        /// The *fix-ness of the unary operator
        /// </summary>
        public UnaryOperatorFix Fix { get; }

        /// <summary>
        /// The operator itself
        /// </summary>
        public string Operator { get; }

        /// <summary>
        /// Works the same as in binary operators but is mainly used to decide disambiguation between
        /// prefix and suffix unary operators
        /// </summary>
        public int Precedence { get; }

        /// <summary>
        /// The action performed by the operator
        /// </summary>
        public Func<double, double> Body { get; }

        /// <summary>
        /// Initializes a new unary operator
        /// </summary>
        /// <param name="fix"></param>
        /// <param name="operator"></param>
        /// <param name="precedence"></param>
        /// <param name="body"></param>
        public UnaryOperator(UnaryOperatorFix fix, string @operator, int precedence, Func<double, double> body)
        {
            Fix = fix;
            Operator = @operator ?? throw new ArgumentNullException(nameof(@operator));
            Precedence = precedence;
            Body = body ?? throw new ArgumentNullException(nameof(body));
        }

        #region Generated Code

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) => obj is UnaryOperator && Equals((UnaryOperator) obj);

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(UnaryOperator other) => Fix == other.Fix && Operator == other.Operator && Precedence == other.Precedence && EqualityComparer<Func<double, double>>.Default.Equals(Body, other.Body);

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            var hashCode = 2002979436;
            hashCode = hashCode * -1521134295 + Fix.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Operator);
            hashCode = hashCode * -1521134295 + Precedence.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Func<double, double>>.Default.GetHashCode(Body);
            return hashCode;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="operator1"></param>
        /// <param name="operator2"></param>
        /// <returns></returns>
        public static bool operator ==(UnaryOperator operator1, UnaryOperator operator2) => operator1.Equals(operator2);

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="operator1"></param>
        /// <param name="operator2"></param>
        /// <returns></returns>
        public static bool operator !=(UnaryOperator operator1, UnaryOperator operator2) => !(operator1 == operator2);

        #endregion Generated Code
    }
}
