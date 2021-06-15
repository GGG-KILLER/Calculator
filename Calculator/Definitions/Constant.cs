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
        public string Identifier { get; }

        /// <summary>
        /// The constant value
        /// </summary>
        public double Value { get; }

        /// <summary>
        /// Initializes a constant
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="value"></param>
        public Constant(string identifier, double value)
        {
            Identifier = identifier;
            Value = value;
        }

        /// <summary>
        /// Returns a new constant definitions with the new value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Constant WithNewValue(double value) =>
            new Constant(Identifier, value);

        #region Generated Code

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) =>
            obj is Constant constant && Equals(constant);

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Constant other) =>
            StringComparer.OrdinalIgnoreCase.Equals(Identifier, other.Identifier)
            && Value == other.Value;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            var hashCode = -1766374309;
            hashCode = hashCode * -1521134295 + StringComparer.OrdinalIgnoreCase.GetHashCode(Identifier);
            hashCode = hashCode * -1521134295 + Value.GetHashCode();
            return hashCode;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="constant1"></param>
        /// <param name="constant2"></param>
        /// <returns></returns>
        public static bool operator ==(Constant constant1, Constant constant2) => constant1.Equals(constant2);

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="constant1"></param>
        /// <param name="constant2"></param>
        /// <returns></returns>
        public static bool operator !=(Constant constant1, Constant constant2) => !(constant1 == constant2);

        #endregion Generated Code
    }
}