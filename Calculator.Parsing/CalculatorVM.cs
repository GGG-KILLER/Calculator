using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Calculator.Common;
using Calculator.Parsing.AST;

namespace Calculator.Parsing
{
    public class CalculatorVM
    {
        private readonly CalculatorLang Language;

        public CalculatorVM ( CalculatorLang language )
        {
            this.Language = language;
        }

        public Double Execute ( String value ) 
        {
            var lexer = new CalculatorLexer ( this.Language, value );
            var parser = new CalculatorParser ( this.Language, lexer );
            return this.Execute ( parser.Parse ( ) );
        }

        public Double Execute ( ASTNode node )
        {
            if ( node is BinaryOperatorExpression binaryOperator )
            {
                return this.Language
                    .GetBinaryOperator ( binaryOperator.Operator )
                    .Action ( this.Execute ( binaryOperator.LeftHandSide ), this.Execute ( binaryOperator.RightHandSide ) );
            }
            else if ( node is UnaryOperatorExpression unaryOperator )
            {
                return this.Language
                    .GetUnaryOperator ( unaryOperator.Operator, unaryOperator.Fix )
                    .Action ( this.Execute ( unaryOperator.Operand ) );
            }
            else if ( node is ParenthesisExpression parenthesis )
            {
                return this.Execute ( parenthesis.InnerExpression );
            }
            else if ( node is FunctionCallExpression functionCall )
            {
                Delegate func = this.Language.GetFunction ( functionCall.Identifier );
                if ( func is Func<Double> f0)
                {
                    if ( functionCall.Arguments.Length != 0 )
                        throw new Exception ( $"{functionCall.Identifier} called with too many arguments." );
                }
                else if ( func is Func<Double, Double> f1 )
                {
                    if ( functionCall.Arguments.Length != 1 )
                        throw new Exception ( $"{functionCall.Identifier} called with too many arguments." );
                    return f1 ( this.Execute ( functionCall.Arguments[0] ) );
                }
                else if ( func is Func<Double, Double, Double> f2 )
                {
                    if ( functionCall.Arguments.Length != 2 )
                        throw new Exception ( $"{functionCall.Identifier} called with too many arguments." );
                    return f2 ( this.Execute ( functionCall.Arguments[0] ), this.Execute ( functionCall.Arguments[1] ) );
                }
                else
                {
                    return ( func as Func<Double[], Double> ) ( functionCall.Arguments.Select ( this.Execute ).ToArray ( ) );
                }
            }
            else if ( node is NumberExpression number )
                return number.Value;
            else if ( node is ConstantExpression constant )
                return this.Language.GetConstant ( constant.Identifier ).Value;
            else
                throw new Exception ( "Unknown ASTNode type." );
        }
    }
}
