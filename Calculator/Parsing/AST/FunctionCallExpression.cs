using System;
using System.Collections.Generic;
using System.Linq;
using Calculator.Lexing;
using Calculator.Parsing.Abstractions;
using GParse.Common.AST;
using GParse.Common.Lexing;

namespace Calculator.Parsing.AST
{
    public class FunctionCallExpression : CalculatorASTNode
    {
        private readonly Token<CalculatorTokenType>[] _tokens;
        public Token<CalculatorTokenType> Identifier => this._tokens[0];
        public readonly CalculatorASTNode[] Arguments;

        public FunctionCallExpression ( IEnumerable<Token<CalculatorTokenType>> tokens, CalculatorASTNode[] args )
        {
            this._tokens = tokens.ToArray ( );
            this.Arguments = args;
        }

        public override IEnumerable<Token<CalculatorTokenType>> Tokens
        {
            get
            {
                var i = 0;
                yield return this._tokens[i++];
                yield return this._tokens[i++];
                foreach ( CalculatorASTNode arg in this.Arguments )
                {
                    foreach ( Token<CalculatorTokenType> token in arg.Tokens )
                        yield return token;
                    yield return this._tokens[i++];
                }
                yield return this._tokens[i];
            }
        }

        public override IEnumerable<CalculatorASTNode> Children => this.Arguments;

        public override void Accept ( ITreeVisitor visitor ) => visitor.Visit ( this );

        public override T Accept<T> ( ITreeVisitor<T> visitor ) => visitor.Visit ( this );

        public override Boolean StructurallyEquals ( CalculatorASTNode node ) =>
            node is FunctionCallExpression functionCallExpression
                && this.Identifier.Raw.Equals ( functionCallExpression.Identifier.Raw )
                && this.Arguments.Zip ( functionCallExpression.Arguments, ( a, b ) => a.StructurallyEquals ( b ) ).All ( a => a );
    }
}
