using System;
using System.Linq;
using Calculator.Errors;
using Calculator.Parsing.Abstractions;
using Calculator.Parsing.AST;

namespace Calculator.Parsing.Visitors
{
    public class TreeEvaluator : ITreeVisitor<Double>
    {
        private readonly CalculatorLanguage Language;

        public TreeEvaluator ( CalculatorLanguage language )
        {
            this.Language = language;
        }

        public Double Visit ( BinaryOperatorExpression binaryOperator )
        {
            if ( !this.Language.HasBinaryOperator ( binaryOperator.Operator.Raw ) )
                throw new EvaluationException ( binaryOperator.Operator.Range.Start, $"Unkown binary operator '{binaryOperator.Operator}'." );

            return this.Language.GetBinaryOperator ( binaryOperator.Operator.Raw )
                .Action ( binaryOperator.LeftHandSide.Accept ( this ), binaryOperator.RightHandSide.Accept ( this ) );
        }

        public Double Visit ( IdentifierExpression identifier )
        {
            var ident = identifier.Identifier.Raw;
            if ( !this.Language.HasConstant ( ident ) )
                throw new EvaluationException ( identifier.Identifier.Range.Start, $"Unknown constant '{ident}'" );

            return this.Language.GetConstant ( ident ).Value;
        }

        public Double Visit ( FunctionCallExpression functionCall )
        {
            var ident = functionCall.Identifier.Raw;
            if ( !this.Language.HasFunction ( ident ) )
                throw new EvaluationException ( functionCall.Identifier.Range.Start, $"Unkown function '{ident}'" );

            Delegate def = this.Language.GetFunction ( ident );
            var args = Array.ConvertAll ( functionCall.Arguments, arg => ( Object ) arg.Accept ( this ) );
            return ( Double ) def.Method.Invoke ( def.Target, args );
        }

        public Double Visit ( NumberExpression number ) => number.Value;

        public Double Visit ( UnaryOperatorExpression unaryOperator )
        {
            var op = unaryOperator.Operator.Raw;
            if ( !this.Language.HasUnaryOperator ( op, unaryOperator.OperatorFix ) )
                throw new EvaluationException ( unaryOperator.Operator.Range.Start, $"Unknown unary operator '{op}'." );

            return this.Language.GetUnaryOperator ( op, unaryOperator.OperatorFix ).Action ( unaryOperator.Operand.Accept ( this ) );
        }
    }
}
