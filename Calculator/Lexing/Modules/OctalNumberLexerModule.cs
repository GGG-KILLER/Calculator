using System;
using GParse.IO;
using GParse.Lexing;
using GParse.Lexing.Modular;
using GParse.Math;

namespace Calculator.Lexing.Modules
{
    /// <summary>
    /// The octal number lexer module.
    /// </summary>
    public class OctalNumberLexerModule : ILexerModule<CalculatorTokenType>
    {
        /// <inheritdoc/>
        public string Name => "Octal Number Lexer Module";

        /// <inheritdoc/>
        public string Prefix => "0o";

        /// <inheritdoc/>
        public bool TryConsume(ICodeReader reader, GParse.DiagnosticList diagnostics, out Token<CalculatorTokenType> token)
        {
            if (reader is null)
                throw new ArgumentNullException(nameof(reader));
            if (diagnostics is null)
                throw new ArgumentNullException(nameof(diagnostics));

            var start = reader.Position;
            reader.Advance(2);

            var number = 0;
            var digits = 0;
            while (reader.Peek() is char ch && (CharUtils.IsInRange('0', ch, '7') || ch == '_'))
            {
                reader.Advance(1);
                if (ch == '_')
                    continue;
                number = (number << 3) | (ch - '0');
                digits++;
            }
            var end = reader.Position;
            var range = new Range<int>(start, end);

            if (digits < 1 || digits > 21)
                diagnostics.Report(CalculatorDiagnostics.SyntaxError.InvalidNumber(reader.GetLocation(range), "octal"));

            token = new Token<CalculatorTokenType>("oct-number", CalculatorTokenType.Number, range, (double) number);
            return true;
        }
    }
}
