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
    public class IdentifierLexerModule : ILexerModule<CalculatorTokenType>
    {
        /// <inheritdoc />
        public string Name => "Identifier Lexer Module";

        /// <inheritdoc />
        public string Prefix => null;

        /// <summary>
        /// Checks whether the provided character is a valid character to start an identifier
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        protected virtual bool IsFirstIdentifierChar(char ch)
        {
            switch (ch)
            {
                case '\u2200': // ∀: FOR ALL (U+2200)
                case '\u2201': // ∁: COMPLEMENT (U+2201)
                case '\u2202': // ∂: PARTIAL DIFFERENTIAL (U+2202)
                case '\u2203': // ∃: THERE EXISTS (U+2203)
                case '\u2204': // ∄: THERE DOES NOT EXIST (U+2204)
                case '\u2205': // ∅: EMPTY SET (U+2205)
                case '\u220F': // ∏: N-ARY PRODUCT (U+220F)
                case '\u2210': // ∐: N-ARY COPRODUCT (U+2210)
                case '\u2211': // ∑: N-ARY SUMMATION (U+2211)
                case '\u221E': // ∞: INFINITY (U+221E)
                case '\u222B': // ∫: INTEGRAL (U+222B)
                case '\u222C': // ∬: DOUBLE INTEGRAL (U+222C)
                case '\u222D': // ∭: TRIPLE INTEGRAL (U+222D)
                case '\u222E': // ∮: CONTOUR INTEGRAL (U+222E)
                case '\u222F': // ∯: SURFACE INTEGRAL (U+222F)
                case '\u2230': // ∰: VOLUME INTEGRAL (U+2230)
                case '\u2231': // ∱: CLOCKWISE INTEGRAL (U+2231)
                case '\u2232': // ∲: CLOCKWISE CONTOUR INTEGRAL (U+2232)
                case '\u2233': // ∳: ANTICLOCKWISE CONTOUR INTEGRAL (U+2233)
                case char l when char.IsLetter(l):
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        protected virtual bool IsTrailingIdentifierChar(char ch) =>
            IsFirstIdentifierChar(ch) || char.IsDigit(ch);

        /// <inheritdoc/>
        public bool TryConsume(ICodeReader reader, GParse.DiagnosticList diagnostics, out Token<CalculatorTokenType> token)
        {
            if (reader is null) throw new ArgumentNullException(nameof(reader));
            if (diagnostics is null) throw new ArgumentNullException(nameof(diagnostics));

            if (reader.Peek() is not char ch || !IsFirstIdentifierChar(ch))
            {
                token = default;
                return false;
            }

            var start = reader.Position;
            var ident = reader.ReadStringWhile(IsTrailingIdentifierChar);
            token = new Token<CalculatorTokenType>(ident, CalculatorTokenType.Identifier, new Range<int>(start, reader.Position), ident, ident);
            return true;
        }
    }
}
