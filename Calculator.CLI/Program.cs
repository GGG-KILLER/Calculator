using System;
using System.Diagnostics;
using Calculator.Lib;
using Calculator.Lib.Definitions;
using GParse.Common.AST;
using GParse.Common.Errors;

namespace Calculator.CLI
{
    internal class Program
    {
        private static void Main ( )
        {
            CalculatorLang lang = BuildLanguage ( );
            var vm = new CalculatorVM ( lang );
            do
            {
                var line = Console.ReadLine ( );
                line = line.Trim ( );
                if ( line == "exit" )
                    break;
                try
                {
                    var sw = Stopwatch.StartNew ( );
                    var lexer = new CalculatorLexer ( lang, line );
                    var parser = new CalculatorParser ( lang, lexer );
                    ASTNode ast = parser.Parse ( );
                    sw.Stop ( );
                    Console.WriteLine ( $"{HumanTime ( sw.ElapsedTicks )} elapsed parsing." );
                    sw.Restart ( );
                    var res = vm.Execute ( line );
                    sw.Stop ( );
                    Console.WriteLine ( $"{HumanTime ( sw.ElapsedTicks )} elapsed parsing + VM." );
                    Console.Write ( ast );
                    Console.Write ( " = " );
                    Console.WriteLine ( res );
                }
                catch ( LexException ex )
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine ( );
                    Console.WriteLine ( $"{ex.Location} {ex}" );
                    Console.ResetColor ( );
                }
                catch ( ParseException ex )
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine ( );
                    Console.WriteLine ( $"{ex.Location} {ex}" );
                    Console.ResetColor ( );
                }
                catch ( Exception ex )
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine ( );
                    Console.WriteLine ( $"VM: {ex}" );
                    Console.ResetColor ( );
                }
            }
            while ( true );
        }

        private static CalculatorLang BuildLanguage ( )
        {
            var lang = new CalculatorLang ( "Math", new Version ( 1, 0, 0 ) );
            // Constants
            lang.AddConstant ( "E", Math.E );
            lang.AddConstant ( "e", Math.E );
            lang.AddConstant ( "PI", Math.PI );
            lang.AddConstant ( "Pi", Math.PI );
            lang.AddConstant ( "pi", Math.PI );
            lang.AddConstant ( "π", Math.PI );

            // Unary operators
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

            // Binary operators Binary operators - Math operators
            lang.AddBinaryOperator ( OperatorAssociativity.Left, "+", 1, ( lhs, rhs ) => lhs + rhs );
            lang.AddBinaryOperator ( OperatorAssociativity.Left, "-", 1, ( lhs, rhs ) => lhs - rhs );
            lang.AddBinaryOperator ( OperatorAssociativity.Left, "*", 2, ( lhs, rhs ) => lhs * rhs );
            lang.AddBinaryOperator ( OperatorAssociativity.Left, "/", 2, ( lhs, rhs ) => lhs / rhs );
            lang.AddBinaryOperator ( OperatorAssociativity.Left, "%", 2, ( lhs, rhs ) => lhs % rhs );
            lang.AddBinaryOperator ( OperatorAssociativity.Right, "^", 3, ( lhs, rhs ) => Math.Pow ( lhs, rhs ) );
            // Binary operators - Logical operators
            lang.AddBinaryOperator ( OperatorAssociativity.Left, "<<", 4, ( lhs, rhs ) => ( Int64 ) lhs << ( Int32 ) rhs );
            lang.AddBinaryOperator ( OperatorAssociativity.Left, ">>", 4, ( lhs, rhs ) => ( Int64 ) lhs >> ( Int32 ) rhs );
            lang.AddBinaryOperator ( OperatorAssociativity.Left, "&", 5, ( lhs, rhs ) => ( Int64 ) lhs & ( Int64 ) rhs );
            lang.AddBinaryOperator ( OperatorAssociativity.Left, "|", 5, ( lhs, rhs ) => ( Int64 ) lhs | ( Int64 ) rhs );

            // Functions Functions - Math
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
            // Functions - Logical
            lang.AddFunction ( "xor", ( lhs, rhs ) => ( Int64 ) lhs ^ ( Int64 ) rhs );

            return lang;
        }

        private static readonly Double TicksPerMicrosecond = TimeSpan.TicksPerMillisecond / 1000D;

        private static String HumanTime ( Int64 ticks )
        {
            if ( ticks > TimeSpan.TicksPerMinute )
                return $"{( Double ) ticks / TimeSpan.TicksPerMinute}m";
            else if ( ticks > TimeSpan.TicksPerSecond )
                return $"{( Double ) ticks / TimeSpan.TicksPerSecond}s";
            else if ( ticks > TimeSpan.TicksPerMillisecond )
                return $"{( Double ) ticks / TimeSpan.TicksPerMillisecond}ms";
            else
                return $"{( Double ) ticks / TicksPerMicrosecond}μs";
        }
    }
}
