using System;
using System.Linq;
using Calculator.Definitions;
using Calculator.Errors;
using Calculator.Parsing.Abstractions;
using Calculator.Parsing.AST;

namespace Calculator.Parsing.Visitors
{
    /// <summary>
    /// A <see cref="CalculatorTreeNode"/> tree evaluator
    /// </summary>
    public class TreeEvaluator : ITreeVisitor<Double>
    {
        private readonly CalculatorLanguage Language;

        /// <summary>
        /// Initializes this <see cref="TreeEvaluator"/>
        /// </summary>
        /// <param name="language"></param>
        public TreeEvaluator ( CalculatorLanguage language )
        {
            this.Language = language;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="binaryOperator"></param>
        /// <returns></returns>
        public Double Visit ( BinaryOperatorExpression binaryOperator )
        {
            if ( binaryOperator is null )
                throw new ArgumentNullException ( nameof ( binaryOperator ) );

            var op = binaryOperator.Operator.Raw;
            if ( !this.Language.HasBinaryOperator ( op ) )
                throw new EvaluationException ( binaryOperator.Range, $"Unkown operator '{op}'." );

            return this.Language.GetBinaryOperator ( op )
                .Body ( binaryOperator.LeftHandSide.Accept ( this ), binaryOperator.RightHandSide.Accept ( this ) );
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public Double Visit ( IdentifierExpression identifier )
        {
            if ( identifier is null )
                throw new ArgumentNullException ( nameof ( identifier ) );

            var ident = identifier.Identifier.Raw;
            if ( !this.Language.HasConstant ( ident ) )
                throw new EvaluationException ( identifier.Range, $"Unknown constant {ident}." );

            return this.Language.GetConstant ( ident ).Value;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="functionCall"></param>
        /// <returns></returns>
        public Double Visit ( FunctionCallExpression functionCall )
        {
            if ( functionCall is null )
                throw new ArgumentNullException ( nameof ( functionCall ) );

            var ident = functionCall.Identifier.Identifier.Raw;
            if ( !this.Language.HasFunction ( ident ) )
            {
                throw new EvaluationException ( functionCall.Range, $"Unkown function '{ident}'." );
            }

            Function funcDef = this.Language.GetFunction ( ident );
            var args = functionCall.Arguments.Select( arg => ( Object ) arg.Accept ( this ) ).ToArray ( );
            var idx = args.Length > 4 ? -1 : args.Length;
            if ( funcDef.Overloads.TryGetValue ( idx, out Delegate @delegate ) )
            {
                return ( Double ) @delegate.Method.Invoke ( @delegate.Target, args.Length > 4 ? new Object[] { args } : args );
            }
            else
            {
                throw new EvaluationException ( functionCall.Range, $"There is no overload of '{ident}' that accepts {args.Length} arguments." );
            }
        }

        /// <inheritdoc/>
        public Double Visit ( NumberExpression number )
        {
            if ( number is null )
                throw new ArgumentNullException ( nameof ( number ) );

            return ( Double ) number.Value.Value;
        }

        /// <inheritdoc/>
        public Double Visit ( UnaryOperatorExpression unaryOperator )
        {
            if ( unaryOperator is null )
                throw new ArgumentNullException ( nameof ( unaryOperator ) );

            var op = unaryOperator.Operator.Raw;
            if ( !this.Language.HasUnaryOperator ( op, unaryOperator.OperatorFix ) )
                throw new EvaluationException ( unaryOperator.Range, $"Unknown operator." );

            return this.Language.GetUnaryOperator ( op, unaryOperator.OperatorFix ).Body ( unaryOperator.Operand.Accept ( this ) );
        }

        /// <inheritdoc/>
        public Double Visit ( ImplicitMultiplicationExpression implicitMultiplication )
        {
            if ( implicitMultiplication is null )
                throw new ArgumentNullException ( nameof ( implicitMultiplication ) );

            if ( !this.Language.HasImplicitMultiplication ( ) )
            {
                throw new EvaluationException ( implicitMultiplication.Range, "Implicit multiplication hasn't been defined." );
            }

            SpecialBinaryOperator @operator = this.Language.GetImplicitMultiplication ( );
            return @operator.Body ( implicitMultiplication.LeftHandSide.Accept ( this ), implicitMultiplication.RightHandSide.Accept ( this ) );
        }

        /// <inheritdoc/>
        public Double Visit ( GroupedExpression grouped )
        {
            if ( grouped is null )
                throw new ArgumentNullException ( nameof ( grouped ) );

            return grouped.Inner.Accept ( this );
        }

        /// <inheritdoc/>
        public Double Visit ( SuperscriptExponentiationExpression superscriptExponentiation )
        {
            if ( superscriptExponentiation is null )
                throw new ArgumentNullException ( nameof ( superscriptExponentiation ) );

            if ( !this.Language.HasSuperscriptExponentiation ( ) )
            {
                throw new EvaluationException ( superscriptExponentiation.Range, "A superscript exponentiation operation was found but the SuperscriptExponentiation SpecialOperator is not registered!" );
            }

            SpecialBinaryOperator @operator = this.Language.GetSuperscriptExponentiation ( );
            return @operator.Body ( superscriptExponentiation.Base.Accept ( this ), ( Double ) superscriptExponentiation.Exponent.Value );
        }
    }
}