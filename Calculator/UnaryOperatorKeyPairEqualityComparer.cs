using System;
using System.Collections.Generic;
using Calculator.Definitions;

namespace Calculator
{
    /// <summary>
    /// The <see cref="IEqualityComparer{T}"/> used for the keys in the <see cref="System.Collections.Immutable.IImmutableDictionary{TKey, TValue}"/>
    /// </summary>
    public class UnaryOperatorKeyPairEqualityComparer : IEqualityComparer<(UnaryOperatorFix fix, String op)>
    {
        private readonly IEqualityComparer<String> stringComparer;

        /// <summary>
        /// Initializes this <see cref="UnaryOperatorKeyPairEqualityComparer"/> with the provided <see cref="IEqualityComparer{T}"/>
        /// </summary>
        /// <param name="stringComparer"></param>
        public UnaryOperatorKeyPairEqualityComparer ( IEqualityComparer<String> stringComparer )
        {
            this.stringComparer = stringComparer;
        }

        /// <inheritdoc/>
        public Boolean Equals ( (UnaryOperatorFix fix, String op) x, (UnaryOperatorFix fix, String op) y ) =>
            x.fix == y.fix && this.stringComparer.Equals ( x.op, y.op );

        /// <inheritdoc/>
        public Int32 GetHashCode ( (UnaryOperatorFix fix, String op) obj ) =>
            unchecked(obj.fix.GetHashCode ( ) * 31 + this.stringComparer.GetHashCode ( obj.op ));
    }
}