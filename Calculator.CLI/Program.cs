﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using Calculator.Definitions;
using Calculator.Parsing;
using GParse;
using GParse.Errors;
using Tsu.CLI.Commands;
using Tsu.Numerics;
using Tsu.Timing;

namespace Calculator.CLI
{
    internal class Program
    {
        private static CalculatorLanguage _language;
        private static TreeEvaluator _evaluator;
        private static SimpleTreeReconstructor _reconstructor;
        private static ConsoleTimingLogger _timingLogger;

        private static void Main()
        {
            _timingLogger = new ConsoleTimingLogger();
            using (_timingLogger.BeginScope("Initialization", true))
            {
                using (_timingLogger.BeginScope("Building the language", true))
                    _language = GetCalculatorLanguage();
                using (_timingLogger.BeginScope("Initializing the evaluator", true))
                    _evaluator = new TreeEvaluator(_language);
                using (_timingLogger.BeginScope("Initializing the reconstructor", true))
                    _reconstructor = new SimpleTreeReconstructor();

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    Console.InputEncoding = Console.OutputEncoding = System.Text.Encoding.Unicode;
            }

            using (_timingLogger.BeginScope("Runtime", false))
            {
                while (true)
                {
                    _timingLogger.Write('>');
                    var line = _timingLogger.ReadLine().Trim();
                    if (line is "q" or "e" or "quit" or "exit")
                        break;

                    var eqs = line.IndexOf('=');
                    if ((line.StartsWith("b ") || line.StartsWith("bench ")) && eqs < 0)
                        BenchmarkWithExpression(line[line.IndexOf(' ')..]);
                    else if ((line.StartsWith("l ") || line.StartsWith("lex ")) && eqs < 0)
                        LexExpression(line[(line.IndexOf(' ') + 1)..]);
                    else if ((line.StartsWith("r ") || line.StartsWith("random ")) && eqs < 0)
                        GenerateRandomExpression(line[(line.IndexOf(' ') + 1)..]);
                    else if ((line.StartsWith("v ") || line.StartsWith("verbose ")) && eqs < 0)
                        ExecuteExpressionVerbose(line[(line.IndexOf(' ') + 1)..]);
                    else
                        ExecuteExpression(line, eqs);
                }
            }
        }

        private static CalculatorLanguage GetCalculatorLanguage()
        {
            const double PI_OVER_180 = Math.PI / 180;
            const double PI_UNDER_180 = 180 / Math.PI;
            var lang = new CalculatorLanguageBuilder(StringComparer.InvariantCultureIgnoreCase);

            // Constants
            lang.AddConstant("E", Math.E)
                .AddConstants(new[] { "π", "pi" }, Math.PI)
                .AddConstants(new[] { "τ", "tau" }, Math.PI * 2)
                .AddConstants(new[] { "φ", "phi" }, 1.6180339887498948)
                .AddConstant("NaN", double.NaN)
                .AddConstants(new[] { "∞", "Inf", "Infinity" }, double.PositiveInfinity)
                .AddConstant("KiB", FileSize.KiB)
                .AddConstant("Kilo", SI.Kilo)
                .AddConstant("MiB", FileSize.MiB)
                .AddConstant("Mega", SI.Mega)
                .AddConstant("GiB", FileSize.GiB)
                .AddConstant("Giga", SI.Giga)
                .AddConstant("TiB", FileSize.TiB)
                .AddConstant("Tera", SI.Tera)
                .AddConstant("PiB", FileSize.PiB)
                .AddConstant("Peta", SI.Peta)
                .AddConstant("EiB", FileSize.EiB)
                .AddConstant("Exa", SI.Exa);

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
                .AddUnaryOperator(UnaryOperatorFix.Prefix, "√", 9, Math.Sqrt)
                .AddSuperscriptExponentiation(9, Math.Pow);

            // Functions - Math.*
            lang.AddFunction("abs", f => f.AddOverload(Math.Abs))
                .AddFunction("acos", f => f.AddOverload(Math.Acos))
                .AddFunction("asin", f => f.AddOverload(Math.Asin))
                .AddFunction("asinh", f => f.AddOverload(Math.Asinh))
                .AddFunction("atan", f => f.AddOverload(Math.Atan))
                .AddFunction("atan2", f => f.AddOverload(Math.Atan2))
                .AddFunction("atanh", f => f.AddOverload(Math.Atanh))
                .AddFunction("bitDecrement", f => f.AddOverload(Math.BitDecrement))
                .AddFunction("bitIncrement", f => f.AddOverload(Math.BitIncrement))
                .AddFunction("cbrt", f => f.AddOverload(Math.Cbrt))
                .AddFunction("ceil", f => f.AddOverload(Math.Ceiling))
                .AddFunction("clamp", f => f.AddOverload(Math.Clamp))
                .AddFunction("copySign", f => f.AddOverload(Math.CopySign))
                .AddFunction("cos", f => f.AddOverload(Math.Cos))
                .AddFunction("cosh", f => f.AddOverload(Math.Cosh))
                .AddFunction("exp", f => f.AddOverload(Math.Exp))
                .AddFunction("floor", f => f.AddOverload(Math.Floor))
                .AddFunction("fusedMultiplyadd", f => f.AddOverload(Math.FusedMultiplyAdd))
                .AddFunction("IEEERemainder", f => f.AddOverload(Math.IEEERemainder))
                .AddFunction("ilogb", f => f.AddOverload(x => Math.ILogB(x)))
                .AddFunction("ln", f => f.AddOverload((Func<double, double>) Math.Log))
                .AddFunction("log", f => f.AddOverload((Func<double, double>) Math.Log)
                                               .AddOverload((Func<double, double, double>) Math.Log))
                .AddFunction("log10", f => f.AddOverload(Math.Log10))
                .AddFunction("log2", f => f.AddOverload(Math.Log2))
                .AddFunction("max", f => f.AddOverload(Math.Max))
                .AddFunction("maxMagnitude", f => f.AddOverload(Math.MaxMagnitude))
                .AddFunction("min", f => f.AddOverload(Math.Min))
                .AddFunction("minMagnitude", f => f.AddOverload(Math.MinMagnitude))
                .AddFunction("pow", f => f.AddOverload(Math.Pow))
                .AddFunction("round", f => f.AddOverload(Math.Round)
                                                 .AddOverload((num, digits) => Math.Round(num, (int) digits)))
                .AddFunction("scaleB", f => f.AddOverload((x, n) => Math.ScaleB(x, (int) n)))
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
                if (arg == 1)
                    return 1d;
                else if (arg < 0)
                    return double.NaN;
                else if (arg > 170)
                    return double.PositiveInfinity;
                else
                    return arg * factorial(arg - 1);
            }
        }

        [Command("lex")]
        private static void LexExpression(string expression)
        {
            using (_timingLogger.BeginScope($"Lexing: {expression}", true))
            {
                var toks = _language.Lex(expression, out var diagnostics);
                foreach (var tok in toks)
                    _timingLogger.WriteLine($"{tok.Type}<{tok.Id}>({tok.Text}, {tok.Value})");

                foreach (var diag in diagnostics.OrderBy(d => d.Severity))
                {
                    var range = SourceRange.Calculate(expression, diag.Range);
                    _timingLogger.WriteLine($@"{CalculatorDiagnostics.HighlightRange(expression, range)}
{diag.Id} {diag.Severity}: {diag.Description}");
                }
            }
        }

        private static string GetExpressionContext(int offset, string expr)
        {
            const int context = 50;
            var start = Math.Max(offset - context / 2, 0);
            var len = Math.Min(context / 2, expr.Length - offset);
            return $@"{expr.Substring(start, len)}
{new string(' ', start)}^";
        }

        private static void ExecuteExpression(string expression, int eqs)
        {
            try
            {
                var name = "ans";
                if (eqs > 0)
                {
                    name = expression.Substring(0, eqs).Trim();
                    expression = expression[(eqs + 1)..].Trim();

                    if (name.Equals("E", StringComparison.OrdinalIgnoreCase) || name.Equals("pi", StringComparison.OrdinalIgnoreCase)
                        || name.Equals("π", StringComparison.OrdinalIgnoreCase))
                    {
                        _timingLogger.LogError("Cannot redefine this constant.");
                        return;
                    }
                }

                var res = _language.Evaluate(expression, out var diagnostics);
                _language = _language.SetConstant(name, res);
                _timingLogger.WriteLine($"{name} = {res}");
                foreach (var diagnostic in diagnostics)
                {
                    var range = SourceRange.Calculate(expression, diagnostic.Range);
                    var str = $@"{CalculatorDiagnostics.HighlightRange(expression, range)}
{diagnostic.Id} {diagnostic.Severity}: {diagnostic.Description}";

                    switch (diagnostic.Severity)
                    {
                        case DiagnosticSeverity.Error:
                            _timingLogger.LogError(str);
                            break;

                        case DiagnosticSeverity.Warning:
                            _timingLogger.LogWarning(str);
                            break;

                        case DiagnosticSeverity.Info:
                            _timingLogger.LogInformation(str);
                            break;

                        case DiagnosticSeverity.Hidden:
                            _timingLogger.LogDebug(str);
                            break;
                    }
                }
            }
            catch (FatalParsingException fpex)
            {
                var range = SourceRange.Calculate(expression, fpex.Range);
                _timingLogger.LogError(CalculatorDiagnostics.HighlightRange(expression, range));
                _timingLogger.LogError($"Runtime Error: {fpex.Message}");
            }
            catch (Exception ex)
            {
                _timingLogger.LogError($"Unexpected exception: {ex}");
            }
        }

        private static void ExecuteExpressionVerbose(string expression)
        {
            try
            {
                using (_timingLogger.BeginScope($"Executing: {expression}", true))
                {
                    var diagnostics = new DiagnosticList();
                    CalculatorParser parser;
                    CalculatorTreeNode node;
                    double res;
                    string rec;

                    using (_timingLogger.BeginScope("Lexer + Parser initialization", true))
                        parser = _language.GetParser(expression, diagnostics);

                    using (_timingLogger.BeginScope("Parsing", true))
                        node = parser.Parse();

                    using (_timingLogger.BeginScope("Evaluation", true))
                        res = node.Accept(_evaluator);

                    using (_timingLogger.BeginScope("Reconstruction", true))
                        rec = node.Accept(_reconstructor);

                    _timingLogger.WriteLine($"{rec} = {res}");
                    foreach (var diagnostic in diagnostics)
                    {
                        var range = SourceRange.Calculate(expression, diagnostic.Range);
                        var str = $@"{CalculatorDiagnostics.HighlightRange(expression, range)}
{diagnostic.Id} {diagnostic.Severity}: {diagnostic.Description}";
                        switch (diagnostic.Severity)
                        {
                            case DiagnosticSeverity.Error:
                                _timingLogger.LogError(str);
                                break;

                            case DiagnosticSeverity.Warning:
                                _timingLogger.LogWarning(str);
                                break;

                            case DiagnosticSeverity.Info:
                                _timingLogger.LogInformation(str);
                                break;

                            case DiagnosticSeverity.Hidden:
                                _timingLogger.LogDebug(str);
                                break;
                        }
                    }
                }
            }
            catch (FatalParsingException fpex)
            {
                var range = SourceRange.Calculate(expression, fpex.Range);
                _timingLogger.LogError(CalculatorDiagnostics.HighlightRange(expression, range));
                _timingLogger.LogError($"{fpex.Range}: {fpex.Message}");
            }
            catch (Exception ex)
            {
                _timingLogger.LogError($"Unexpected exception: {ex}");
            }
        }

        private static readonly Random _random = new Random();

        private static CalculatorTreeNode GenerateRandomExpression(int maxDepth)
        {
            var unaryOperators = _language.UnaryOperators.Values.ToArray();
            var binaryOperators = _language.BinaryOperators.Values.ToArray();
            var constants = _language.Constants.Values.ToArray();
            var functions = _language.Functions.Values.ToArray();
            if (maxDepth > 0)
            {
                switch (_random.Next(0, 3))
                {
                    case 0:
                        var unop = unaryOperators[_random.Next(0, unaryOperators.Length)];
                        return unop.Fix == UnaryOperatorFix.Postfix
                            ? ASTHelper.Postfix(GenerateRandomExpression(maxDepth - 1), unop.Operator)
                            : ASTHelper.Prefix(unop.Operator, GenerateRandomExpression(maxDepth - 1));

                    case 1:
                        var binop = binaryOperators[_random.Next(0, binaryOperators.Length)];
                        return ASTHelper.Binary(GenerateRandomExpression(maxDepth - 1), binop.Operator, GenerateRandomExpression(maxDepth - 1));

                    case 2:
                        var func = functions[_random.Next(0, functions.Length)];
                        var argCounts = func.Overloads.Keys.ToArray();
                        var overload = func.Overloads[argCounts[_random.Next(0, argCounts.Length)]];
                        var args = new object[overload.Method.GetParameters().Length];
                        for (var i = 0; i < args.Length; i++)
                            args[i] = GenerateRandomExpression(maxDepth - 1);
                        return ASTHelper.Function(func.Name, args);

                    // Will never arrive here anyways.
                    default:
                        throw null;
                }
            }
            else
            {
                return _random.Next(0, 2) == 1
                    ? ASTHelper.Number(_random.NextDouble())
                    : ASTHelper.Identifier(constants[_random.Next(0, constants.Length)].Identifier);
            }
        }

        private static void GenerateRandomExpression(string restOfLine)
        {
            var depth = int.Parse(restOfLine);
            _timingLogger.WriteLine("Randomly generated expression:");
            _timingLogger.WriteLine($"{GenerateRandomExpression(depth).Accept(_reconstructor)}");
        }

        private static void BenchmarkWithExpression(string expression)
        {
            var results = new List<long>();
            var eval = 0D;
            for (var i = 0; i < 200; i++)
            {
                var sw = Stopwatch.StartNew();
                eval = _language.Evaluate(expression, out _);
                sw.Stop();
                results.Add(sw.ElapsedTicks);
            }

            var medianTmp = new Dictionary<long, int>();
            foreach (var tick in results)
            {
                if (!medianTmp.ContainsKey(tick))
                    medianTmp[tick] = 0;
                medianTmp[tick]++;
            }
            var median = medianTmp.First(kv => kv.Value == medianTmp.Max(kv2 => kv2.Value)).Key;

            /*
             * ( a + b + c + d ) / e ≡ a / e + b / e + c / e + d / e
             */
            _timingLogger.WriteLine($"Firstly, the evaluted result: {eval}");
            _timingLogger.WriteLine($"Average time: {Duration.Format(results.Aggregate(0L, (avg, ticks) => avg + ticks / results.Count))}");
            _timingLogger.WriteLine($"Maximum time: {Duration.Format(results.Max())}");
            _timingLogger.WriteLine($"Minimum time: {Duration.Format(results.Min())}");
            _timingLogger.WriteLine($"Median time:  {Duration.Format(median)}");
            _timingLogger.WriteLine("Execution times sorted by frequency:");
            foreach (var kv in medianTmp.OrderBy(kv => kv.Value))
                _timingLogger.WriteLine($"  {kv.Value:000} times: {Duration.Format(kv.Key, "{0:000.00}{1}")}");
        }
    }
}