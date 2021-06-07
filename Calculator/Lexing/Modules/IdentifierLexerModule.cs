using System;
using GParse;
using GParse.IO;
using GParse.Lexing;
using GParse.Lexing.Modular;
using GParse.Math;

namespace Calculator.Lexing.Modules
{
    /// <summary>
    /// A lexer module to parse identifiers
    /// </summary>
    public sealed class IdentifierLexerModule : ILexerModule<CalculatorTokenType>
    {
        /// <inheritdoc />
        public string Name => "Identifier Lexer Module";

        /// <inheritdoc />
        public string Prefix => null;

        /// <inheritdoc/>
        public bool TryConsume(ICodeReader reader, DiagnosticList diagnostics, out Token<CalculatorTokenType> token)
        {
            if (reader is null) throw new ArgumentNullException(nameof(reader));
            if (diagnostics is null) throw new ArgumentNullException(nameof(diagnostics));

            if (reader.Peek() is not char ch || !CalculatorCharUtils.IsLeadingIdentifierChar(ch))
            {
                token = default;
                return false;
            }

            var start = reader.Position;
            var ident = reader.ReadStringWhile(CalculatorCharUtils.IsTrailingIdentifierChar);
            token = new Token<CalculatorTokenType>(ident, CalculatorTokenType.Identifier, new Range<int>(start, reader.Position), ident, ident);
            return true;
        }
    }
}
