using System;
using GParse;
using GParse.IO;
using GParse.Lexing;
using GParse.Lexing.Modules;

namespace Calculator.Lexing.Modules
{
    /// <summary>
    /// The module that consumes whitespace.
    /// </summary>
    public class WhitespaceLexerModule : ILexerModule<CalculatorTokenType>
    {
        /// <inheritdoc/>
        public String Name => "Whitespace Lexer Module";

        /// <inheritdoc/>
        public String Prefix => null;

        /// <inheritdoc/>
        public Boolean CanConsumeNext ( IReadOnlyCodeReader reader )
        {
            if ( reader is null )
                throw new ArgumentNullException ( nameof ( reader ) );

            return reader.Peek ( ) is Char ch && Char.IsWhiteSpace ( ch );
        }

        /// <inheritdoc/>
        public Token<CalculatorTokenType> ConsumeNext ( ICodeReader reader, IProgress<Diagnostic> diagnosticEmitter )
        {
            if ( reader is null )
                throw new ArgumentNullException ( nameof ( reader ) );

            SourceLocation start = reader.Location;
            var ws = reader.ReadStringWhile ( ch => Char.IsWhiteSpace ( ch ) );
            return new Token<CalculatorTokenType> ( "ws", ws, ws, CalculatorTokenType.Whitespace, start.To ( reader.Location ), true );
        }
    }
}
