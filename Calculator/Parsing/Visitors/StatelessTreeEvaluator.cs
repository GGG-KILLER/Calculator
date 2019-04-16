using System;
using System.Linq;
using Calculator.Errors;
using Calculator.Parsing.Abstractions;
using Calculator.Parsing.AST;

namespace Calculator.Parsing.Visitors
{
    /// <summary>
    /// A stateless <see cref="CalculatorTreeNode" /> tree evaluator
    /// </summary>
    public class StatelessTreeEvaluator : ITreeVisitor<Double>
    {
        private readonly CalculatorLanguage Language;

        /// <summary>
        /// Initializes this <see cref="StatelessTreeEvaluator" />
        /// </summary>
        /// <param name="language"></param>
        public StatelessTreeEvaluator ( CalculatorLanguage language )
        {
            this.Language = language;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="binaryOperator"></param>
        /// <returns></returns>
        public Double Visit ( BinaryOperatorExpression binaryOperator )
        {
            var op = binaryOperator.Operator.Raw;
            if ( !this.Language.HasBinaryOperator ( op ) )
                throw new EvaluationException ( binaryOperator.Operator.Range, $"Unkown operator '{op}'." );

            return this.Language.GetBinaryOperator ( op )
                .Body ( binaryOperator.LeftHandSide.Accept ( this ), binaryOperator.RightHandSide.Accept ( this ) );
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public Double Visit ( IdentifierExpression identifier )
        {
            var ident = identifier.Identifier.Raw;
            if ( !this.Language.HasConstant ( ident ) )
                throw new EvaluationException ( identifier.Identifier.Range, $"Unknown constant {ident}." );

            return this.Language.GetConstant ( ident ).Value;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="functionCall"></param>
        /// <returns></returns>
        public Double Visit ( FunctionCallExpression functionCall )
        {
            var ident = functionCall.Identifier.Identifier.Raw;
            if ( !this.Language.HasFunction ( ident ) )
                throw new EvaluationException (
                    functionCall.Identifier.Identifier.Range.Start.To ( functionCall.Tokens.Last ( ).Range.End ),
                    $"Unkown function '{ident}'." );

            Definitions.Function funcDef = this.Language.GetFunction ( ident );
            var args = functionCall.Arguments.Select( arg => ( Object ) arg.Accept ( this ) ).ToArray ( );
            var idx = args.Length > 4 ? -1 : args.Length;
            if ( funcDef.Overloads.TryGetValue ( idx, out Delegate @delegate ) )
                return ( Double ) @delegate.Method.Invoke ( @delegate.Target, args.Length > 4 ? new Object[] { args } : args );
            else
                throw new EvaluationException (
                    functionCall.Identifier.Identifier.Range.Start.To ( functionCall.Tokens.Last ( ).Range.End ),
                    $"There is no overload of '{ident}' that accepts {args.Length} arguments." );
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public Double Visit ( NumberExpression number ) =>
            ( Double ) number.Value.Value;

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="unaryOperator"></param>
        /// <returns></returns>
        public Double Visit ( UnaryOperatorExpression unaryOperator )
        {
            var op = unaryOperator.Operator.Raw;
            if ( !this.Language.HasUnaryOperator ( op, unaryOperator.OperatorFix ) )
                throw new EvaluationException ( unaryOperator.Operator.Range, $"Unknown operator." );

            return this.Language.GetUnaryOperator ( op, unaryOperator.OperatorFix ).Body ( unaryOperator.Operand.Accept ( this ) );
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="implicitMultiplication"></param>
        /// <returns></returns>
        public Double Visit ( ImplicitMultiplicationExpression implicitMultiplication ) =>
            implicitMultiplication.LeftHandSide.Accept ( this ) * implicitMultiplication.RightHandSide.Accept ( this );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="grouped"></param>
        /// <returns></returns>
        public Double Visit ( GroupedExpression grouped ) =>
            grouped.Inner.Accept ( this );
    }
}
