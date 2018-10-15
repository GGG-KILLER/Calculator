using System;

namespace Calculator.Definitions
{
    public readonly struct ConstantDef
    {
        /// <summary>
        /// The constant identifier (such as PI or E)
        /// </summary>
        public readonly String Identifier;

        /// <summary>
        /// Whether the constant's identifier is case sensitive
        /// </summary>
        public readonly Boolean IsCaseSensitive;

        /// <summary>
        /// The constant value
        /// </summary>
        public readonly Double Value;

        public ConstantDef ( String ident, Boolean isCaseSensitive, Double val )
        {
            this.Identifier      = ident;
            this.IsCaseSensitive = isCaseSensitive;
            this.Value           = val;
        }
    }
}
