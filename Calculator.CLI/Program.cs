using System;
using Calculator.Lib;
using Calculator.Lib.AST;
using Calculator.Lib.Definitions;
using Calculator.Lib.Exceptions;
using Calculator.Lib.Visitors;
using GParse.Common.Errors;
using GUtils.Timing;

namespace Calculator.CLI
{
    internal class Program
    {
        private static CalculatorLang Language;
        private static MathExpressionReconstructor ExpressionReconstructor;
        private static CNodeTreeExecutor TreeExecutor;
        private static TimingArea Root;

        private static void Main ( )
        {
            using ( Root = new TimingArea ( "Initialization" ) )
            {
                TimingArea r = Root;
                using ( Root = new TimingArea ( "Building the language", Root ) )
                    Language = BuildLanguage ( );
                Root = r;
                using ( Root.TimeLine ( "Initializing the reconstructor" ) )
                    ExpressionReconstructor = new MathExpressionReconstructor ( );
                using ( Root.TimeLine ( "Initializing the executor" ) )
                    TreeExecutor = new CNodeTreeExecutor ( Language );
            }

            while ( true )
            {
                Console.Write ( '>' );
                var line = Console.ReadLine ( );
                line = line.Trim ( );
                if ( line == "q" || line == "e" || line == "quit" || line == "exit" )
                    break;

                if ( line.StartsWith ( "b " ) || line.StartsWith ( "bench " ) )
                    BenchmarkWithExpression ( line.Substring ( line.IndexOf ( ' ' ) ) );
                else
                    ExecuteExpression ( line );
            }
        }

        private static CalculatorLang BuildLanguage ( )
        {
            var lang = new CalculatorLang ( "Math", new Version ( 1, 0, 0 ) );

            // Constants
            using ( Root.TimeLine ( "Adding constants" ) )
            {
                lang.AddConstant ( "E", Math.E );
                lang.AddConstant ( "e", Math.E );
                lang.AddConstant ( "PI", Math.PI );
                lang.AddConstant ( "Pi", Math.PI );
                lang.AddConstant ( "pi", Math.PI );
                lang.AddConstant ( "π", Math.PI );
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

        private static void ExecuteExpression ( String expression )
        {
            try
            {
                using ( var exArea = new TimingArea ( $"Executing: {expression}" ) )
                {
                    CalculatorParser parser;
                    CASTNode ast;
                    Double res;
                    String rec;

                    using ( exArea.TimeLine ( "Lexing" ) )
                        parser = new CalculatorParser ( Language, new CalculatorLexer ( Language, expression ) );

                    using ( exArea.TimeLine ( "Parsing" ) )
                        ast = parser.Parse ( );

                    using ( exArea.TimeLine ( "Execution" ) )
                        res = ast.Accept ( TreeExecutor );

                    using ( exArea.TimeLine ( "Reconstruction" ) )
                        rec = ast.Accept ( ExpressionReconstructor );

                    exArea.Log ( $"{rec} = {res}" );
                }
            }
            catch ( LexException ex )
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine ( $"{ex.Location} {ex}" );
                Console.ResetColor ( );
            }
            catch ( ParseException ex )
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine ( $"{ex.Location} {ex}" );
                Console.ResetColor ( );
            }
            catch ( CalculatorException ex )
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine ( $"Runtime exception: {ex.Message}" );
                Console.ResetColor ( );
            }
            catch ( Exception ex )
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine ( $"Unexpected exception: {ex}" );
                Console.ResetColor ( );
            }
        }

        private static void BenchmarkWithExpression ( String expression )
        {
            for ( var i = 0; i < 50; i++ )
                ExecuteExpression ( expression );
        }
    }
}
