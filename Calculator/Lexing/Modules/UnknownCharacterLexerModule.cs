using System;
using GParse.IO;
using GParse.Lexing;
using GParse.Lexing.Modular;
using GParse.Math;

namespace Calculator.Lexing.Modules
{
    /// <summary>
    /// A module that eats up unknown characters.
    /// </summary>
    public sealed class UnknownCharacterLexerModule : ILexerModule<CalculatorTokenType>
    {
        /// <inheritdoc/>
        public string Name => "Unknown Character Lexer Module";

        /// <inheritdoc/>
        public string Prefix => null;

        /// <inheritdoc/>
        public bool TryConsume(ICodeReader reader, GParse.DiagnosticList diagnostics, out Token<CalculatorTokenType> token)
        {
            if (reader is null)
                throw new ArgumentNullException(nameof(reader));
            if (diagnostics is null)
                throw new ArgumentNullException(nameof(diagnostics));

            var start = reader.Position;
            var ch = reader.Read().Value;
            var end = reader.Position;
            var range = new Range<int>(start, end);
            diagnostics.Report(CalculatorDiagnostics.SyntaxError.UnknownCharacter(reader.GetLocation(range), ch));
            token = new Token<CalculatorTokenType>("unknown", CalculatorTokenType.Unknown, range, true);
            return true;
        }
    }
}
