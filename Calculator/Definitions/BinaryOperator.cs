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
        public String Operator { get; }

        /// <summary>
        /// Operator precedence (higher = executed first)
        /// </summary>
        public Int32 Precedence { get; }

        /// <summary>
        /// The action performed by the operator
        /// </summary>
        public Func<Double, Double, Double> Body { get; }

        /// <summary>
        /// Initializes a new binary operator
        /// </summary>
        /// <param name="associativity"></param>
        /// <param name="operator"></param>
        /// <param name="precedence"></param>
        /// <param name="body"></param>
        public BinaryOperator ( Associativity associativity, String @operator, Int32 precedence, Func<Double, Double, Double> body )
        {
            if ( associativity < Associativity.None || Associativity.Right < associativity )
                throw new ArgumentOutOfRangeException ( nameof ( associativity ) );
            if ( String.IsNullOrWhiteSpace ( @operator ) )
                throw new ArgumentException ( "message", nameof ( @operator ) );

            this.Associativity = associativity;
            this.Operator      = @operator.ToLower ( );
            this.Precedence    = precedence;
            this.Body          = body ?? throw new ArgumentNullException ( nameof ( body ) );
        }

        #region Generated Code

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override Boolean Equals ( Object obj ) => obj is BinaryOperator && this.Equals ( ( BinaryOperator ) obj );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Boolean Equals ( BinaryOperator other ) => this.Associativity == other.Associativity && this.Operator == other.Operator && this.Precedence == other.Precedence && EqualityComparer<Func<Double, Double, Double>>.Default.Equals ( this.Body, other.Body );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <returns></returns>
        public override Int32 GetHashCode ( )
        {
            var hashCode = 767561535;
            hashCode = hashCode * -1521134295 + this.Associativity.GetHashCode ( );
            hashCode = hashCode * -1521134295 + EqualityComparer<String>.Default.GetHashCode ( this.Operator );
            hashCode = hashCode * -1521134295 + this.Precedence.GetHashCode ( );
            hashCode = hashCode * -1521134295 + EqualityComparer<Func<Double, Double, Double>>.Default.GetHashCode ( this.Body );
            return hashCode;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="operator1"></param>
        /// <param name="operator2"></param>
        /// <returns></returns>
        public static Boolean operator == ( BinaryOperator operator1, BinaryOperator operator2 ) => operator1.Equals ( operator2 );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="operator1"></param>
        /// <param name="operator2"></param>
        /// <returns></returns>
        public static Boolean operator != ( BinaryOperator operator1, BinaryOperator operator2 ) => !( operator1 == operator2 );

        #endregion Generated Code
    }
}
