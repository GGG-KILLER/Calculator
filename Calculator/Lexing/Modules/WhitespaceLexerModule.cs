using System;
using GParse;
using GParse.IO;
using GParse.Lexing;
using GParse.Lexing.Modular;
using GParse.Math;

namespace Calculator.Lexing.Modules
{
    /// <summary>
    /// The module that consumes whitespace.
    /// </summary>
    public class WhitespaceLexerModule : ILexerModule<CalculatorTokenType>
    {
        /// <inheritdoc/>
        public string Name => "Whitespace Lexer Module";

        /// <inheritdoc/>
        public string Prefix => null;

        /// <inheritdoc/>
        public bool TryConsume(ICodeReader reader, DiagnosticList diagnostics, out Token<CalculatorTokenType> token)
        {
            if (reader is null) throw new ArgumentNullException(nameof(reader));
            if (diagnostics is null) throw new ArgumentNullException(nameof(diagnostics));

            if (reader.Peek() is char ch && char.IsWhiteSpace(ch))
            {
                var start = reader.Position;
                var whitespaces = reader.FindOffset(static c => !char.IsWhiteSpace(c));
                reader.Advance(whitespaces);
                var end = reader.Position;
                var range = new Range<int>(start, end);
                token = new Token<CalculatorTokenType>("ws", CalculatorTokenType.Whitespace, range, true);
                return true;
            }

            token = default;
            return false;
        }
    }
}
