using System;
using System.Collections.Generic;
using Calculator.Core.Tokens;

namespace Calculator.Core
{
    internal class Lexer
    {
        private readonly List<Token> Tokens;

        public Lexer ( IEnumerable<Token> tokens )
        {
            this.Tokens = new List<Token> ( tokens );
        }

        public void SubSolve ( )
        {
            Expect<Token.Constant, Token.Operator, Token.Constant> ( );

            // The sum operation
            if ( IsNext<Token.Constant, Token.Operator.Plus> ( ) )
                this.Tokens.Insert ( 0, new Token.Constant.Number (
                    Read<Token.Constant> ( ).Value +
                    Read<Token.Constant> ( 1 ).Value ) );
            // The subtraction operation
            else if ( IsNext<Token.Constant, Token.Operator.Minus> ( ) )
                this.Tokens.Insert ( 0, new Token.Constant.Number (
                    Read<Token.Constant> ( ).Value -
                    Read<Token.Constant> ( 1 ).Value ) );
        }

        public IEnumerable<IEnumerable<Token>> ProgressivelySolve ( )
        {
            do
            {
                SubSolve ( );
                yield return this.Tokens;
            }
            while ( this.Tokens.Count > 1 );
        }

        public Double Solve ( )
        {
            do SubSolve ( );
            while ( this.Tokens.Count > 1 );

            var lasttoken = this.Tokens[0];
            if ( !( lasttoken is Token.Constant.Number ) )
                throw new Exception ( "Unexpected final result: " + lasttoken );

            return ( lasttoken as Token.Constant.Number ).Value;
        }

        private Token Peek ( Int32 skip = 0 )
        {
            return this.Tokens[skip];
        }

        private Token Read ( Int32 skip = 0 )
        {
            // Get the value to be returned
            var ret = this.Tokens[skip];

            // Remove the previous tokens
            this.Tokens.RemoveRange ( 0, skip + 1 );

            return ret;
        }

        private T1 Read<T1> ( Int32 skip = 0 ) where T1 : Token
        {
            return this.Read ( skip ) as T1;
        }

        private Boolean IsNext<T1> ( ) => this.Peek ( ) is T1;

        private Boolean IsNext<T1, T2> ( ) => this.Peek ( ) is T1 && this.Peek ( 1 ) is T2;

        private Boolean IsNext<T1, T2, T3> ( ) => this.Peek ( ) is T1 && this.Peek ( 1 ) is T2
            && this.Peek ( 2 ) is T3;

        private void Expect<T1> ( )
        {
            if ( !IsNext<T1> ( ) )
                throw new Exception ( $"Expected {typeof ( T1 ).Name} but got {Peek ( ).GetType ( ).Name}." );
        }

        private void Expect<T1, T2> ( )
        {
            if ( !IsNext<T1, T2> ( ) )
                throw new Exception ( $"Expected {typeof ( T1 ).Name} and {typeof ( T2 ).Name} but got {Peek ( ).GetType ( ).Name} and {Peek ( 1 ).GetType ( ).Name}." );
        }

        private void Expect<T1, T2, T3> ( )
        {
            if ( !IsNext<T1, T2, T3> ( ) )
                throw new Exception ( $"Expected {typeof ( T1 ).Name}, {typeof ( T2 ).Name} and {typeof ( T3 ).Name} but received {Peek ( ).GetType ( ).Name}, {Peek ( 1 ).GetType ( ).Name} and {Peek ( 2 ).GetType ( ).Name}" );
        }
    }
}
