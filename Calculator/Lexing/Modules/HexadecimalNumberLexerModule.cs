using System;
using System.Runtime.CompilerServices;
using GParse;
using GParse.IO;
using GParse.Lexing;
using GParse.Lexing.Modules;

namespace Calculator.Lexing.Modules
{
    /// <summary>
    /// The hexadecimal number parser.
    /// </summary>
    public class HexadecimalNumberLexerModule : ILexerModule<CalculatorTokenType>
    {
        /// <inheritdoc/>
        public String Name => "Hexadecimal Number Lexer Module";

        /// <inheritdoc/>
        public String Prefix => "0x";

        /// <inheritdoc/>
        public Boolean CanConsumeNext ( IReadOnlyCodeReader reader ) => true;

        /// <inheritdoc/>
        public Token<CalculatorTokenType> ConsumeNext ( ICodeReader reader, IProgress<Diagnostic> diagnosticEmitter )
        {
            if ( reader is null )
                throw new ArgumentNullException ( nameof ( reader ) );
            if ( diagnosticEmitter is null )
                throw new ArgumentNullException ( nameof ( diagnosticEmitter ) );

            SourceLocation start = reader.Location;
            reader.Advance ( 2 );

            var number = 0L;
            var digits = 0;
            while ( reader.Peek ( ) is Char ch && ( isHexChar ( ch, out var digit ) || ch == '_' ) )
            {
                reader.Advance ( 1 );
                if ( ch == '_' )
                    continue;
                number = ( number << 4 ) | digit;
                digits++;
            }
            SourceLocation end = reader.Location;

            if ( digits < 0 || digits > 22 )
                diagnosticEmitter.Report ( CalculatorDiagnostics.SyntaxError.InvalidNumber ( start.To ( end ), "octal" ) );

            reader.Restore ( start );
            var raw = reader.PeekString ( end.Byte - start.Byte );
            reader.Restore ( end );

            return new Token<CalculatorTokenType> ( "hex-number", raw, ( Double ) number, CalculatorTokenType.Number, start.To ( end ) );

            [MethodImpl ( MethodImplOptions.AggressiveInlining )]
            static Boolean isHexChar ( Char ch, out Int64 value )
            {
                if ( CharUtils.IsInRange ( '0', ch, '9' ) )
                {
                    value = ch - '0';
                    return true;
                }
                else if ( CharUtils.IsInRange ( 'a', CharUtils.AsciiLowerCase ( ch ), 'f' ) )
                {
                    value = 10 + ( ch - 'a' );
                    return true;
                }

                value = default;
                return false;
            }
        }
    }
}
