using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using Calculator.Definitions;

namespace Calculator.Benchmark
{
    [Config(typeof(Config))]
    public class LanguageBuildBenchmark
    {
        private class Config : ManualConfig
        {
            public Config()
            {
                var @base = Job.Dry.WithEvaluateOverhead(true);
                AddJob(@base.WithRuntime(CoreRuntime.Core31));
                AddJob(@base.WithRuntime(CoreRuntime.Core50));
                AddJob(@base.WithRuntime(CoreRuntime.Core60));
            }
        }

        [Benchmark(Baseline = true)]
        public CalculatorLanguage GetCalculatorLanguage()
        {
            const double PI_OVER_180 = Math.PI / 180;
            const double PI_UNDER_180 = 180 / Math.PI;
            var lang = new CalculatorLanguageBuilder(StringComparer.InvariantCultureIgnoreCase);

            // Constants
            lang.AddConstant("E", Math.E)
                .AddConstants(new[] { "π", "pi" }, Math.PI);

            // Binary operators - Logical Operators
            lang.AddBinaryOperators(Associativity.Left, new[] { "|", "∨" }, 1, (left, right) => ((long) left) | ((long) right))
            .AddBinaryOperators(Associativity.Left, new[] { "xor", "⊻", "⊕" }, 2, (left, right) => ((long) left) ^ ((long) right))
            .AddBinaryOperators(Associativity.Left, new[] { "&", "∧" }, 3, (left, right) => ((long) left) & ((long) right))
            .AddBinaryOperator(Associativity.Left, ">>", 4, (left, right) => ((long) left) >> ((int) right))
            .AddBinaryOperator(Associativity.Left, "<<", 4, (left, right) => ((long) left) << ((int) right));

            // Binary operators - Math Operators
            lang.AddBinaryOperators(Associativity.Left, new[] { "+", "➕" }, 5, (left, right) => left + right)
                .AddBinaryOperators(Associativity.Left, new[] { "-" }, 5, (left, right) => left - right)
                .AddBinaryOperators(Associativity.Left, new[] { "*", "×", "✕", "❌", "✖", "·" }, 6, (left, right) => left * right)
                .AddBinaryOperators(Associativity.Left, new[] { "/", "÷" }, 6, (left, right) => left / right)
                .AddBinaryOperator(Associativity.Left, "%", 6, (left, right) => left % right)
                .AddImplicitMultiplication(7, (left, right) => left * right);

            // Unary operators
            lang.AddUnaryOperator(UnaryOperatorFix.Prefix, "-", 8, n => -n)
                .AddUnaryOperator(UnaryOperatorFix.Prefix, "~", 8, n => ~(long) n)
                .AddUnaryOperator(UnaryOperatorFix.Postfix, "!", 8, factorial);

            // Exponentiation
            lang.AddBinaryOperator(Associativity.Right, "^", 9, Math.Pow)
                .AddSuperscriptExponentiation(9, Math.Pow);

            // Functions - Math.*
            lang.AddFunction("abs", f => f.AddOverload(Math.Abs))
                .AddFunction("acos", f => f.AddOverload(Math.Acos))
                .AddFunction("asin", f => f.AddOverload(Math.Asin))
                .AddFunction("asinh", f => f.AddOverload(Math.Asinh))
                .AddFunction("atan", f => f.AddOverload(Math.Atan))
                .AddFunction("atan2", f => f.AddOverload(Math.Atan2))
                .AddFunction("atanh", f => f.AddOverload(Math.Atanh))
                //.AddFunction ( "bitDecrement", f => f.AddOverload ( Math.BitDecrement ) )
                //.AddFunction ( "bitIncrement", f => f.AddOverload ( Math.BitIncrement ) )
                .AddFunction("cbrt", f => f.AddOverload(Math.Cbrt))
                .AddFunction("ceil", f => f.AddOverload(Math.Ceiling))
                .AddFunction("clamp", f => f.AddOverload(Math.Clamp))
                //.AddFunction ( "copySign", f => f.AddOverload ( Math.CopySign ) )
                .AddFunction("cos", f => f.AddOverload(Math.Cos))
                .AddFunction("cosh", f => f.AddOverload(Math.Cosh))
                .AddFunction("exp", f => f.AddOverload(Math.Exp))
                .AddFunction("floor", f => f.AddOverload(Math.Floor))
                //.AddFunction ( "fusedMultiplyadd", f => f.AddOverload ( Math.FusedMultiplyAdd ) )
                .AddFunction("IEEERemainder", f => f.AddOverload(Math.IEEERemainder))
                //.AddFunction ( "ilogb", f => f.AddOverload ( x => Math.ILogB ( x ) ) )
                .AddFunction("ln", f => f.AddOverload((Func<double, double>) Math.Log))
                .AddFunction("log", f => f.AddOverload((Func<double, double>) Math.Log)
                                          .AddOverload((Func<double, double, double>) Math.Log))
                .AddFunction("log10", f => f.AddOverload(Math.Log10))
                //.AddFunction ( "log2", f => f.AddOverload ( Math.Log2 ) )
                .AddFunction("log2", f => f.AddOverload(n => Math.Log(n, 2)))
                .AddFunction("max", f => f.AddOverload(Math.Max))
                //.AddFunction ( "maxMagnitude", f => f.AddOverload ( Math.MaxMagnitude ) )
                .AddFunction("min", f => f.AddOverload(Math.Min))
                //.AddFunction ( "minMagnitude", f => f.AddOverload ( Math.MinMagnitude ) )
                .AddFunction("pow", f => f.AddOverload(Math.Pow))
                .AddFunction("round", f => f.AddOverload(Math.Round))
                //.AddFunction ( "scaleB", f => f.AddOverload ( ( x, n ) => Math.ScaleB ( x, ( Int32 ) n ) ) )
                .AddFunction("sign", f => f.AddOverload(n => Math.Sign(n)))
                .AddFunction("sin", f => f.AddOverload(Math.Sin))
                .AddFunction("sinh", f => f.AddOverload(Math.Sinh))
                .AddFunction("sqrt", f => f.AddOverload(Math.Sqrt))
                .AddFunction("tan", f => f.AddOverload(Math.Tan))
                .AddFunction("tanh", f => f.AddOverload(Math.Tanh))
                .AddFunction("truncate", f => f.AddOverload(Math.Truncate));

            // Functions - Custom
            lang.AddFunction("rad", f => f.AddOverload(n => n * PI_OVER_180))
                .AddFunction("deg", f => f.AddOverload(n => n * PI_UNDER_180))
                /*
                 * a - b
                 * c - d
                 *
                 * a*d = c*b
                 *
                 * d = (c*b)/a
                 */
                .AddFunctions(new[] { "rot", "ruleOfThree" }, f => f.AddOverload((a, b, c) => (b * c) / a));

            // Get immutable calculator language
            return lang.ToCalculatorLanguage();

            static double factorial(double arg)
            {
                if (double.IsInfinity(arg))
                    return arg;

                var res = 1d;
                for (var i = 2; i <= arg && !double.IsInfinity(res) && !double.IsInfinity(i); i++)
                    res *= i;
                return res;
            }
        }

        [Benchmark()]
        public CalculatorLanguage GetCalculatorLanguage2()
        {
            const double PI_OVER_180 = Math.PI / 180;
            const double PI_UNDER_180 = 180 / Math.PI;
            return new CalculatorLanguage(StringComparer.InvariantCultureIgnoreCase)
                // Constants
                .SetConstant("E", Math.E)
                .SetConstant("π", Math.PI)
                .SetConstant("pi", Math.PI)
                // Binary Operators - Logical
                .AddBinaryOperator(Associativity.Left, "|", 1, (left, right) => ((long) left) | ((long) right))
                .AddBinaryOperator(Associativity.Left, "∨", 1, (left, right) => ((long) left) | ((long) right))
                .AddBinaryOperator(Associativity.Left, "xor", 2, (left, right) => ((long) left) ^ ((long) right))
                .AddBinaryOperator(Associativity.Left, "⊻", 2, (left, right) => ((long) left) ^ ((long) right))
                .AddBinaryOperator(Associativity.Left, "⊕", 2, (left, right) => ((long) left) ^ ((long) right))
                .AddBinaryOperator(Associativity.Left, "&", 3, (left, right) => ((long) left) & ((long) right))
                .AddBinaryOperator(Associativity.Left, "∧", 3, (left, right) => ((long) left) & ((long) right))
                .AddBinaryOperator(Associativity.Left, ">>", 4, (left, right) => ((long) left) >> ((int) right))
                .AddBinaryOperator(Associativity.Left, "<<", 4, (left, right) => ((long) left) << ((int) right))
                // Binary Operators - Arithmetic
                .AddBinaryOperator(Associativity.Left, "+", 5, (left, right) => left + right)
                .AddBinaryOperator(Associativity.Left, "➕", 5, (left, right) => left + right)
                .AddBinaryOperator(Associativity.Left, "-", 5, (left, right) => left - right)
                .AddBinaryOperator(Associativity.Left, "*", 6, (left, right) => left * right)
                .AddBinaryOperator(Associativity.Left, "×", 6, (left, right) => left * right)
                .AddBinaryOperator(Associativity.Left, "✕", 6, (left, right) => left * right)
                .AddBinaryOperator(Associativity.Left, "❌", 6, (left, right) => left * right)
                .AddBinaryOperator(Associativity.Left, "✖", 6, (left, right) => left * right)
                .AddBinaryOperator(Associativity.Left, "·", 6, (left, right) => left * right)
                .AddBinaryOperator(Associativity.Left, "/", 6, (left, right) => left / right)
                .AddBinaryOperator(Associativity.Left, "÷", 6, (left, right) => left / right)
                .AddBinaryOperator(Associativity.Left, "%", 6, (left, right) => left % right)
                .SetImplicitMultiplication(7, (left, right) => left * right)
                // Unary Operators
                .AddUnaryOperator(UnaryOperatorFix.Prefix, "-", 8, n => -n)
                .AddUnaryOperator(UnaryOperatorFix.Prefix, "~", 8, n => ~(long) n)
                .AddUnaryOperator(UnaryOperatorFix.Postfix, "!", 8, factorial)
                // Binary Operators - Exponentiation
                .AddBinaryOperator(Associativity.Right, "^", 9, Math.Pow)
                .SetSuperscriptExponentiation(9, Math.Pow)
                // Functions - Math.*
                .AddFunction("abs", f => f.AddOverload(Math.Abs))
                .AddFunction("acos", f => f.AddOverload(Math.Acos))
                .AddFunction("asin", f => f.AddOverload(Math.Asin))
                .AddFunction("asinh", f => f.AddOverload(Math.Asinh))
                .AddFunction("atan", f => f.AddOverload(Math.Atan))
                .AddFunction("atan2", f => f.AddOverload(Math.Atan2))
                .AddFunction("atanh", f => f.AddOverload(Math.Atanh))
                .AddFunction("cbrt", f => f.AddOverload(Math.Cbrt))
                .AddFunction("ceil", f => f.AddOverload(Math.Ceiling))
                .AddFunction("clamp", f => f.AddOverload(Math.Clamp))
                .AddFunction("cos", f => f.AddOverload(Math.Cos))
                .AddFunction("cosh", f => f.AddOverload(Math.Cosh))
                .AddFunction("exp", f => f.AddOverload(Math.Exp))
                .AddFunction("floor", f => f.AddOverload(Math.Floor))
                .AddFunction("IEEERemainder", f => f.AddOverload(Math.IEEERemainder))
                .AddFunction("ln", f => f.AddOverload((Func<double, double>) Math.Log))
                .AddFunction("log", f => f.AddOverload((Func<double, double>) Math.Log)
                                          .AddOverload((Func<double, double, double>) Math.Log))
                .AddFunction("log10", f => f.AddOverload(Math.Log10))
                .AddFunction("log2", f => f.AddOverload(n => Math.Log(n, 2)))
                .AddFunction("max", f => f.AddOverload(Math.Max))
                .AddFunction("min", f => f.AddOverload(Math.Min))
                .AddFunction("pow", f => f.AddOverload(Math.Pow))
                .AddFunction("round", f => f.AddOverload(Math.Round))
                .AddFunction("sign", f => f.AddOverload(n => Math.Sign(n)))
                .AddFunction("sin", f => f.AddOverload(Math.Sin))
                .AddFunction("sinh", f => f.AddOverload(Math.Sinh))
                .AddFunction("sqrt", f => f.AddOverload(Math.Sqrt))
                .AddFunction("tan", f => f.AddOverload(Math.Tan))
                .AddFunction("tanh", f => f.AddOverload(Math.Tanh))
                .AddFunction("truncate", f => f.AddOverload(Math.Truncate))
                // Functions - Custom
                .AddFunction("rad", f => f.AddOverload(n => n * PI_OVER_180))
                .AddFunction("deg", f => f.AddOverload(n => n * PI_UNDER_180))
                .AddFunction("rot", f => f.AddOverload((a, b, c) => (b * c) / a))
                .AddFunction("ruleOfThree", f => f.AddOverload((a, b, c) => (b * c) / a));

            static double factorial(double arg)
            {
                if (double.IsInfinity(arg))
                    return arg;

                var res = 1d;
                for (var i = 2; i <= arg && !double.IsInfinity(res) && !double.IsInfinity(i); i++)
                    res *= i;
                return res;
            }
        }
    }
}