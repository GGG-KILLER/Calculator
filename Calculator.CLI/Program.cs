using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using Calculator.Definitions;
using Calculator.Lexing;
using Calculator.Parsing;
using Calculator.Parsing.AST;
using Calculator.Parsing.Visitors;
using GParse;
using GParse.Errors;
using GParse.Lexing;
using GUtils.CLI.Commands;
using GUtils.Timing;

namespace Calculator.CLI
{
    internal class Program
    {
        private static CalculatorLanguage language;
        private static TreeEvaluator evaluator;
        private static SimpleTreeReconstructor reconstructor;
        private static ConsoleTimingLogger timingLogger;

        private static void Main ( )
        {
            timingLogger = new ConsoleTimingLogger ( );
            using ( timingLogger.BeginScope ( "Initialization", true ) )
            {
                using ( timingLogger.BeginScope ( "Building the language", true ) )
                    language = GetCalculatorLanguage ( );
                using ( timingLogger.BeginScope ( "Initializing the evaluator", true ) )
                    evaluator = new TreeEvaluator ( language );
                using ( timingLogger.BeginScope ( "Initializing the reconstructor", true ) )
                    reconstructor = new SimpleTreeReconstructor ( );

                if ( RuntimeInformation.IsOSPlatform ( OSPlatform.Windows ) )
                    Console.InputEncoding = Console.OutputEncoding = System.Text.Encoding.Unicode;
            }

            using ( timingLogger.BeginScope ( "Runtime", false ) )
            {
                while ( true )
                {
                    timingLogger.Write ( '>' );
                    var line = timingLogger.ReadLine ( ).Trim ( );
                    if ( line == "q" || line == "e" || line == "quit" || line == "exit" )
                        break;

                    var eqs = line.IndexOf ( '=' );
                    if ( ( line.StartsWith ( "b " ) || line.StartsWith ( "bench " ) ) && eqs < 0 )
                        BenchmarkWithExpression ( line.Substring ( line.IndexOf ( ' ' ) ) );
                    else if ( ( line.StartsWith ( "l " ) || line.StartsWith ( "lex " ) ) && eqs < 0 )
                        LexExpression ( line.Substring ( line.IndexOf ( ' ' ) + 1 ) );
                    else if ( ( line.StartsWith ( "r " ) || line.StartsWith ( "random " ) ) && eqs < 0 )
                        GenerateRandomExpression ( line.Substring ( line.IndexOf ( ' ' ) + 1 ) );
                    else if ( ( line.StartsWith ( "v " ) || line.StartsWith ( "verbose " ) ) && eqs < 0 )
                        ExecuteExpressionVerbose ( line.Substring ( line.IndexOf ( ' ' ) + 1 ) );
                    else
                        ExecuteExpression ( line, eqs );
                }
            }
        }
        private static CalculatorLanguage GetCalculatorLanguage ( )
        {
            const Double PI_OVER_180 = Math.PI / 180;
            const Double PI_UNDER_180 = 180 / Math.PI;
            var lang = new CalculatorLanguageBuilder ( StringComparer.InvariantCultureIgnoreCase );

            // Constants
            lang.AddConstant ( "E", Math.E )
                .AddConstants ( new[] { "π", "pi" }, Math.PI );

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
                //.AddFunction ( "bitDecrement", f => f.AddOverload ( Math.BitDecrement ) )
                //.AddFunction ( "bitIncrement", f => f.AddOverload ( Math.BitIncrement ) )
                .AddFunction ( "cbrt", f => f.AddOverload ( Math.Cbrt ) )
                .AddFunction ( "ceil", f => f.AddOverload ( Math.Ceiling ) )
                .AddFunction ( "clamp", f => f.AddOverload ( Math.Clamp ) )
                //.AddFunction ( "copySign", f => f.AddOverload ( Math.CopySign ) )
                .AddFunction ( "cos", f => f.AddOverload ( Math.Cos ) )
                .AddFunction ( "cosh", f => f.AddOverload ( Math.Cosh ) )
                .AddFunction ( "exp", f => f.AddOverload ( Math.Exp ) )
                .AddFunction ( "floor", f => f.AddOverload ( Math.Floor ) )
                //.AddFunction ( "fusedMultiplyadd", f => f.AddOverload ( Math.FusedMultiplyAdd ) )
                .AddFunction ( "IEEERemainder", f => f.AddOverload ( Math.IEEERemainder ) )
                //.AddFunction ( "ilogb", f => f.AddOverload ( x => Math.ILogB ( x ) ) )
                .AddFunction ( "ln", f => f.AddOverload ( ( Func<Double, Double> ) Math.Log ) )
                .AddFunction ( "log", f => f.AddOverload ( ( Func<Double, Double> ) Math.Log )
                                            .AddOverload ( ( Func<Double, Double, Double> ) Math.Log ) )
                .AddFunction ( "log10", f => f.AddOverload ( Math.Log10 ) )
                //.AddFunction ( "log2", f => f.AddOverload ( Math.Log2 ) )
                .AddFunction ( "log2", f => f.AddOverload ( n => Math.Log ( n, 2 ) ) )
                .AddFunction ( "max", f => f.AddOverload ( Math.Max ) )
                //.AddFunction ( "maxMagnitude", f => f.AddOverload ( Math.MaxMagnitude ) )
                .AddFunction ( "min", f => f.AddOverload ( Math.Min ) )
                //.AddFunction ( "minMagnitude", f => f.AddOverload ( Math.MinMagnitude ) )
                .AddFunction ( "pow", f => f.AddOverload ( Math.Pow ) )
                .AddFunction ( "round", f => f.AddOverload ( Math.Round ) )
                //.AddFunction ( "scaleB", f => f.AddOverload ( ( x, n ) => Math.ScaleB ( x, ( Int32 ) n ) ) )
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
                if ( Double.IsInfinity ( arg ) )
                    return arg;

                var res = 1d;
                for ( var i = 2; i <= arg && !Double.IsInfinity ( res ) && !Double.IsInfinity ( i ); i++ )
                    res *= i;
                return res;
            }
        }

        [Command ( "lex" )]
        private static void LexExpression ( String expression )
        {
            using ( timingLogger.BeginScope ( $"Lexing: {expression}", true ) )
            {
                IEnumerable<Token<CalculatorTokenType>> toks = language.Lex ( expression, out IEnumerable<Diagnostic> diagnostics );
                foreach ( Token<CalculatorTokenType> tok in toks )
                    timingLogger.WriteLine ( tok );

                foreach ( Diagnostic diag in diagnostics.OrderBy ( d => d.Severity ) )
                {
                }
            }
        }

        private static String GetExpressionContext ( Int32 offset, String expr )
        {
            const Int32 context = 50;
            var start =  Math.Max ( offset - context / 2, 0 );
            var len = Math.Min ( context / 2, expr.Length - offset );
            return $@"{expr.Substring ( start, len )}
{new String ( ' ', start )}^";
        }

        private static void ExecuteExpression ( String expression, Int32 eqs )
        {
            try
            {
                String name = "ans";
                if ( eqs > 0 )
                {
                    name = expression.Substring ( 0, eqs ).Trim ( );
                    expression = expression.Substring ( eqs + 1 ).Trim ( );

                    if ( name.Equals ( "E", StringComparison.OrdinalIgnoreCase ) || name.Equals ( "pi", StringComparison.OrdinalIgnoreCase )
                        || name.Equals ( "π", StringComparison.OrdinalIgnoreCase ) )
                    {
                        timingLogger.LogError ( "Cannot redefine this constant." );
                        return;
                    }
                }

                var res = language.Evaluate ( expression, out IEnumerable<Diagnostic> diagnostics );
                language = language.SetConstant ( name, res );
                timingLogger.WriteLine ( $"{name} = {res}" );
                foreach ( Diagnostic diagnostic in diagnostics )
                {
                    var str = $@"{CalculatorDiagnostics.HighlightRange ( expression, diagnostic.Range )}
{diagnostic.Id} {diagnostic.Severity}: {diagnostic.Description}";

                    switch ( diagnostic.Severity )
                    {
                        case DiagnosticSeverity.Error:
                            timingLogger.LogError ( str );
                            break;

                        case DiagnosticSeverity.Warning:
                            timingLogger.LogWarning ( str );
                            break;

                        case DiagnosticSeverity.Info:
                            timingLogger.LogInformation ( str );
                            break;

                        case DiagnosticSeverity.Hidden:
                            timingLogger.LogDebug ( str );
                            break;
                    }
                }
            }
            catch ( FatalParsingException fpex )
            {
                timingLogger.LogError ( CalculatorDiagnostics.HighlightRange ( expression, fpex.Range ) );
                timingLogger.LogError ( $"Runtime Error: {fpex.Message}" );
            }
            catch ( Exception ex )
            {
                timingLogger.LogError ( $"Unexpected exception: {ex}" );
            }
        }

        private static void ExecuteExpressionVerbose ( String expression )
        {
            try
            {
                using ( timingLogger.BeginScope ( $"Executing: {expression}", true ) )
                {
                    var diagnostics = new DiagnosticList();
                    CalculatorParser parser;
                    CalculatorTreeNode node;
                    Double res;
                    String rec;

                    using ( timingLogger.BeginScope ( "Lexer + Parser initialization", true ) )
                        parser = language.GetParser ( expression, diagnostics );

                    using ( timingLogger.BeginScope ( "Parsing", true ) )
                        node = parser.Parse ( );

                    using ( timingLogger.BeginScope ( "Evaluation", true ) )
                        res = node.Accept ( evaluator );

                    using ( timingLogger.BeginScope ( "Reconstruction", true ) )
                        rec = node.Accept ( reconstructor );

                    timingLogger.WriteLine ( $"{rec} = {res}" );
                    foreach ( Diagnostic diagnostic in diagnostics )
                    {
                        var str = $@"{CalculatorDiagnostics.HighlightRange ( expression, diagnostic.Range )}
{diagnostic.Id} {diagnostic.Severity}: {diagnostic.Description}";
                        switch ( diagnostic.Severity )
                        {
                            case DiagnosticSeverity.Error:
                                timingLogger.LogError ( str );
                                break;

                            case DiagnosticSeverity.Warning:
                                timingLogger.LogWarning ( str );
                                break;

                            case DiagnosticSeverity.Info:
                                timingLogger.LogInformation ( str );
                                break;

                            case DiagnosticSeverity.Hidden:
                                timingLogger.LogDebug ( str );
                                break;
                        }
                    }
                }
            }
            catch ( FatalParsingException fpex )
            {
                timingLogger.LogError ( GetExpressionContext ( fpex.Range.Start.Byte, expression ) );
                timingLogger.LogError ( $"{fpex.Range}: {fpex.Message}" );
            }
            catch ( Exception ex )
            {
                timingLogger.LogError ( $"Unexpected exception: {ex}" );
            }
        }

        private static readonly Random random = new Random ( );

        private static CalculatorTreeNode GenerateRandomExpression ( Int32 maxDepth )
        {
            UnaryOperator[] unaryOperators   = language.UnaryOperators.Values.ToArray ( );
            BinaryOperator[] binaryOperators = language.BinaryOperators.Values.ToArray ( );
            Constant[] constants             = language.Constants.Values.ToArray ( );
            Function[] functions             = language.Functions.Values.ToArray ( );
            if ( maxDepth > 0 )
            {
                switch ( random.Next ( 0, 3 ) )
                {
                    case 0:
                        UnaryOperator unop = unaryOperators[random.Next ( 0, unaryOperators.Length )];
                        return unop.Fix == UnaryOperatorFix.Postfix
                            ? ASTHelper.Postfix ( GenerateRandomExpression ( maxDepth - 1 ), unop.Operator )
                            : ASTHelper.Prefix ( unop.Operator, GenerateRandomExpression ( maxDepth - 1 ) );

                    case 1:
                        BinaryOperator binop = binaryOperators[random.Next ( 0, binaryOperators.Length )];
                        return ASTHelper.Binary ( GenerateRandomExpression ( maxDepth - 1 ), binop.Operator, GenerateRandomExpression ( maxDepth - 1 ) );

                    case 2:
                        Function func = functions[random.Next ( 0, functions.Length )];
                        var argCounts = func.Overloads.Keys.ToArray ( );
                        Delegate overload = func.Overloads[argCounts[random.Next ( 0, argCounts.Length )]];
                        var args = new Object[overload.Method.GetParameters ( ).Length];
                        for ( var i = 0; i < args.Length; i++ )
                            args[i] = GenerateRandomExpression ( maxDepth - 1 );
                        return ASTHelper.Function ( func.Name, args );

                    // Will never arrive here anyways.
                    default:
                        throw null;
                }
            }
            else
            {
                return random.Next ( 0, 2 ) == 1
                    ? ASTHelper.Number ( random.NextDouble ( ) )
                    : ( CalculatorTreeNode ) ASTHelper.Identifier ( constants[random.Next ( 0, constants.Length )].Identifier );
            }
        }

        private static void GenerateRandomExpression ( String restOfLine )
        {
            var depth = Int32.Parse ( restOfLine );
            timingLogger.WriteLine ( "Randomly generated expression:" );
            timingLogger.WriteLine ( $"{GenerateRandomExpression ( depth ).Accept ( reconstructor )}" );
        }

        private static void BenchmarkWithExpression ( String expression )
        {
            var results = new List<Int64> ( );
            var eval = 0D;
            for ( var i = 0; i < 200; i++ )
            {
                var sw = Stopwatch.StartNew ( );
                eval = language.Evaluate ( expression, out _ );
                sw.Stop ( );
                results.Add ( sw.ElapsedTicks );
            }

            var medianTmp = new Dictionary<Int64, Int32> ( );
            foreach ( var tick in results )
            {
                if ( !medianTmp.ContainsKey ( tick ) )
                    medianTmp[tick] = 0;
                medianTmp[tick]++;
            }
            var median = medianTmp.First ( kv => kv.Value == medianTmp.Max ( kv2 => kv2.Value ) ).Key;

            /*
             * ( a + b + c + d ) / e ≡ a / e + b / e + c / e + d / e
             */
            timingLogger.WriteLine ( $"Firstly, the evaluted result: {eval}" );
            timingLogger.WriteLine ( $"Average time: {Duration.Format ( results.Aggregate ( 0L, ( avg, ticks ) => avg + ticks / results.Count ) )}" );
            timingLogger.WriteLine ( $"Maximum time: {Duration.Format ( results.Max ( ) )}" );
            timingLogger.WriteLine ( $"Minimum time: {Duration.Format ( results.Min ( ) )}" );
            timingLogger.WriteLine ( $"Median time:  {Duration.Format ( median )}" );
            timingLogger.WriteLine ( "Execution times sorted by frequency:" );
            foreach ( KeyValuePair<Int64, Int32> kv in medianTmp.OrderBy ( kv => kv.Value ) )
                timingLogger.WriteLine ( $"  {kv.Value:000} times: {Duration.Format ( kv.Key, "{0:000.00}{1}" )}" );
        }
    }
}
