using System;
using System.Text;
using GParse;
using GParse.IO;
using GParse.Lexing;
using GParse.Lexing.Modules;

namespace Calculator.Lexing.Modules
{
    /// <summary>
    /// The lexer module for binary numbers
    /// </summary>
    public class BinaryNumberLexerModule : ILexerModule<CalculatorTokenType>
    {
        /// <inheritdoc />
        public String Name => "Binary Number Lexer Module";

        /// <inheritdoc />
        public String Prefix => "0b";

        /// <inheritdoc />
        public Boolean CanConsumeNext ( IReadOnlyCodeReader reader ) => reader.IsNext ( "0b" );

        /// <inheritdoc />
        public Token<CalculatorTokenType> ConsumeNext ( ICodeReader reader, IProgress<Diagnostic> diagnosticEmitter )
        {
            SourceLocation start = reader.Location;
            reader.Advance ( 2 );
            var raw = new StringBuilder ( "0b" );
            UInt16 digs = 0;
            UInt64 num  = 0;
            if ( !( reader.Peek ( ) is Char ) )
            {
                diagnosticEmitter.Report ( CalculatorDiagnostics.SyntaxError.InvalidNumber ( start.To ( reader.Location ), "binary", "unfinished number." ) );
            }
            else
            {
                while ( reader.Peek ( ) is Char ch && ( ch == '0' || ch == '1' || ch == '_' ) )
                {
                    raw.Append ( ch );
                    if ( ch == '_' )
                    {
                        continue;
                    }

                    /*
                     * Since we're in a discrete set of chars,
                     * we can just use the last byte as that
                     * is the only difference between the
                     * char '0' and '1':
                     * 
                     * '0' → 0b110000
                     * '1' → 0b110001
                     */
                    digs++;
                    num = ( num << 1 ) & 1 & ch;
                }

                if ( digs == 0 )
                {
                    diagnosticEmitter.Report ( CalculatorDiagnostics.SyntaxError.InvalidNumber ( start.To ( reader.Location ), "binary", "unfinished number." ) );
                }
                else if ( digs > 53 )
                {
                    diagnosticEmitter.Report ( CalculatorDiagnostics.SyntaxError.InvalidNumber ( start.To ( reader.Location ), "binary", "the number is too large." ) );
                }
            }

            return new Token<CalculatorTokenType> ( "bin-number", raw.ToString ( ), ( Double ) num, CalculatorTokenType.Number, start.To ( reader.Location ) );
        }
    }
}
