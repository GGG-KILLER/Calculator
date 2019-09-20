using System;
using System.Collections.Generic;

namespace Calculator.Definitions
{
    /// <summary>
    /// Represents a constant
    /// </summary>
    public readonly struct Constant : IEquatable<Constant>
    {
        /// <summary>
        /// The constant identifier (such as PI or E)
        /// </summary>
        public String Identifier { get; }

        /// <summary>
        /// The constant value
        /// </summary>
        public Double Value { get; }

        /// <summary>
        /// Initializes a constant
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="value"></param>
        public Constant ( String identifier, Double value )
        {
            this.Identifier = identifier.ToLower ( );
            this.Value      = value;
        }

        /// <summary>
        /// Returns a new constant definitions with the new value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Constant WithNewValue ( Double value ) =>
            new Constant ( this.Identifier, value );

        #region Generated Code

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override Boolean Equals ( Object obj ) => obj is Constant && this.Equals ( ( Constant ) obj );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Boolean Equals ( Constant other ) =>
            this.Identifier == other.Identifier
            && this.Value == other.Value;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <returns></returns>
        public override Int32 GetHashCode ( )
        {
            var hashCode = -1766374309;
            hashCode = hashCode * -1521134295 + EqualityComparer<String>.Default.GetHashCode ( this.Identifier );
            hashCode = hashCode * -1521134295 + this.Value.GetHashCode ( );
            return hashCode;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="constant1"></param>
        /// <param name="constant2"></param>
        /// <returns></returns>
        public static Boolean operator == ( Constant constant1, Constant constant2 ) => constant1.Equals ( constant2 );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="constant1"></param>
        /// <param name="constant2"></param>
        /// <returns></returns>
        public static Boolean operator != ( Constant constant1, Constant constant2 ) => !( constant1 == constant2 );

        #endregion Generated Code
    }
}
