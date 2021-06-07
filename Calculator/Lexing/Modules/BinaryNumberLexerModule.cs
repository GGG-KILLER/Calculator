using System;
using GParse;
using GParse.IO;
using GParse.Lexing;
using GParse.Lexing.Modules;

namespace Calculator.Lexing.Modules
{
    /// <summary>
    /// The binary number lexer module.
    /// </summary>
    public class BinaryNumberLexerModule : ILexerModule<CalculatorTokenType>
    {
        /// <inheritdoc/>
        public String Name => "Binary Number Lexer Module";

        /// <inheritdoc/>
        public String Prefix => "0b";

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

            var number = 0;
            var digits = 0;
            while ( reader.Peek ( ) is Char ch && ( CharUtils.IsInRange ( '0', ch, '1' ) || ch == '_' ) )
            {
                reader.Advance ( 1 );
                if ( ch == '_' )
                    continue;
                number = ( number << 1 ) | ( ch - '0' );
                digits++;
            }
            SourceLocation end = reader.Location;

            if ( digits < 1 || digits > 64 )
                diagnosticEmitter.Report ( CalculatorDiagnostics.SyntaxError.InvalidNumber ( start.To ( end ), "binary" ) );

            reader.Restore ( start );
            var raw = reader.PeekString ( end.Byte - start.Byte );
            reader.Restore ( end );

            return new Token<CalculatorTokenType> ( "bin-number", raw, ( Double ) number, CalculatorTokenType.Number, start.To ( end ) );
        }
    }
}
