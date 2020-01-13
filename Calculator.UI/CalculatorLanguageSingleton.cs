using System;
using Calculator.Definitions;
using GUtils.Numerics;

namespace Calculator.UI
{
    public static class CalculatorLanguageSingleton
    {
        private static CalculatorLanguage GetCalculatorLanguage ( )
        {
            const Double PI_OVER_180 = Math.PI / 180;
            const Double PI_UNDER_180 = 180 / Math.PI;
            var lang = new CalculatorLanguageBuilder(StringComparer.InvariantCultureIgnoreCase);

            // Constants
            lang.AddConstant ( "E", Math.E )
                .AddConstants ( new[] { "π", "pi" }, Math.PI )
                .AddConstants ( new[] { "τ", "tau" }, Math.PI * 2 )
                .AddConstants ( new[] { "φ", "phi" }, 1.6180339887498948 )
                .AddConstant ( "NaN", Double.NaN )
                .AddConstants ( new[] { "∞", "Inf", "Infinity" }, Double.PositiveInfinity )
                .AddConstant ( "KiB", FileSize.KiB )
                .AddConstant ( "Kilo", Math.Pow ( 1000, 1 ) )
                .AddConstant ( "MiB", FileSize.MiB )
                .AddConstant ( "Mega", Math.Pow ( 1000, 2 ) )
                .AddConstant ( "GiB", FileSize.GiB )
                .AddConstant ( "Giga", Math.Pow ( 1000, 3 ) )
                .AddConstant ( "TiB", FileSize.TiB )
                .AddConstant ( "Tera", Math.Pow ( 1000, 4 ) )
                .AddConstant ( "PiB", FileSize.PiB )
                .AddConstant ( "Peta", Math.Pow ( 1000, 5 ) )
                .AddConstant ( "EiB", FileSize.EiB )
                .AddConstant ( "Exa", Math.Pow ( 1000, 6 ) );

            // Binary operators - Logical Operators
            lang.AddBinaryOperators ( Associativity.Left, new[] { "|", "∨" }, 1, ( left, right ) => ( ( Int64 ) left ) | ( ( Int64 ) right ) )
                .AddBinaryOperators ( Associativity.Left, new[] { "xor", "⊻", "⊕" }, 2, ( left, right ) => ( ( Int64 ) left ) ^ ( ( Int64 ) right ) )
                .AddBinaryOperators ( Associativity.Left, new[] { "&", "∧" }, 3, ( left, right ) => ( ( Int64 ) left ) & ( ( Int64 ) right ) )
                .AddBinaryOperator ( Associativity.Left, ">>", 4, ( left, right ) => ( ( Int64 ) left ) >> ( ( Int32 ) right ) )
                .AddBinaryOperator ( Associativity.Left, "<<", 4, ( left, right ) => ( ( Int64 ) left ) << ( ( Int32 ) right ) );

            // Binary operators - Math Operators
            lang.AddBinaryOperators ( Associativity.Left, new[] { "+", "➕" }, 5, ( left, right ) => left + right )
                .AddBinaryOperators ( Associativity.Left, new[] { "-" }, 5, ( left, right ) => left - right )
                .AddBinaryOperators ( Associativity.Left, new[] { "*", "×", "✕", "❌", "✖", "·" }, 6, ( left, right ) => left * right )
                .AddBinaryOperators ( Associativity.Left, new[] { "/", "÷" }, 6, ( left, right ) => left / right )
                .AddBinaryOperator ( Associativity.Left, "%", 6, ( left, right ) => left % right )
                .AddImplicitMultiplication ( 7, ( left, right ) => left * right );

            // Unary operators
            lang.AddUnaryOperator ( UnaryOperatorFix.Prefix, "-", 8, n => -n )
                .AddUnaryOperator ( UnaryOperatorFix.Prefix, "~", 8, n => ~( Int64 ) n )
                .AddUnaryOperator ( UnaryOperatorFix.Postfix, "!", 8, factorial );

            // Exponentiation
            lang.AddBinaryOperator ( Associativity.Right, "^", 9, Math.Pow )
                .AddSuperscriptExponentiation ( 9, Math.Pow );

            // Functions - Math.*
            lang.AddFunction ( "abs", f => f.AddOverload ( Math.Abs ) )
                .AddFunction ( "acos", f => f.AddOverload ( Math.Acos ) )
                .AddFunction ( "asin", f => f.AddOverload ( Math.Asin ) )
                .AddFunction ( "asinh", f => f.AddOverload ( Math.Asinh ) )
                .AddFunction ( "atan", f => f.AddOverload ( Math.Atan ) )
                .AddFunction ( "atan2", f => f.AddOverload ( Math.Atan2 ) )
                .AddFunction ( "atanh", f => f.AddOverload ( Math.Atanh ) )
                .AddFunction ( "bitDecrement", f => f.AddOverload ( Math.BitDecrement ) )
                .AddFunction ( "bitIncrement", f => f.AddOverload ( Math.BitIncrement ) )
                .AddFunction ( "cbrt", f => f.AddOverload ( Math.Cbrt ) )
                .AddFunction ( "ceil", f => f.AddOverload ( Math.Ceiling ) )
                .AddFunction ( "clamp", f => f.AddOverload ( Math.Clamp ) )
                .AddFunction ( "copySign", f => f.AddOverload ( Math.CopySign ) )
                .AddFunction ( "cos", f => f.AddOverload ( Math.Cos ) )
                .AddFunction ( "cosh", f => f.AddOverload ( Math.Cosh ) )
                .AddFunction ( "exp", f => f.AddOverload ( Math.Exp ) )
                .AddFunction ( "floor", f => f.AddOverload ( Math.Floor ) )
                .AddFunction ( "fusedMultiplyadd", f => f.AddOverload ( Math.FusedMultiplyAdd ) )
                .AddFunction ( "IEEERemainder", f => f.AddOverload ( Math.IEEERemainder ) )
                .AddFunction ( "ilogb", f => f.AddOverload ( x => Math.ILogB ( x ) ) )
                .AddFunction ( "ln", f => f.AddOverload ( ( Func<Double, Double> ) Math.Log ) )
                .AddFunction ( "log", f => f.AddOverload ( ( Func<Double, Double> ) Math.Log )
                                            .AddOverload ( ( Func<Double, Double, Double> ) Math.Log ) )
                .AddFunction ( "log10", f => f.AddOverload ( Math.Log10 ) )
                .AddFunction ( "log2", f => f.AddOverload ( Math.Log2 ) )
                .AddFunction ( "max", f => f.AddOverload ( Math.Max ) )
                .AddFunction ( "maxMagnitude", f => f.AddOverload ( Math.MaxMagnitude ) )
                .AddFunction ( "min", f => f.AddOverload ( Math.Min ) )
                .AddFunction ( "minMagnitude", f => f.AddOverload ( Math.MinMagnitude ) )
                .AddFunction ( "pow", f => f.AddOverload ( Math.Pow ) )
                .AddFunction ( "round", f => f.AddOverload ( Math.Round ) )
                .AddFunction ( "scaleB", f => f.AddOverload ( ( x, n ) => Math.ScaleB ( x, ( Int32 ) n ) ) )
                .AddFunction ( "sign", f => f.AddOverload ( n => Math.Sign ( n ) ) )
                .AddFunction ( "sin", f => f.AddOverload ( Math.Sin ) )
                .AddFunction ( "sinh", f => f.AddOverload ( Math.Sinh ) )
                .AddFunction ( "sqrt", f => f.AddOverload ( Math.Sqrt ) )
                .AddFunction ( "tan", f => f.AddOverload ( Math.Tan ) )
                .AddFunction ( "tanh", f => f.AddOverload ( Math.Tanh ) )
                .AddFunction ( "truncate", f => f.AddOverload ( Math.Truncate ) );

            // Functions - Custom
            lang.AddFunction ( "rad", f => f.AddOverload ( n => n * PI_OVER_180 ) )
                .AddFunction ( "deg", f => f.AddOverload ( n => n * PI_UNDER_180 ) )
                /*
                 * a - b
                 * c - d
                 *
                 * a*d = c*b
                 *
                 * d = (c*b)/a
                 */
                .AddFunctions ( new[] { "rot", "ruleOfThree" }, f => f.AddOverload ( ( a, b, c ) => ( b * c ) / a ) );

            // Get immutable calculator language
            return lang.ToCalculatorLanguage ( );

            static Double factorial ( Double arg )
            {
                if ( arg == 1 )
                    return 1d;
                else if ( arg < 0 )
                    return Double.NaN;
                else if ( arg > 170 )
                    return Double.PositiveInfinity;
                else
                    return arg * factorial ( arg - 1 );
            }
        }

        public static CalculatorLanguage Instance { get; } = GetCalculatorLanguage ( );
    }
}