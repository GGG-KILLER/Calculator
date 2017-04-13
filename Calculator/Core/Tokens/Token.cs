using System;

namespace Calculator.Core.Tokens
{
    internal class Token
    {
        /// <summary>
        /// A constant Token
        /// </summary>
        internal class Constant : Token
        {
            public Double Value { get; protected set; }

            /// <summary>
            /// A constant number
            /// </summary>
            internal class Number : Constant
            {
                public Number ( Double Value )
                {
                    this.Value = Value;
                }

                public override String ToString ( ) => $"Number<{Value}>";
            }

            internal class MathConstant : Constant
            {
                public MathConstant ( String name )
                {
                    switch ( name.ToLower ( ) )
                    {
                        case "pi":
                        case "\u03C0":
                        case "\u03A0":
                            this.Value = Math.PI;
                            break;

                        default:
                            throw new Exception ( "Unknown constant " + name );
                    }
                }

                public override String ToString ( ) => $"Constant<{Value}>";
            }
        }

        /// <summary>
        /// An operator token
        /// </summary>
        internal class Operator : Token
        {
            /// <summary>
            /// The sum operator
            /// </summary>
            internal class Plus : Operator { public override String ToString ( ) => "+"; }

            /// <summary>
            /// The subtraction operator
            /// </summary>
            internal class Minus : Operator { public override String ToString ( ) => "-"; }
        }
    }
}
