using System;
using System.Linq;
using Calculator.Errors;

namespace Calculator.Parsing
{
    /// <summary>
    /// A <see cref="CalculatorTreeNode"/> tree evaluator
    /// </summary>
    public class TreeEvaluator : ITreeVisitor<double>
    {
        private readonly CalculatorLanguage _language;

        /// <summary>
        /// Initializes this <see cref="TreeEvaluator"/>
        /// </summary>
        /// <param name="language"></param>
        public TreeEvaluator(CalculatorLanguage language) =>
            _language = language;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="binaryOperator"></param>
        /// <returns></returns>
        public double Visit(BinaryOperatorExpression binaryOperator)
        {
            if (binaryOperator is null)
                throw new ArgumentNullException(nameof(binaryOperator));

            var op = binaryOperator.Operator.Text;
            if (!_language.HasBinaryOperator(op))
                throw new EvaluationException(binaryOperator.Range, $"Unkown operator '{op}'.");

            return _language.GetBinaryOperator(op)
                .Body(binaryOperator.LeftHandSide.Accept(this), binaryOperator.RightHandSide.Accept(this));
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public double Visit(IdentifierExpression identifier)
        {
            if (identifier is null)
                throw new ArgumentNullException(nameof(identifier));

            var ident = identifier.Identifier.Text;
            if (!_language.HasConstant(ident))
                throw new EvaluationException(identifier.Range, $"Unknown constant {ident}.");

            return _language.GetConstant(ident).Value;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="functionCall"></param>
        /// <returns></returns>
        public double Visit(FunctionCallExpression functionCall)
        {
            if (functionCall is null)
                throw new ArgumentNullException(nameof(functionCall));

            var ident = functionCall.Identifier.Identifier.Text;
            if (!_language.HasFunction(ident))
            {
                throw new EvaluationException(functionCall.Range, $"Unkown function '{ident}'.");
            }

            var funcDef = _language.GetFunction(ident);
            var args = functionCall.Arguments.Select(arg => (object) arg.Accept(this)).ToArray();
            var idx = args.Length > 4 ? -1 : args.Length;
            if (funcDef.Overloads.TryGetValue(idx, out var @delegate))
            {
                return (double) @delegate.Method.Invoke(@delegate.Target, args.Length > 4 ? new object[] { args } : args);
            }
            else
            {
                throw new EvaluationException(functionCall.Range, $"There is no overload of '{ident}' that accepts {args.Length} arguments.");
            }
        }

        /// <inheritdoc/>
        public double Visit(NumberExpression number)
        {
            if (number is null)
                throw new ArgumentNullException(nameof(number));

            return (double) number.Value.Value;
        }

        /// <inheritdoc/>
        public double Visit(UnaryOperatorExpression unaryOperator)
        {
            if (unaryOperator is null)
                throw new ArgumentNullException(nameof(unaryOperator));

            var op = unaryOperator.Operator.Text;
            if (!_language.HasUnaryOperator(op, unaryOperator.OperatorFix))
                throw new EvaluationException(unaryOperator.Range, $"Unknown operator.");

            return _language.GetUnaryOperator(op, unaryOperator.OperatorFix).Body(unaryOperator.Operand.Accept(this));
        }

        /// <inheritdoc/>
        public double Visit(ImplicitMultiplicationExpression implicitMultiplication)
        {
            if (implicitMultiplication is null)
                throw new ArgumentNullException(nameof(implicitMultiplication));

            if (!_language.HasImplicitMultiplication())
            {
                throw new EvaluationException(implicitMultiplication.Range, "Implicit multiplication hasn't been defined.");
            }

            var @operator = _language.GetImplicitMultiplication();
            return @operator.Body(implicitMultiplication.LeftHandSide.Accept(this), implicitMultiplication.RightHandSide.Accept(this));
        }

        /// <inheritdoc/>
        public double Visit(GroupedExpression grouped)
        {
            if (grouped is null)
                throw new ArgumentNullException(nameof(grouped));

            return grouped.Inner.Accept(this);
        }

        /// <inheritdoc/>
        public double Visit(SuperscriptExponentiationExpression superscriptExponentiation)
        {
            if (superscriptExponentiation is null)
                throw new ArgumentNullException(nameof(superscriptExponentiation));

            if (!_language.HasSuperscriptExponentiation())
            {
                throw new EvaluationException(superscriptExponentiation.Range, "A superscript exponentiation operation was found but the SuperscriptExponentiation SpecialOperator is not registered!");
            }

            var @operator = _language.GetSuperscriptExponentiation();
            return @operator.Body(superscriptExponentiation.Base.Accept(this), (int) superscriptExponentiation.Exponent.Value);
        }
    }
}