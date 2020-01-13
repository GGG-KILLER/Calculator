﻿using System;
using System.Collections.Generic;

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
        public const Char Zero  = '\u2070'; // ⁰

        /// <summary>
        /// The superscript char one: ¹
        /// </summary>
        public const Char One   = '\u00b9'; // ¹

        /// <summary>
        /// The superscript char two: ²
        /// </summary>
        public const Char Two   = '\u00b2'; // ²

        /// <summary>
        /// The superscript char three: ³
        /// </summary>
        public const Char Three = '\u00b3'; // ³

        /// <summary>
        /// The superscript char four: ⁴
        /// </summary>
        public const Char Four  = '\u2074'; // ⁴

        /// <summary>
        /// The superscript char five: ⁵
        /// </summary>
        public const Char Five  = '\u2075'; // ⁵

        /// <summary>
        /// The superscript char six: ⁶
        /// </summary>
        public const Char Six   = '\u2076'; // ⁶

        /// <summary>
        /// The superscript char seven: ⁷
        /// </summary>
        public const Char Seven = '\u2077'; // ⁷

        /// <summary>
        /// The superscript char eight: ⁸
        /// </summary>
        public const Char Eight = '\u2078'; // ⁸

        /// <summary>
        /// The superscript char nine: ⁹
        /// </summary>
        public const Char Nine  = '\u2079'; // ⁹

        /// <summary>
        /// The superscript char plus: ⁺
        /// </summary>
        public const Char Plus  = '\u207a'; // ⁺

        /// <summary>
        /// The superscript char minus: ⁻
        /// </summary>
        public const Char Minus = '\u207b'; // ⁻

        /// <summary>
        /// Whether the provided char is one of the supported superscript chars
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static Boolean IsSupportedChar ( Char ch )
        {
            switch ( ch )
            {
                case Zero:
                case One:
                case Two:
                case Three:
                case Four:
                case Five:
                case Six:
                case Seven:
                case Eight:
                case Nine:
                case Plus:
                case Minus:
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Translates a superscript character to a <see cref="Double"/>
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static Double TranslateChar ( Char ch ) =>
            ch switch
            {
                Zero => 0d,

                One => 1d,

                Two => 2d,

                Three => 3d,

                Four => 4d,

                Five => 5d,

                Six => 6d,

                Seven => 7d,

                Eight => 8d,

                Nine => 9d,

                Plus => -1d,

                Minus => -1d,

                _ => throw new NotSupportedException ( ),
            };

        /// <summary>
        /// Translates a digit into a superscript character
        /// </summary>
        /// <param name="digit"></param>
        /// <returns></returns>
        public static Char TranslateDigit ( Byte digit ) =>
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

                _ => throw new NotSupportedException ( ),
            };

        /// <summary>
        /// Translates a number into superscript,
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static String TranslateNumber ( Int32 number )
        {
            var queue = new Stack<Char>();
            var isNeg = false;
            if ( number < 0 )
            {
                isNeg = true;
                number = -number;
            }

            while ( number > 0 )
            {
                number = Math.DivRem ( number, 10, out var remainder );
                queue.Push ( TranslateDigit ( ( Byte ) remainder ) );
            }

            if ( isNeg )
                return Minus + new String ( queue.ToArray ( ) );
            else
                return new String ( queue.ToArray ( ) );
        }
    }
}