using System;
using GParse;
using GParse.IO;
using GParse.Lexing;
using GParse.Lexing.Modules;

namespace Calculator.Lexing.Modules
{
    /// <summary>
    /// A lexer module to parse identifiers
    /// </summary>
    public class IdentifierLexerModule : ILexerModule<CalculatorTokenType>
    {
        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public String Name => "Identifier Lexer Module";

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public String Prefix => null;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public Boolean CanConsumeNext ( SourceCodeReader reader ) =>
            reader.HasContent && Char.IsLetter ( ( Char ) reader.Peek ( ) );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="diagnosticReporter"></param>
        /// <returns></returns>
        public Token<CalculatorTokenType> ConsumeNext ( SourceCodeReader reader, IProgress<Diagnostic> diagnosticReporter )
        {
            SourceLocation start = reader.Location;
            var ident = reader.ReadStringWhile ( Char.IsLetterOrDigit );
            return new Token<CalculatorTokenType> ( ident, ident, ident, CalculatorTokenType.Identifier, start.To ( reader.Location ) );
        }
    }
}
