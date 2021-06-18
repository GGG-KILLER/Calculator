using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using GParse.Lexing;
using GParse.Lexing.Modular;
using GParse.Math;
using GParse.Utilities;
using Tsu;

namespace Calculator.Lexing
{
    /// <summary>
    /// A lexer builder that already pre-adds the identifier lexer delegate, whitespace lexer delegate,
    /// lparen definition, rparen definition, comma definition and definitions of different type of numbers.
    /// </summary>
    public sealed class CalculatorLexerBuilder : DelegateLexerBuilder<CalculatorTokenType>
    {
        /// <summary>
        /// Initializes a new <see cref="CalculatorLexerBuilder" />
        /// </summary>
        /// <param name="language"></param>
        public CalculatorLexerBuilder(CalculatorLanguage language)
            : base(CalculatorTokenType.EndOfExpression)
        {
            // Identifiers
            AddDelegate(null, static (reader, diagnostics) =>
            {
                if (reader.Peek() is not char ch || !CalculatorCharUtils.IsLeadingIdentifierChar(ch))
                    return Option.None<Token<CalculatorTokenType>>();

                var start = reader.Position;
                var ident = reader.ReadStringWhile(CalculatorCharUtils.IsTrailingIdentifierChar);
                return new Token<CalculatorTokenType>(ident, CalculatorTokenType.Identifier, new Range<int>(start, reader.Position), ident, ident);
            });

            // Superscript
            if (language.HasSuperscriptExponentiation())
            {
                AddDelegate(null, static (reader, diagnostics) =>
                {
                    if (reader.Peek() is not char firstChar || !SuperscriptChars.IsSupportedChar(firstChar))
                        return Option.None<Token<CalculatorTokenType>>();

                    var start = reader.Position;
                    var sign = 1;
                    if (firstChar is SuperscriptChars.Plus or SuperscriptChars.Minus)
                    {
                        reader.Advance(1);
                        if (firstChar is SuperscriptChars.Minus)
                            sign = -1;

                        if (reader.Peek() is char secondChar && !SuperscriptChars.IsSupportedChar(secondChar))
                        {
                            var errorRange = new Range<int>(start, reader.Position);
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
                            var errorRange = new Range<int>(reader.Position - 1, reader.Position);
                            diagnostics.Report(CalculatorDiagnostics.SyntaxError.InvalidSuperscript(errorRange, "unexpected sign inside number."));
                            continue;
                        }

                        number = number * 10 + digit;
                    }
                    var end = reader.Position;
                    var range = new Range<int>(start, end);

                    return new Token<CalculatorTokenType>("superscript", CalculatorTokenType.Superscript, range, number * sign);
                });
            }

            // Trivia
            AddDelegate(null, static (reader, diagnostics) =>
            {
                if (reader.Peek() is char ch && char.IsWhiteSpace(ch))
                {
                    var start = reader.Position;
                    var whitespaces = reader.FindOffset(static c => !char.IsWhiteSpace(c));
                    if (whitespaces < 0)
                        whitespaces = reader.Length - reader.Position;
                    reader.Advance(whitespaces);
                    var end = reader.Position;
                    var range = new Range<int>(start, end);
                    return new Token<CalculatorTokenType>("ws", CalculatorTokenType.Whitespace, range, true);
                }

                return Option.None<Token<CalculatorTokenType>>();
            });

            // Punctuations
            AddLiteral("(", CalculatorTokenType.LParen, "(");
            AddLiteral(")", CalculatorTokenType.RParen, ")");
            AddLiteral(",", CalculatorTokenType.Comma, ",");

            // Numbers
            AddDelegate("0b", static (reader, diagnostics) =>
            {
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
                    diagnostics.Report(CalculatorDiagnostics.SyntaxError.InvalidNumber(range, "binary"));

                return new Token<CalculatorTokenType>("bin-number", CalculatorTokenType.Number, range, (double) number);
            });
            AddDelegate("0o", static (reader, diagnostics) =>
            {
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

                if (digits is < 1 or > 21)
                    diagnostics.Report(CalculatorDiagnostics.SyntaxError.InvalidNumber(range, "octal"));

                return new Token<CalculatorTokenType>("oct-number", CalculatorTokenType.Number, range, (double) number);
            });
            AddDelegate("0x", static (reader, diagnostics) =>
            {
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

                if (digits is < 1 or > 22)
                    diagnostics.Report(CalculatorDiagnostics.SyntaxError.InvalidNumber(range, "hexadecimal"));

                return new Token<CalculatorTokenType>("hex-number", CalculatorTokenType.Number, range, (double) number);

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
            });
            AddDelegate(null, static (reader, diagnostics) =>
            {
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
                        diagnostics.Report(CalculatorDiagnostics.SyntaxError.InvalidNumber(range, "decimal"));
                    }

                    return new Token<CalculatorTokenType>("dec-number", CalculatorTokenType.Number, range, result);
                }

                return Option.None<Token<CalculatorTokenType>>();
            });

            var ops = new HashSet<string>(language.UnaryOperators.Values.Select(un => un.Operator), StringComparer.InvariantCulture);
            ops.UnionWith(language.BinaryOperators.Values.Select(bi => bi.Operator));
            ops.RemoveWhere(s => IsValidIdentifier(s));
            foreach (var op in ops)
                AddLiteral(op, CalculatorTokenType.Operator, op);

            Fallback = static (reader, diagnostics) =>
            {
                var start = reader.Position;
                var ch = reader.Read().Value;
                var end = reader.Position;
                var range = new Range<int>(start, end);
                diagnostics.Report(CalculatorDiagnostics.SyntaxError.UnknownCharacter(range, ch));
                return new Token<CalculatorTokenType>("unknown", CalculatorTokenType.Unknown, range, true);
            };
        }

        private static bool IsValidIdentifier(string str)
        {
            if (!CalculatorCharUtils.IsLeadingIdentifierChar(str[0]))
                return false;

            for (var i = 1; i < str.Length; i++)
            {
                if (!CalculatorCharUtils.IsTrailingIdentifierChar(str[i]))
                    return false;
            }

            return true;
        }
    }
}