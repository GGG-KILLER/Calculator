using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Calculator.Definitions;
using Calculator.Lexing;
using Calculator.Parsing;
using Calculator.Parsing.AST;
using Calculator.Parsing.Visitors;
using GParse.Common.Errors;
using GParse.Parsing.Abstractions.Lexing;
using GUtils.Timing;

namespace Calculator.CLI
{
    internal class Program
    {
        private static CalculatorLanguage Language;
        private static TreeEvaluator Evaluator;
        private static TreeReconstructor Reconstructor;
        private static TimingArea Root;

        private static void Main ( )
        {
            using ( Root = new TimingArea ( "Initialization" ) )
            {
                TimingArea r = Root;
                using ( Root = new TimingArea ( "Building the language", Root ) )
                    Language = BuildLanguage ( );
                Root = r;
                using ( Root.TimeLine ( "Initializing the evaluator" ) )
                    Evaluator = new TreeEvaluator ( Language );
                using ( Root.TimeLine ( "Initializing the reconstructor" ) )
                    Reconstructor = new TreeReconstructor ( );
            }

            Root = new TimingArea ( "Runtime" );
            while ( true )
            {
                Console.Write ( '>' );
                var line = Console.ReadLine ( );
                line = line.Trim ( );
                if ( line == "q" || line == "e" || line == "quit" || line == "exit" )
                    break;

                if ( line.StartsWith ( "b " ) || line.StartsWith ( "bench " ) )
                    BenchmarkWithExpression ( line.Substring ( line.IndexOf ( ' ' ) ) );
                else if ( line.StartsWith ( "l " ) || line.StartsWith ( "lex " ) )
                    LexExpression ( line.Substring ( line.IndexOf ( ' ' ) + 1 ) );
                else if ( line.StartsWith ( "r " ) || line.StartsWith ( "random " ) )
                    GenerateRandomExpression ( line.Substring ( line.IndexOf ( ' ' ) + 1 ) );
                else if ( line.StartsWith ( "v " ) || line.StartsWith ( "verbose " ) )
                    ExecuteExpressionVerbose ( line.Substring ( line.IndexOf ( ' ' ) + 1 ) );
                else
                    ExecuteExpression ( line );
            }
        }

        private static CalculatorLanguage BuildLanguage ( )
        {
            var lang = new CalculatorLanguage ( );

            // Constants
            using ( Root.TimeLine ( "Adding constants" ) )
            {
                lang.AddConstant ( "E", Math.E, false );
                lang.AddConstant ( "pi", Math.PI, false );
                lang.AddConstant ( "π", Math.PI, false );
            }

            // Unary operators
            using ( Root.TimeLine ( "Adding unary operators" ) )
            {
                lang.AddUnaryOperator ( UnaryOperatorFix.Prefix, "-", 1, n => -n );
                lang.AddUnaryOperator ( UnaryOperatorFix.Prefix, "~", 1, n => ~( ( Int64 ) n ) );
                lang.AddUnaryOperator ( UnaryOperatorFix.Postfix, "!", 1, n =>
                {
                    if ( Double.IsInfinity ( n ) )
                        return n;
                    var r = 1D;
                    for ( var i = 2; i < n && !Double.IsInfinity ( r ) && !Double.IsInfinity ( i ); i++ )
                        r *= i;
                    return r;
                } );
            }

            // Binary operators Binary operators - Math operators
            using ( Root.TimeLine ( "Adding math binary operators" ) )
            {
                lang.AddBinaryOperator ( OperatorAssociativity.Left, "+", 1, ( lhs, rhs ) => lhs + rhs );
                lang.AddBinaryOperator ( OperatorAssociativity.Left, "-", 1, ( lhs, rhs ) => lhs - rhs );
                lang.AddBinaryOperator ( OperatorAssociativity.Left, "*", 2, ( lhs, rhs ) => lhs * rhs );
                lang.AddBinaryOperator ( OperatorAssociativity.Left, "/", 2, ( lhs, rhs ) => lhs / rhs );
                lang.AddBinaryOperator ( OperatorAssociativity.Left, "%", 2, ( lhs, rhs ) => lhs % rhs );
                lang.AddBinaryOperator ( OperatorAssociativity.Right, "^", 3, ( lhs, rhs ) => Math.Pow ( lhs, rhs ) );
            }

            // Binary operators - Logical operators
            using ( Root.TimeLine ( "Adding logical binary operators" ) )
            {
                lang.AddBinaryOperator ( OperatorAssociativity.Left, "<<", 4, ( lhs, rhs ) => ( Int64 ) lhs << ( Int32 ) rhs );
                lang.AddBinaryOperator ( OperatorAssociativity.Left, ">>", 4, ( lhs, rhs ) => ( Int64 ) lhs >> ( Int32 ) rhs );
                lang.AddBinaryOperator ( OperatorAssociativity.Left, "&", 5, ( lhs, rhs ) => ( Int64 ) lhs & ( Int64 ) rhs );
                lang.AddBinaryOperator ( OperatorAssociativity.Left, "|", 5, ( lhs, rhs ) => ( Int64 ) lhs | ( Int64 ) rhs );
            }

            // Functions Functions - Math
            using ( Root.TimeLine ( "Adding math functions" ) )
            {
                lang.AddFunction ( "abs", Math.Abs );
                lang.AddFunction ( "acos", Math.Acos );
                lang.AddFunction ( "asin", Math.Asin );
                lang.AddFunction ( "atan", Math.Atan );
                lang.AddFunction ( "atan2", Math.Atan2 );
                lang.AddFunction ( "ceil", Math.Ceiling );
                lang.AddFunction ( "cos", Math.Cos );
                lang.AddFunction ( "cosh", Math.Cosh );
                lang.AddFunction ( "exp", Math.Exp );
                lang.AddFunction ( "floor", Math.Floor );
                lang.AddFunction ( "ln", ( Func<Double, Double> ) Math.Log );
                lang.AddFunction ( "log", ( Func<Double, Double, Double> ) Math.Log );
                lang.AddFunction ( "log10", Math.Log10 );
                lang.AddFunction ( "log2", ( n ) => Math.Log ( n, 2 ) );
                lang.AddFunction ( "max", Math.Max );
                lang.AddFunction ( "min", Math.Min );
                lang.AddFunction ( "pow", Math.Pow );
                lang.AddFunction ( "round", Math.Round );
                lang.AddFunction ( "sin", Math.Sin );
                lang.AddFunction ( "sinh", Math.Sinh );
                lang.AddFunction ( "sqrt", Math.Sqrt );
                lang.AddFunction ( "tan", Math.Tan );
                lang.AddFunction ( "tanh", Math.Tanh );
                lang.AddFunction ( "truncate", Math.Truncate );
                lang.AddFunction ( "rot", ( a, b, c ) => ( b * c ) / a );
                /*
                 * a - b
                 * c - d
                 *
                 * a*d = c*b
                 *
                 * d = (c*b)/a
                 */
                lang.AddFunction ( "ruleOfThree", ( a, b, c ) => ( b * c ) / a );
            }

            // Functions - Logical
            using ( Root.TimeLine ( "Adding logical functions" ) )
            {
                lang.AddFunction ( "xor", ( lhs, rhs ) => ( Int64 ) lhs ^ ( Int64 ) rhs );
            }

            return lang;
        }

        private static void LexExpression ( String expression )
        {
            using ( var lexArea = new TimingArea ( $"Lexing: {expression}" ) )
            {
                ILexer<CalculatorTokenType> lexer;

                using ( lexArea.TimeLine ( "Lexer init" ) )
                    lexer = Language.GetLexer ( expression );

                using ( var tokArea = new TimingArea ( "Tokens", lexArea ) )
                    while ( !lexer.EOF )
                        tokArea.Log ( lexer.ConsumeToken ( ) );
            }
        }

        private static void ExecuteExpression ( String expression )
        {
            try
            {
                Root.Log ( $"{expression} = {new CalculatorParser ( Language.GetLexer ( expression ), Language ).Parse ( ).Accept ( Evaluator )}" );
            }
            catch ( LocationBasedException lbex )
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Root.Log ( expression );
                Root.Log ( new String ( ' ', lbex.Location.Byte ) + '^' );
                Root.Log ( $"{lbex.Location}: {lbex.Message}" );
                Console.ResetColor ( );
            }
            catch ( Exception ex )
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Root.Log ( $"Unexpected exception: {ex}" );
                Console.ResetColor ( );
            }
        }

        private static void ExecuteExpressionVerbose ( String expression )
        {
            try
            {
                using ( var exArea = new TimingArea ( $"Executing: {expression}", Root ) )
                {
                    ILexer<CalculatorTokenType> lexer;
                    CalculatorParser parser;
                    CalculatorASTNode node;
                    Double res;
                    String rec;

                    using ( exArea.TimeLine ( "Lexer + Parser initialization" ) )
                    {
                        lexer = Language.GetLexer ( expression );
                        parser = new CalculatorParser ( lexer, Language );
                    }

                    using ( exArea.TimeLine ( "Parsing" ) )
                        node = parser.Parse ( );

                    using ( exArea.TimeLine ( "Evaluation" ) )
                        res = node.Accept ( Evaluator );

                    using ( exArea.TimeLine ( "Reconstruction" ) )
                        rec = node.Accept ( Reconstructor );

                    exArea.Log ( $"{rec} = {res}" );
                }
            }
            catch ( LocationBasedException lbex )
            {
                Console.WriteLine ( expression );
                Console.WriteLine ( new String ( ' ', lbex.Location.Byte ) + '^' );
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine ( $"{lbex.Location}: {lbex.Message}" );
                Console.ResetColor ( );
            }
            catch ( Exception ex )
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine ( $"Unexpected exception: {ex}" );
                Console.ResetColor ( );
            }
        }

        private static readonly Random random = new Random ( );

        private static CalculatorASTNode GenerateRandomExpression ( Int32 maxDepth )
        {
            UnaryOperatorDef[] unaryOperators          = Language.UnaryOperators.ToArray ( );
            BinaryOperatorDef[] binaryOperators        = Language.BinaryOperators.ToArray ( );
            ConstantDef[] constants                    = Language.Constants.ToArray ( );
            KeyValuePair<String, Delegate>[] functions = Language.Functions.ToArray ( );
            if ( maxDepth > 0 )
            {
                switch ( random.Next ( 0, 3 ) )
                {
                    case 0:
                        UnaryOperatorDef unop = unaryOperators[random.Next ( 0, unaryOperators.Length )];
                        return ASTHelper.UnaryOperator ( unop.Operator, GenerateRandomExpression ( maxDepth - 1 ), unop.Fix );

                    case 1:
                        BinaryOperatorDef binop = binaryOperators[random.Next ( 0, binaryOperators.Length )];
                        return ASTHelper.BinaryOperator ( GenerateRandomExpression ( maxDepth - 1 ), binop.Operator, GenerateRandomExpression ( maxDepth - 1 ) );

                    case 2:
                        KeyValuePair<String, Delegate> func = functions[random.Next(0, functions.Length)];
                        var args = new Object[func.Value.Method.GetParameters ( ).Length];
                        for ( var i = 0; i < args.Length; i++ )
                            args[i] = GenerateRandomExpression ( maxDepth - 1 );
                        return ASTHelper.FunctionCall ( func.Key, args );

                    // Will never arrive here anyways.
                    default:
                        throw null;
                }
            }
            else
            {
                return random.Next ( 0, 2 ) == 1
                    ? ASTHelper.Number ( random.NextDouble ( ) )
                    : ( CalculatorASTNode ) ASTHelper.Identifier ( constants[random.Next ( 0, constants.Length )].Identifier );
            }
        }

        private static void GenerateRandomExpression ( String restOfLine )
        {
            var depth = Int32.Parse ( restOfLine );
            Root.Log ( "Randomly generated expression:" );
            Root.Log ( $"{GenerateRandomExpression ( depth ).Accept ( Reconstructor )}" );
        }

        private static void BenchmarkWithExpression ( String expression )
        {
            var results = new List<Int64> ( );
            var eval = 0D;
            for ( var i = 0; i < 200; i++ )
            {
                var sw = Stopwatch.StartNew ( );
                eval = new CalculatorParser ( Language.GetLexer ( expression ), Language ).Parse ( ).Accept ( Evaluator );
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
            Root.Log ( $"Firstly, the evaluted result: {eval}" );
            Root.Log ( $"Average time: {Duration.Format ( results.Aggregate ( 0L, ( avg, ticks ) => avg + ticks / results.Count ) )}" );
            Root.Log ( $"Maximum time: {Duration.Format ( results.Max ( ) )}" );
            Root.Log ( $"Minimum time: {Duration.Format ( results.Min ( ) )}" );
            Root.Log ( $"Median time:  {Duration.Format ( median )}" );
            Root.Log ( "Execution times sorted by frequency:" );
            foreach ( KeyValuePair<Int64, Int32> kv in medianTmp.OrderBy ( kv => kv.Value ) )
                Root.Log ( $"  {kv.Value:000} times: {Duration.Format ( kv.Key, "{0:000.00}{1}" )}" );
        }
    }
}
