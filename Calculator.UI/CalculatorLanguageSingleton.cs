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
                .AddConstant ( "E", Math.E )
                .AddConstant ( "pi", Math.PI )
                .AddConstant ( "π", Math.PI )
                .AddUnaryOperator ( UnaryOperatorFix.Prefix, "-", 1, n => -n )
                .AddUnaryOperator ( UnaryOperatorFix.Prefix, "~", 1, n => ~( Int64 ) n )
                .AddUnaryOperator ( UnaryOperatorFix.Postfix, "!", 1, n =>
                {
                    if ( Double.IsInfinity ( n ) )
                        return n;
                    var r = 1D;
                    for ( var i = 2; i < n && !Double.IsInfinity ( r ) && !Double.IsInfinity ( i ); i++ )
                        r *= i;
                    return r;
                } )
                .AddBinaryOperator ( Associativity.Left, "+", 1, ( lhs, rhs ) => lhs + rhs )
                .AddBinaryOperator ( Associativity.Left, "-", 1, ( lhs, rhs ) => lhs - rhs )
                .AddBinaryOperator ( Associativity.Left, "*", 2, ( lhs, rhs ) => lhs * rhs )
                .AddBinaryOperator ( Associativity.Left, "/", 2, ( lhs, rhs ) => lhs / rhs )
                .AddBinaryOperator ( Associativity.Left, "%", 2, ( lhs, rhs ) => lhs % rhs )
                .AddBinaryOperator ( Associativity.Right, "^", 3, ( lhs, rhs ) => Math.Pow ( lhs, rhs ) )
                .AddBinaryOperator ( Associativity.Left, "<<", 4, ( lhs, rhs ) => ( Int64 ) lhs << ( Int32 ) rhs )
                .AddBinaryOperator ( Associativity.Left, ">>", 4, ( lhs, rhs ) => ( Int64 ) lhs >> ( Int32 ) rhs )
                .AddBinaryOperator ( Associativity.Left, "&", 5, ( lhs, rhs ) => ( Int64 ) lhs & ( Int64 ) rhs )
                .AddBinaryOperator ( Associativity.Left, "|", 5, ( lhs, rhs ) => ( Int64 ) lhs | ( Int64 ) rhs )
                .AddBinaryOperator ( Associativity.Left, "xor", 5, ( lhs, rhs ) => ( Int64 ) lhs ^ ( Int64 ) rhs )
                .AddFunction ( "abs", f => f.AddOverload ( Math.Abs ) )
                .AddFunction ( "acos", f => f.AddOverload ( Math.Acos ) )
                .AddFunction ( "asin", f => f.AddOverload ( Math.Asin ) )
                .AddFunction ( "atan", f => f.AddOverload ( Math.Atan ) )
                .AddFunction ( "atan2", f => f.AddOverload ( Math.Atan2 ) )
                .AddFunction ( "ceil", f => f.AddOverload ( Math.Ceiling ) )
                .AddFunction ( "cos", f => f.AddOverload ( Math.Cos ) )
                .AddFunction ( "cosh", f => f.AddOverload ( Math.Cosh ) )
                .AddFunction ( "exp", f => f.AddOverload ( Math.Exp ) )
                .AddFunction ( "floor", f => f.AddOverload ( Math.Floor ) )
                .AddFunction ( "ln", f => f.AddOverload ( ( Func<Double, Double> ) Math.Log ) )
                .AddFunction ( "log", f => f.AddOverload ( ( Func<Double, Double> ) Math.Log )
                    .AddOverload ( ( Func<Double, Double, Double> ) Math.Log ) )
                .AddFunction ( "log10", f => f.AddOverload ( Math.Log10 ) )
                .AddFunction ( "log2", f => f.AddOverload ( ( n ) => Math.Log ( n, 2 ) ) )
                .AddFunction ( "max", f => f.AddOverload ( Math.Max ) )
                .AddFunction ( "min", f => f.AddOverload ( Math.Min ) )
                .AddFunction ( "pow", f => f.AddOverload ( Math.Pow ) )
                .AddFunction ( "round", f => f.AddOverload ( Math.Round ) )
                .AddFunction ( "sin", f => f.AddOverload ( Math.Sin ) )
                .AddFunction ( "sinh", f => f.AddOverload ( Math.Sinh ) )
                .AddFunction ( "sqrt", f => f.AddOverload ( Math.Sqrt ) )
                .AddFunction ( "tan", f => f.AddOverload ( Math.Tan ) )
                .AddFunction ( "tanh", f => f.AddOverload ( Math.Tanh ) )
                .AddFunction ( "truncate", f => f.AddOverload ( Math.Truncate ) )
                /*
                    * a - b
                    * c - d
                    *
                    * a*d = c*b
                    *
                    * d = (c*b)/a
                    */
                .AddFunctions ( new[] { "rot", "ruleOfThree" }, f => f.AddOverload ( ( a, b, c ) => ( b * c ) / a ) )
                .ToCalculatorLanguage ( );
        }

        public static CalculatorLanguage Instance { get; }
    }
}
