using System;
using GParse.IO;
using GParse.Lexing;
using GParse.Lexing.Modular;
using GParse.Math;

namespace Calculator.Lexing.Modules
{

    /// <summary>
    /// The lexer module responsible for parsing superscript (⁻¹²³) numbers
    /// </summary>
    public sealed class SuperscriptLexerModule : ILexerModule<CalculatorTokenType>
    {
        /// <inheritdoc/>
        public string Name => "Superscript Parser";

        /// <inheritdoc/>
        public string Prefix => null;

        /// <inheritdoc/>
        public bool TryConsume(ICodeReader reader, GParse.DiagnosticList diagnostics, out Token<CalculatorTokenType> token)
        {
            if (reader is null) throw new ArgumentNullException(nameof(reader));
            if (diagnostics is null) throw new ArgumentNullException(nameof(diagnostics));

            if (reader.Peek() is not char firstChar || !SuperscriptChars.IsSupportedChar(firstChar))
            {
                token = default;
                return false;
            }

            var start = reader.Position;
            var sign = 1;
            if (firstChar is SuperscriptChars.Plus or SuperscriptChars.Minus)
            {
                reader.Advance(1);
                if (firstChar is SuperscriptChars.Minus)
                    sign = -1;

                if (reader.Peek() is char secondChar && !SuperscriptChars.IsSupportedChar(secondChar))
                {
                    var errorRange = reader.GetLocation(new Range<int>(start, reader.Position));
                    diagnostics.Report(CalculatorDiagnostics.SyntaxError.InvalidSuperscript(errorRange, "Expected a number after the sign"));
                }
            }

            var number = 0;
            while (reader.Peek() is char digitChar && SuperscriptChars.IsSupportedChar(digitChar))
            {
                reader.Advance(1);
                var digit = SuperscriptChars.TranslateChar(digitChar);
                if (digit < 0)
                {
                    var errorRange = reader.GetLocation(new Range<int>(reader.Position - 1, reader.Position));
                    diagnostics.Report(CalculatorDiagnostics.SyntaxError.InvalidSuperscript(errorRange, "unexpected sign inside number."));
                    continue;
                }

                number = number * 10 + digit;
            }
            var end = reader.Position;
            var range = new Range<int>(start, end);

            token = new Token<CalculatorTokenType>("superscript", CalculatorTokenType.Superscript, range, number * sign);
            return true;
        }
    }
}