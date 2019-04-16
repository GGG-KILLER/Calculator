using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calculator.Definitions;

namespace Calculator.UI
{
    public static class CalculatorLanguageSingleton
    {
        static CalculatorLanguageSingleton ( )
        {
            Instance = new CalculatorLanguageBuilder ( )
                .SetConstant ( "E", Math.E )
                .SetConstant ( "pi", Math.PI )
                .SetConstant ( "π", Math.PI )
                .SetUnaryOperator ( UnaryOperatorFix.Prefix, "-", 1, n => -n )
                .SetUnaryOperator ( UnaryOperatorFix.Prefix, "~", 1, n => ~( Int64 ) n )
                .SetUnaryOperator ( UnaryOperatorFix.Postfix, "!", 1, n =>
                {
                    if ( Double.IsInfinity ( n ) )
                        return n;
                    var r = 1D;
                    for ( var i = 2; i < n && !Double.IsInfinity ( r ) && !Double.IsInfinity ( i ); i++ )
                        r *= i;
                    return r;
                } )
                .SetBinaryOperator ( OperatorAssociativity.Left, "+", 1, ( lhs, rhs ) => lhs + rhs )
                .SetBinaryOperator ( OperatorAssociativity.Left, "-", 1, ( lhs, rhs ) => lhs - rhs )
                .SetBinaryOperator ( OperatorAssociativity.Left, "*", 2, ( lhs, rhs ) => lhs * rhs )
                .SetBinaryOperator ( OperatorAssociativity.Left, "/", 2, ( lhs, rhs ) => lhs / rhs )
                .SetBinaryOperator ( OperatorAssociativity.Left, "%", 2, ( lhs, rhs ) => lhs % rhs )
                .SetBinaryOperator ( OperatorAssociativity.Right, "^", 3, ( lhs, rhs ) => Math.Pow ( lhs, rhs ) )
                .SetBinaryOperator ( OperatorAssociativity.Left, "<<", 4, ( lhs, rhs ) => ( Int64 ) lhs << ( Int32 ) rhs )
                .SetBinaryOperator ( OperatorAssociativity.Left, ">>", 4, ( lhs, rhs ) => ( Int64 ) lhs >> ( Int32 ) rhs )
                .SetBinaryOperator ( OperatorAssociativity.Left, "&", 5, ( lhs, rhs ) => ( Int64 ) lhs & ( Int64 ) rhs )
                .SetBinaryOperator ( OperatorAssociativity.Left, "|", 5, ( lhs, rhs ) => ( Int64 ) lhs | ( Int64 ) rhs )
                .SetBinaryOperator ( OperatorAssociativity.Left, "xor", 5, ( lhs, rhs ) => ( Int64 ) lhs ^ ( Int64 ) rhs )
                .SetFunction ( "abs", f => f.AddOverload ( Math.Abs ) )
                .SetFunction ( "acos", f => f.AddOverload ( Math.Acos ) )
                .SetFunction ( "asin", f => f.AddOverload ( Math.Asin ) )
                .SetFunction ( "atan", f => f.AddOverload ( Math.Atan ) )
                .SetFunction ( "atan2", f => f.AddOverload ( Math.Atan2 ) )
                .SetFunction ( "ceil", f => f.AddOverload ( Math.Ceiling ) )
                .SetFunction ( "cos", f => f.AddOverload ( Math.Cos ) )
                .SetFunction ( "cosh", f => f.AddOverload ( Math.Cosh ) )
                .SetFunction ( "exp", f => f.AddOverload ( Math.Exp ) )
                .SetFunction ( "floor", f => f.AddOverload ( Math.Floor ) )
                .SetFunction ( "ln", f => f.AddOverload ( ( Func<Double, Double> ) Math.Log ) )
                .SetFunction ( "log", f => f.AddOverload ( ( Func<Double, Double> ) Math.Log )
                    .AddOverload ( ( Func<Double, Double, Double> ) Math.Log ) )
                .SetFunction ( "log10", f => f.AddOverload ( Math.Log10 ) )
                .SetFunction ( "log2", f => f.AddOverload ( ( n ) => Math.Log ( n, 2 ) ) )
                .SetFunction ( "max", f => f.AddOverload ( Math.Max ) )
                .SetFunction ( "min", f => f.AddOverload ( Math.Min ) )
                .SetFunction ( "pow", f => f.AddOverload ( Math.Pow ) )
                .SetFunction ( "round", f => f.AddOverload ( Math.Round ) )
                .SetFunction ( "sin", f => f.AddOverload ( Math.Sin ) )
                .SetFunction ( "sinh", f => f.AddOverload ( Math.Sinh ) )
                .SetFunction ( "sqrt", f => f.AddOverload ( Math.Sqrt ) )
                .SetFunction ( "tan", f => f.AddOverload ( Math.Tan ) )
                .SetFunction ( "tanh", f => f.AddOverload ( Math.Tanh ) )
                .SetFunction ( "truncate", f => f.AddOverload ( Math.Truncate ) )
                /*
                    * a - b
                    * c - d
                    *
                    * a*d = c*b
                    *
                    * d = (c*b)/a
                    */
                .SetFunction ( new[] { "rot", "ruleOfThree" }, f => f.AddOverload ( ( a, b, c ) => ( b * c ) / a ) )
                .GetCalculatorLanguage ( );
        }

        public static CalculatorLanguage Instance { get; }
    }
}
