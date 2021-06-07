using System.Collections.Generic;
using Calculator.Definitions;

namespace Calculator
{
    /// <summary>
    /// The <see cref="IEqualityComparer{T}"/> used for the keys in the <see cref="System.Collections.Immutable.IImmutableDictionary{TKey, TValue}"/>
    /// </summary>
    public class UnaryOperatorKeyPairEqualityComparer : IEqualityComparer<(UnaryOperatorFix fix, string op)>
    {
        private readonly IEqualityComparer<string> _stringComparer;

        /// <summary>
        /// Initializes this <see cref="UnaryOperatorKeyPairEqualityComparer"/> with the provided <see cref="IEqualityComparer{T}"/>
        /// </summary>
        /// <param name="stringComparer"></param>
        public UnaryOperatorKeyPairEqualityComparer(IEqualityComparer<string> stringComparer) => this._stringComparer = stringComparer;

        /// <inheritdoc/>
        public bool Equals((UnaryOperatorFix fix, string op) x, (UnaryOperatorFix fix, string op) y) =>
            x.fix == y.fix && _stringComparer.Equals(x.op, y.op);

        /// <inheritdoc/>
        public int GetHashCode((UnaryOperatorFix fix, string op) obj) =>
            unchecked(obj.fix.GetHashCode() * 31 + _stringComparer.GetHashCode(obj.op));
    }
}