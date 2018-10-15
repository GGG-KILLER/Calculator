using System;
using GParse.Common;
using GParse.Common.IO;
using GParse.Common.Lexing;
using GParse.Parsing.Abstractions.Lexing;

namespace Calculator.Lexing.Modules
{
    public class IdentifierLexerModule : ILexerModule<CalculatorTokenType>
    {
        public String Name => "Identifier Lexer Module";
        public String Prefix => null;

        public Boolean CanConsumeNext ( SourceCodeReader reader ) =>
            reader.HasContent && Char.IsLetter ( ( Char ) reader.Peek ( ) );

        public Token<CalculatorTokenType> ConsumeNext ( SourceCodeReader reader )
        {
            SourceLocation start = reader.Location;
            var ident = reader.ReadStringWhile ( Char.IsLetterOrDigit );
            return new Token<CalculatorTokenType> ( ident, ident, ident, CalculatorTokenType.Identifier, start.To ( reader.Location ) );
        }
    }
}
