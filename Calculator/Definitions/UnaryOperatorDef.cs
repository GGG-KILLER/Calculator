using System;

namespace Calculator.Definitions
{
    public enum UnaryOperatorFix
    {
        /// <summary>
        /// unaryop = op, exp
        /// </summary>
        Prefix,

        /// <summary>
        /// unaryop = exp, op
        /// </summary>
        Postfix
    }

    public readonly struct UnaryOperatorDef
    {
        /// <summary>
        /// The *fix-ness of the unary operator
        /// </summary>
        public readonly UnaryOperatorFix Fix;

        /// <summary>
        /// The operator itself
        /// </summary>
        public readonly String Operator;

        /// <summary>
        /// Works the same as in binary operators but is mainly
        /// used to decide disambiguation between prefix and
        /// suffix unary operators
        /// </summary>
        public readonly Int32 Precedence;

        /// <summary>
        /// The action performed by the operator
        /// </summary>
        public readonly Func<Double, Double> Action;

        public UnaryOperatorDef ( UnaryOperatorFix fix, String @operator, Int32 precedence, Func<Double, Double> action )
        {
            this.Fix = fix;
            this.Operator = @operator ?? throw new ArgumentNullException ( nameof ( @operator ) );
            this.Precedence = precedence;
            this.Action = action ?? throw new ArgumentNullException ( nameof ( action ) );
        }
    }
}
