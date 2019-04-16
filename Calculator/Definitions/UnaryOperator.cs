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
        public String Operator { get; }

        /// <summary>
        /// Works the same as in binary operators but is mainly used to decide disambiguation between
        /// prefix and suffix unary operators
        /// </summary>
        public Int32 Precedence { get; }

        /// <summary>
        /// The action performed by the operator
        /// </summary>
        public Func<Double, Double> Body { get; }

        /// <summary>
        /// Initializes a new unary operator
        /// </summary>
        /// <param name="fix"></param>
        /// <param name="operator"></param>
        /// <param name="precedence"></param>
        /// <param name="body"></param>
        public UnaryOperator ( UnaryOperatorFix fix, String @operator, Int32 precedence, Func<Double, Double> body )
        {
            this.Fix = fix;
            this.Operator = @operator?.ToLower ( ) ?? throw new ArgumentNullException ( nameof ( @operator ) );
            this.Precedence = precedence;
            this.Body = body ?? throw new ArgumentNullException ( nameof ( body ) );
        }

        #region Generated Code

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override Boolean Equals ( Object obj ) => obj is UnaryOperator && this.Equals ( ( UnaryOperator ) obj );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Boolean Equals ( UnaryOperator other ) => this.Fix == other.Fix && this.Operator == other.Operator && this.Precedence == other.Precedence && EqualityComparer<Func<Double, Double>>.Default.Equals ( this.Body, other.Body );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <returns></returns>
        public override Int32 GetHashCode ( )
        {
            var hashCode = 2002979436;
            hashCode = hashCode * -1521134295 + this.Fix.GetHashCode ( );
            hashCode = hashCode * -1521134295 + EqualityComparer<String>.Default.GetHashCode ( this.Operator );
            hashCode = hashCode * -1521134295 + this.Precedence.GetHashCode ( );
            hashCode = hashCode * -1521134295 + EqualityComparer<Func<Double, Double>>.Default.GetHashCode ( this.Body );
            return hashCode;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="operator1"></param>
        /// <param name="operator2"></param>
        /// <returns></returns>
        public static Boolean operator == ( UnaryOperator operator1, UnaryOperator operator2 ) => operator1.Equals ( operator2 );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="operator1"></param>
        /// <param name="operator2"></param>
        /// <returns></returns>
        public static Boolean operator != ( UnaryOperator operator1, UnaryOperator operator2 ) => !( operator1 == operator2 );

        #endregion Generated Code
    }
}
