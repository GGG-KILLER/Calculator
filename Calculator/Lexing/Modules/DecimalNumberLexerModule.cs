using System;
using System.Globalization;
using System.Text;
using GParse;
using GParse.Composable;
using GParse.IO;
using GParse.Lexing;
using GParse.Lexing.Composable;
using GParse.Lexing.Modular;
using GParse.Math;

namespace Calculator.Lexing.Modules
{
    /// <summary>
    /// The decimal number parser.
    /// </summary>
    public sealed class DecimalNumberLexerModule : ILexerModule<CalculatorTokenType>
    {
        /// <inheritdoc/>
        public string Name => "Decimal Number Lexer Module";

        /// <inheritdoc/>
        public string Prefix => null;

        /// <inheritdoc/>
        public bool TryConsume(ICodeReader reader, DiagnosticList diagnostics, out Token<CalculatorTokenType> token)
        {
            if (reader is null) throw new ArgumentNullException(nameof(reader));
            if (diagnostics is null) throw new ArgumentNullException(nameof(diagnostics));

            if (reader.Peek() is char firstChar && (CalculatorCharUtils.IsDecimal(firstChar) || firstChar == '.'))
            {
                var start = reader.Position;

                var builder = new StringBuilder();
                if (CalculatorCharUtils.IsDecimal(firstChar))
                {
                    while (reader.Peek() is char ch && (CalculatorCharUtils.IsDecimal(ch) || ch == '_'))
                    {
                        reader.Advance(1);
                        if (ch == '_')
                            continue;
                        builder.Append(ch);
                    }
                }

                if (reader.IsNext('.'))
                {
                    reader.Advance(1);
                    builder.Append('.');

                    while (reader.Peek() is char ch && (CalculatorCharUtils.IsDecimal(ch) || ch == '_'))
                    {
                        reader.Advance(1);
                        if (ch == '_')
                            continue;
                        builder.Append(ch);
                    }
                }

                if (reader.IsNext('E') || reader.IsNext('e'))
                {
                    reader.Advance(1);
                    builder.Append('e');

                    if (reader.IsNext('+') || reader.IsNext('-'))
                        builder.Append(reader.Read().Value);

                    while (reader.Peek() is char ch && (CalculatorCharUtils.IsDecimal(ch) || ch == '_'))
                    {
                        reader.Advance(1);
                        if (ch == '_')
                            continue;
                        builder.Append(ch);
                    }
                }

                var end = reader.Position;
                var range = new Range<int>(start, end);

                if (!double.TryParse(
                    builder.ToString(),
                    NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent,
                    CultureInfo.InvariantCulture,
                    out var result))
                {
                    var errorRange = reader.GetLocation(range);
                    diagnostics.Report(CalculatorDiagnostics.SyntaxError.InvalidNumber(errorRange, "decimal"));
                }

                token = new Token<CalculatorTokenType>("dec-number", CalculatorTokenType.Number, range, result);
                return true;
            }

            token = default;
            return false;
        }
    }
}
