using System;
using System.Text;
using GParse;
using GParse.IO;
using GParse.Lexing;
using GParse.Lexing.Modules;

namespace Calculator.Lexing.Modules
{
    /// <summary>
    /// The lexer module responsible for parsing superscript (⁻¹²³) numbers
    /// </summary>
    public class SuperscriptLexerModule : ILexerModule<CalculatorTokenType>
    {
        /// <summary>
        /// All superscript chars
        /// </summary>
        protected class SuperscriptChars
        {
            /// <summary>
            /// The superscript char zero: ⁰
            /// </summary>
            public const Char Zero  = '\u2070'; // ⁰

            /// <summary>
            /// The superscript char one: ¹
            /// </summary>
            public const Char One   = '\u00b9'; // ¹

            /// <summary>
            /// The superscript char two: ²
            /// </summary>
            public const Char Two   = '\u00b2'; // ²

            /// <summary>
            /// The superscript char three: ³
            /// </summary>
            public const Char Three = '\u00b3'; // ³

            /// <summary>
            /// The superscript char four: ⁴
            /// </summary>
            public const Char Four  = '\u2074'; // ⁴

            /// <summary>
            /// The superscript char five: ⁵
            /// </summary>
            public const Char Five  = '\u2075'; // ⁵

            /// <summary>
            /// The superscript char six: ⁶
            /// </summary>
            public const Char Six   = '\u2076'; // ⁶

            /// <summary>
            /// The superscript char seven: ⁷
            /// </summary>
            public const Char Seven = '\u2077'; // ⁷

            /// <summary>
            /// The superscript char eight: ⁸
            /// </summary>
            public const Char Eight = '\u2078'; // ⁸

            /// <summary>
            /// The superscript char nine: ⁹
            /// </summary>
            public const Char Nine  = '\u2079'; // ⁹

            /// <summary>
            /// The superscript char plus: ⁺
            /// </summary>
            public const Char Plus  = '\u207a'; // ⁺

            /// <summary>
            /// The superscript char minus: ⁻
            /// </summary>
            public const Char Minus = '\u207b'; // ⁻
        }

        /// <inheritdoc />
        public String Name => "Superscript Parser";

        /// <inheritdoc />
        public String Prefix => null;

        private static Boolean IsSupportedSuperscript ( Char ch )
        {
            switch ( ch )
            {
                case SuperscriptChars.Zero:
                case SuperscriptChars.One:
                case SuperscriptChars.Two:
                case SuperscriptChars.Three:
                case SuperscriptChars.Four:
                case SuperscriptChars.Five:
                case SuperscriptChars.Six:
                case SuperscriptChars.Seven:
                case SuperscriptChars.Eight:
                case SuperscriptChars.Nine:
                case SuperscriptChars.Plus:
                case SuperscriptChars.Minus:
                    return true;

                default:
                    return false;
            }
        }

        private static Double TranslateSuperscript ( Char ch )
        {
            switch ( ch )
            {
                case SuperscriptChars.Zero:
                    return 0d;

                case SuperscriptChars.One:
                    return 1d;

                case SuperscriptChars.Two:
                    return 2d;

                case SuperscriptChars.Three:
                    return 3d;

                case SuperscriptChars.Four:
                    return 4d;

                case SuperscriptChars.Five:
                    return 5d;

                case SuperscriptChars.Six:
                    return 6d;

                case SuperscriptChars.Seven:
                    return 7d;

                case SuperscriptChars.Eight:
                    return 8d;

                case SuperscriptChars.Nine:
                    return 9d;

                case SuperscriptChars.Plus:
                    return -1d;

                case SuperscriptChars.Minus:
                    return -1d;

                default:
                    throw new NotSupportedException ( );
            }
        }

        /// <inheritdoc />
        public Boolean CanConsumeNext ( IReadOnlyCodeReader reader ) => reader.Peek ( ) is Char ch && IsSupportedSuperscript ( ch );

        /// <inheritdoc />
        public Token<CalculatorTokenType> ConsumeNext ( ICodeReader reader, IProgress<Diagnostic> diagnosticEmitter )
        {
            SourceLocation start = reader.Location;
            var sign = 1d;
            var raw = new StringBuilder ( );
            if ( reader.Peek ( ) is Char firstChar && ( firstChar == SuperscriptChars.Plus || firstChar == SuperscriptChars.Minus ) )
            {
                if ( firstChar == SuperscriptChars.Minus )
                {
                    sign = -1d;
                }

                raw.Append ( reader.Read ( ).Value );
            }

            var number = 0d;
            if ( reader.Peek ( ) is Char secondChar && !IsSupportedSuperscript ( secondChar ) )
            {
                diagnosticEmitter.Report ( CalculatorDiagnostics.SyntaxError.InvalidSuperscript ( start.To ( reader.Location ), "Expected a number after the sign" ) );
            }
            else
            {
                while ( reader.Read ( ) is Char digitChar && IsSupportedSuperscript ( digitChar ) )
                {
                    var digit = TranslateSuperscript ( digitChar );
                    raw.Append ( digitChar );
                    if ( digit < 0 )
                    {
                        diagnosticEmitter.Report ( CalculatorDiagnostics.SyntaxError.InvalidSuperscript ( start.To ( reader.Location ), "unexpected sign inside number." ) );
                        continue;
                    }

                    number = number * 10 + digit;
                }
            }

            return new Token<CalculatorTokenType> ( "superscript", raw.ToString ( ), number * sign, CalculatorTokenType.Superscript, start.To ( reader.Location ) );
        }
    }
}
