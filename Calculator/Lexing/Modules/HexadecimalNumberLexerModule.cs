using System;
using System.Runtime.CompilerServices;
using GParse.IO;
using GParse.Lexing;
using GParse.Lexing.Modular;
using GParse.Math;

namespace Calculator.Lexing.Modules
{
    /// <summary>
    /// The hexadecimal number parser.
    /// </summary>
    public class HexadecimalNumberLexerModule : ILexerModule<CalculatorTokenType>
    {
        /// <inheritdoc/>
        public string Name => "Hexadecimal Number Lexer Module";

        /// <inheritdoc/>
        public string Prefix => "0x";

        /// <inheritdoc/>
        public bool TryConsume(ICodeReader reader, GParse.DiagnosticList diagnostics, out Token<CalculatorTokenType> token)
        {
            if (reader is null)
                throw new ArgumentNullException(nameof(reader));
            if (diagnostics is null)
                throw new ArgumentNullException(nameof(diagnostics));

            var start = reader.Position;
            reader.Advance(2);

            var number = 0L;
            var digits = 0;
            while (reader.Peek() is char ch && (isHexChar(ch, out var digit) || ch == '_'))
            {
                reader.Advance(1);
                if (ch == '_')
                    continue;
                number = (number << 4) | digit;
                digits++;
            }
            var end = reader.Position;
            var range = new Range<int>(start, end);

            if (digits < 1 || digits > 22)
                diagnostics.Report(CalculatorDiagnostics.SyntaxError.InvalidNumber(reader.GetLocation(range), "hexadecimal"));

            token = new Token<CalculatorTokenType>("hex-number", CalculatorTokenType.Number, range, (double) number);
            return true;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static bool isHexChar(char ch, out long value)
            {
                if (CharUtils.IsInRange('0', ch, '9'))
                {
                    value = ch - '0';
                    return true;
                }
                else if (CharUtils.IsInRange('a', CharUtils.AsciiLowerCase(ch), 'f'))
                {
                    value = 10 + (CharUtils.AsciiLowerCase(ch) - 'a');
                    return true;
                }

                value = default;
                return false;
            }
        }
    }
}
