using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using GParse.Math;
using GParse.Utilities;

namespace Calculator
{
    /// <summary>
    /// A class containing utility methods for <see cref="char"/>s.
    /// </summary>
    internal static class CalculatorCharUtils
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsDecimal(char ch) =>
            CharUtils.IsInRange('0', ch, '9');

        public static bool IsLeadingIdentifierChar(char ch)
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
                    return true;

                default:
                    return char.IsLetter(ch);
            }
        }

        public static bool IsTrailingIdentifierChar(char ch) =>
            IsLeadingIdentifierChar(ch) || char.IsDigit(ch);
    }
}
