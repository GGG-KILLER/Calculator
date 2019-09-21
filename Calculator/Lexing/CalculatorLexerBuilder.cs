using System;
using System.Collections.Generic;
using System.Linq;
using Calculator.Lexing.Modules;
using GParse.Lexing;

namespace Calculator.Lexing
{
    /// <summary>
    /// A lexer builder that already pre-adds the identifier lexer module, whitespace definition, lparen
    /// definition, rparen definition, comma definition and definitions of different type of numbers
    /// </summary>
    public class CalculatorLexerBuilder : ModularLexerBuilder<CalculatorTokenType>
    {
        private static Boolean IsValidIdentifier ( in String str )
        {
            if ( !Char.IsLetter ( str[0] ) && str[0] != '_' )
                return false;

            for ( var i = 1; i < str.Length; i++ )
            {
                if ( !Char.IsLetterOrDigit ( str[i] ) && str[i] != '_' )
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Initializes a new <see cref="CalculatorLexerBuilder" />
        /// </summary>
        /// <param name="language"></param>
        public CalculatorLexerBuilder ( CalculatorLanguage language )
        {
#pragma warning disable CC0067 // Virtual Method Called On Constructor

            // Identifiers
            this.AddModule ( new IdentifierLexerModule ( ) );

            // Superscript
            if ( language.HasSuperscriptExponentiation ( ) )
                this.AddModule ( new SuperscriptLexerModule ( ) );

            // Trivia
            this.AddRegex ( "ws", CalculatorTokenType.Whitespace, @"\s+", null, null, true );

            // Punctuations
            this.AddLiteral ( "(", CalculatorTokenType.LParen, "(" );
            this.AddLiteral ( ")", CalculatorTokenType.RParen, ")" );
            this.AddLiteral ( ",", CalculatorTokenType.Comma, "," );

            // Numbers
            this.AddRegex ( "bin-number", CalculatorTokenType.Number, "0b([01]+)", "0b", match => ( Double ) Convert.ToInt64 ( match.Groups[1].Value, 2 ) );

            this.AddRegex ( "oct-number", CalculatorTokenType.Number, "0o([0-7]+)", "0o", match => ( Double ) Convert.ToInt64 ( match.Groups[1].Value, 8 ) );

            this.AddRegex ( "hex-number", CalculatorTokenType.Number, "0x([a-fA-F0-9]+)", "0x", match => ( Double ) Convert.ToInt64 ( match.Groups[1].Value, 16 ) );

            this.AddRegex ( "dec-number", CalculatorTokenType.Number, @"(?:\d+(?:\.\d+)?|\.\d+)(?:e[+-]?\d+)?", null, match => ( Double ) Convert.ToDouble ( match.Value ) );

            var ops = new HashSet<String> ( language.UnaryOperators.Values.Select ( un => un.Operator ) );
            ops.UnionWith ( language.BinaryOperators.Values.Select ( bi => bi.Operator ) );
            ops.RemoveWhere ( s => IsValidIdentifier ( s ) );
            foreach ( var op in ops )
                this.AddLiteral ( op.ToLowerInvariant ( ), CalculatorTokenType.Operator, op );

#pragma warning restore CC0067 // Virtual Method Called On Constructor
        }
    }
}