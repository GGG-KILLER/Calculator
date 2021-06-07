using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using GParse;
using GParse.Composable;
using GParse.IO;
using GParse.Lexing;
using GParse.Lexing.Composable;
using GParse.Lexing.Modular;
using GParse.Math;
using GParse.Utilities;

namespace Calculator.Lexing.Modules
{
    /// <summary>
    /// The decimal number parser.
    /// </summary>
    public sealed class DecimalNumberLexerModule : ILexerModule<CalculatorTokenType>
    {
        private static readonly GrammarNode<char> _regex =
            GrammarTreeOptimizer.Optimize(RegexParser.Parse(@"(?:\d[\d_]*(?:\.[\d_]+)?|\.[\d_]+)(?:[Ee][+-]?[\d_]+)?"));

        /// <inheritdoc/>
        public string Name => "Decimal Number Lexer Module";

        /// <inheritdoc/>
        public string Prefix => null;

        /// <inheritdoc/>
        public bool TryConsume(ICodeReader reader, DiagnosticList diagnostics, out Token<CalculatorTokenType> token)
        {
            if (reader is null) throw new ArgumentNullException(nameof(reader));
            if (diagnostics is null) throw new ArgumentNullException(nameof(diagnostics));

            var start = reader.Position;
            var match = GrammarTreeInterpreter.MatchString(reader, _regex);
            if (match.IsMatch && double.TryParse(
                match.Value.Replace("_", ""),
                NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent,
                CultureInfo.InvariantCulture,
                out var result))
            {
                token = new Token<CalculatorTokenType>("dec-number", CalculatorTokenType.Number, new Range<int>(start, reader.Position), result);
                return true;
            }
            else
            {
                reader.Restore(start);
                token = default;
                return false;
            }
        }
    }
}
