using System;
using System.Collections.Generic;
using System.Linq;
using Calculator.Lexing.Modules;
using GParse.Lexing.Modular;

namespace Calculator.Lexing
{
    /// <summary>
    /// A lexer builder that already pre-adds the identifier lexer module, whitespace definition, lparen
    /// definition, rparen definition, comma definition and definitions of different type of numbers
    /// </summary>
    public sealed class CalculatorLexerBuilder : ModularLexerBuilder<CalculatorTokenType>
    {
        private static bool IsValidIdentifier(string str)
        {
            if (!char.IsLetter(str[0]) && str[0] != '_')
                return false;

            for (var i = 1; i < str.Length; i++)
            {
                if (!char.IsLetterOrDigit(str[i]) && str[i] != '_')
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Initializes a new <see cref="CalculatorLexerBuilder" />
        /// </summary>
        /// <param name="language"></param>
        public CalculatorLexerBuilder(CalculatorLanguage language)
            : base(CalculatorTokenType.EndOfExpression)
        {
            // Identifiers
            AddModule(new IdentifierLexerModule());

            // Superscript
            if (language.HasSuperscriptExponentiation())
                AddModule(new SuperscriptLexerModule());

            // Trivia
            AddModule(new WhitespaceLexerModule());

            // Punctuations
            AddLiteral("(", CalculatorTokenType.LParen, "(");
            AddLiteral(")", CalculatorTokenType.RParen, ")");
            AddLiteral(",", CalculatorTokenType.Comma, ",");

            // Numbers
            AddModule(new BinaryNumberLexerModule());
            AddModule(new OctalNumberLexerModule());
            AddModule(new HexadecimalNumberLexerModule());
            AddModule(new DecimalNumberLexerModule());

            var ops = new HashSet<string>(language.UnaryOperators.Values.Select(un => un.Operator), StringComparer.InvariantCulture);
            ops.UnionWith(language.BinaryOperators.Values.Select(bi => bi.Operator));
            ops.RemoveWhere(s => IsValidIdentifier(s));
            foreach (var op in ops)
                AddLiteral(op, CalculatorTokenType.Operator, op);

            Fallback = new UnknownCharacterLexerModule();
        }
    }
}