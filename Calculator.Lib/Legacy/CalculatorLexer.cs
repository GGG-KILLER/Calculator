﻿using System;
using System.Text;
using Calculator.Lib.Definitions;
using GParse.Common;
using GParse.Common.Errors;
using GParse.Parsing;
using GParse.Parsing.Settings;

namespace Calculator.Lib
{
    public class CalculatorLexer : LexerBase
    {
        private readonly CalculatorLang Language;

        public CalculatorLexer ( CalculatorLang lang, String input ) : base ( input )
        {
            this.Language = lang;

            // Add operators and constants of the language (using
            // a hack to ignore already-added operators)
            foreach ( UnaryOperatorDef unaryop in lang.UnaryOperators )
                try { this.tokenManager.AddToken ( unaryop.Operator, unaryop.Operator, TokenType.Operator ); }
                catch { }

            foreach ( BinaryOperatorDef binaryop in lang.BinaryOperators )
                try { this.tokenManager.AddToken ( binaryop.Operator, binaryop.Operator, TokenType.Operator ); }
                catch { }

            foreach ( ConstantDef constdef in lang.Constants )
                try { this.tokenManager.AddToken ( constdef.Identifier, constdef.Identifier, TokenType.Identifier, next => !Char.IsLetterOrDigit ( next ) ); }
                catch { }

            // Parenthesis and commas aren't operators
            this.tokenManager
                .AddToken ( "(", "(", TokenType.Punctuation )
                .AddToken ( ")", ")", TokenType.Punctuation )
                .AddToken ( ",", ",", TokenType.Punctuation );
            // Integer lexing settings
            this.numberSettings = new IntegerLexSettings
            {
                BinaryPrefix = "0b",
                HexadecimalPrefix = "0x",
                OctalPrefix = "0o"
            };
            // Other settings
            this.storeWhitespaces = false;
        }

        protected override Boolean TryReadToken ( out Token tok )
        {
            SourceLocation start = this.Location;
            if ( Char.IsDigit ( ( Char ) this.reader.Peek ( ) ) )
            {
                if ( this.TryReadInteger ( out var Raw, out var Value, this.numberSettings ) )
                {
                    tok = this.CreateToken ( "number", Raw, ( Double ) Value, TokenType.Number, start.To ( this.Location ) );
                }
                else
                {
                    (var raw, var value) = this.ReadFloatingPointNumber ( );
                    tok = this.CreateToken ( "number", raw, value, TokenType.Number, start.To ( this.Location ) );
                }
            }
            else if ( Char.IsLetter ( ( Char ) this.reader.Peek ( ) ) )
            {
                var id = this.reader.ReadStringWhile ( Char.IsLetterOrDigit );
                tok = this.CreateToken ( "identifier", id, id, TokenType.Identifier, start.To ( this.Location ) );
            }
            else
            {
                tok = null;
                return false;
            }
            return true;
        }

        private (String Raw, Double Value) ReadFloatingPointNumber ( )
        {
            var rawNumber = new StringBuilder ( );
            String integer = this.reader.ReadStringWhile ( Char.IsDigit ), fractional = null, exp = null;
            rawNumber.Append ( integer );

            if ( this.reader.IsNext ( "." ) )
            {
                this.reader.Advance ( 1 );
                rawNumber.Append ( '.' );

                fractional = this.reader.ReadStringWhile ( Char.IsDigit );
                rawNumber.Append ( fractional );
            }

            if ( this.reader.IsNext ( "e" ) || this.reader.IsNext ( "E" ) )
            {
                rawNumber.Append ( this.reader.ReadString ( 1 ) );
                if ( this.reader.IsNext ( "+" ) || this.reader.IsNext ( "-" ) )
                    rawNumber.Append ( this.reader.ReadString ( 1 ) );

                exp = this.reader.ReadStringWhile ( Char.IsDigit );
                rawNumber.Append ( exp );

                if ( String.IsNullOrEmpty ( exp ) )
                    throw new LexException ( "Malformed number literal.", this.Location );
            }

            if ( String.IsNullOrEmpty ( integer ) && String.IsNullOrEmpty ( fractional ) )
                throw new LexException ( "Malformed number literal.", this.Location );

            return (
                Raw: rawNumber.ToString ( ),
                Value: Double.Parse ( rawNumber.ToString ( ) )
            );
        }
    }
}