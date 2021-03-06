﻿using System;
using System.Diagnostics;

namespace Calculator.Lexing
{
    /// <summary>
    /// All superscript chars
    /// </summary>
    public class SuperscriptChars
    {
        /// <summary>
        /// The superscript char zero: ⁰
        /// </summary>
        public const char Zero = '\u2070'; // ⁰

        /// <summary>
        /// The superscript char one: ¹
        /// </summary>
        public const char One = '\u00b9'; // ¹

        /// <summary>
        /// The superscript char two: ²
        /// </summary>
        public const char Two = '\u00b2'; // ²

        /// <summary>
        /// The superscript char three: ³
        /// </summary>
        public const char Three = '\u00b3'; // ³

        /// <summary>
        /// The superscript char four: ⁴
        /// </summary>
        public const char Four = '\u2074'; // ⁴

        /// <summary>
        /// The superscript char five: ⁵
        /// </summary>
        public const char Five = '\u2075'; // ⁵

        /// <summary>
        /// The superscript char six: ⁶
        /// </summary>
        public const char Six = '\u2076'; // ⁶

        /// <summary>
        /// The superscript char seven: ⁷
        /// </summary>
        public const char Seven = '\u2077'; // ⁷

        /// <summary>
        /// The superscript char eight: ⁸
        /// </summary>
        public const char Eight = '\u2078'; // ⁸

        /// <summary>
        /// The superscript char nine: ⁹
        /// </summary>
        public const char Nine = '\u2079'; // ⁹

        /// <summary>
        /// The superscript char plus: ⁺
        /// </summary>
        public const char Plus = '\u207a'; // ⁺

        /// <summary>
        /// The superscript char minus: ⁻
        /// </summary>
        public const char Minus = '\u207b'; // ⁻

        /// <summary>
        /// Whether the provided char is one of the supported superscript chars
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static bool IsSupportedChar(char ch)
        {
            return ch switch
            {
                Zero or One or Two or Three or Four or Five or Six or Seven or Eight or Nine or Plus or Minus => true,
                _ => false,
            };
        }

        /// <summary>
        /// Translates a superscript character to a <see cref="double"/>
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static int TranslateChar(char ch) =>
            ch switch
            {
                Zero => 0,
                One => 1,
                Two => 2,
                Three => 3,
                Four => 4,
                Five => 5,
                Six => 6,
                Seven => 7,
                Eight => 8,
                Nine => 9,
                Plus => -1,
                Minus => -1,
                _ => throw new NotSupportedException(),
            };

        /// <summary>
        /// Translates a digit into a superscript character
        /// </summary>
        /// <param name="digit"></param>
        /// <returns></returns>
        public static char TranslateDigit(byte digit) =>
            digit switch
            {
                0 => Zero,
                1 => One,
                2 => Two,
                3 => Three,
                4 => Four,
                5 => Five,
                6 => Six,
                7 => Seven,
                8 => Eight,
                9 => Nine,
                _ => throw new NotSupportedException(),
            };

        /// <summary>
        /// Translates a number into superscript,
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string TranslateNumber(int number)
        {
            // Max int length is 11 chars (10 digits + sign)
            Span<char> buffer = stackalloc char[11];
            var isNeg = false;
            if (number < 0)
            {
                isNeg = true;
                number = -number;
            }

            var idx = 11;
            while (number > 0)
            {
                Debug.Assert(idx > 1);
                number = Math.DivRem(number, 10, out var remainder);
                buffer[--idx] = TranslateDigit((byte) remainder);
            }

            if (isNeg)
                buffer[--idx] = Minus;

            return buffer.Slice(idx).ToString();
        }
    }
}