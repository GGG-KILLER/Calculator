using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Calculator.Core.Tokens;

namespace Calculator.Core
{
    internal class Tokenizer
    {
        private readonly StringReader _reader;

        public Tokenizer ( String input )
        {
            this._reader = new StringReader ( input );
        }

        public static IEnumerable<Token> Tokenize ( String Input )
        {
            return new Tokenizer ( Input )
                .Tokenize ( );
        }

        private IEnumerable<Token> Tokenize ( )
        {
            var _tokens = new List<Token> ( );

            while ( this._reader.Peek ( ) != -1 )
            {
                var ch = this._reader.CharPeek ( );

                if ( Char.IsDigit ( ch ) )
                    _tokens.Add ( GetNumber ( ) );
                else if ( Char.IsLetter ( ch ) )
                    _tokens.Add ( GetConstant ( ) );
                else if ( ch == '+' )
                {
                    this._reader.Read ( );
                    _tokens.Add ( new Token.Operator.Plus ( ) );
                }
                else if ( ch == '-' )
                {
                    this._reader.Read ( );
                    _tokens.Add ( new Token.Operator.Minus ( ) );
                }
                else if ( ch == ' ' )
                    this._reader.Read ( );
                else
                    throw new InvalidDataException ( $"The character '{ch}' could not be processed." );
            }

            return _tokens;
        }

        private Token GetConstant ( )
        {
            var constant = new StringBuilder ( );

            Char ch;
            while ( Char.IsLetter ( ( ch = this._reader.CharPeek ( ) ) ) )
                constant.Append ( this._reader.CharRead ( ) );

            return new Token.Constant.MathConstant ( constant.ToString ( ) );
        }

        private Token.Constant.Number GetNumber ( )
        {
            var number = new StringBuilder ( );
            var pastDot = false;

            Char ch;
            while ( ( ( ch = this._reader.CharPeek ( ) ) == '.' ) || Char.IsDigit ( ch ) )
            {
                if ( ch == '.' && pastDot )
                    throw new InvalidDataException ( "Number cannot have 2 dots" );
                else if ( ch == '.' )
                    pastDot = true;

                number.Append ( this._reader.CharRead ( ) );
            }

            var txtNumber = number.ToString ( );
            return new Token.Constant.Number ( Double.Parse ( txtNumber ) );
        }
    }

    internal static class StringReaderExtensions
    {
        public static Char CharRead ( this StringReader reader ) => ( Char ) reader.Read ( );

        public static Char CharPeek ( this StringReader reader ) => ( Char ) reader.Peek ( );
    }
}
