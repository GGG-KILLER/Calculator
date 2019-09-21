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
        /// <inheritdoc/>
        public String Name => "Superscript Parser";

        /// <inheritdoc/>
        public String Prefix => null;

        /// <inheritdoc/>
        public Boolean CanConsumeNext ( IReadOnlyCodeReader reader ) => reader.Peek ( ) is Char ch && SuperscriptChars.IsSupportedChar ( ch );

        /// <inheritdoc/>
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
            if ( reader.Peek ( ) is Char secondChar && !SuperscriptChars.IsSupportedChar ( secondChar ) )
            {
                diagnosticEmitter.Report ( CalculatorDiagnostics.SyntaxError.InvalidSuperscript ( start.To ( reader.Location ), "Expected a number after the sign" ) );
            }
            else
            {
                while ( reader.Read ( ) is Char digitChar && SuperscriptChars.IsSupportedChar ( digitChar ) )
                {
                    var digit = SuperscriptChars.TranslateChar ( digitChar );
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