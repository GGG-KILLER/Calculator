using System;
using GParse.IO;
using GParse.Lexing;
using GParse.Lexing.Modular;
using GParse.Math;
using GParse.Utilities;

namespace Calculator.Lexing.Modules
{
    /// <summary>
    /// The binary number lexer module.
    /// </summary>
    public sealed class BinaryNumberLexerModule : ILexerModule<CalculatorTokenType>
    {
        /// <inheritdoc/>
        public string Name => "Binary Number Lexer Module";

        /// <inheritdoc/>
        public string Prefix => "0b";

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
            while (reader.Peek() is char ch && (CharUtils.IsInRange('0', ch, '1') || ch == '_'))
            {
                reader.Advance(1);
                if (ch == '_')
                    continue;
                number = (number << 1) | (ch - '0');
                digits++;
            }
            var end = reader.Position;
            var range = new Range<int>(start, end);

            if (digits is < 1 or > 64)
                diagnostics.Report(CalculatorDiagnostics.SyntaxError.InvalidNumber(reader.GetLocation(range), "binary"));

            token = new Token<CalculatorTokenType>("bin-number", CalculatorTokenType.Number, range, (double) number);
            return true;
        }
    }
}
