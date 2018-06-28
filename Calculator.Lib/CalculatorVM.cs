using System;
using Calculator.Lib.AST;
using Calculator.Lib.Definitions;
using GParse.Common;

namespace Calculator.Lib
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
                var args = new Object[functionCall.Arguments.Length];
                for ( var i = 0; i < functionCall.Arguments.Length; i++ )
                    args[i] = this.Execute ( functionCall.Arguments[i] );

                return ( Double ) func.DynamicInvoke ( args );
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
