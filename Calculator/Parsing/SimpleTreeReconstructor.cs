﻿using System;
using System.Linq;
using Calculator.Lexing;

namespace Calculator.Parsing
{
    /// <summary>
    /// A simple <see cref="CalculatorTreeNode" /> tree reconstructor that doesn't take into account operators precedences and stuff (mostly because it doesn't have any information on them)
    /// </summary>
    public class SimpleTreeReconstructor : ITreeVisitor<string>
    {
        /// <inheritdoc />
        public string Visit(BinaryOperatorExpression binaryOperator)
        {
            if (binaryOperator is null)
                throw new ArgumentNullException(nameof(binaryOperator));

            return $"({binaryOperator.LeftHandSide.Accept(this)}) {binaryOperator.Operator.Text} ({binaryOperator.RightHandSide.Accept(this)})";
        }

        /// <inheritdoc />
        public string Visit(IdentifierExpression identifier)
        {
            if (identifier is null)
                throw new ArgumentNullException(nameof(identifier));

            return identifier.Identifier.Text;
        }

        /// <inheritdoc />
        public string Visit(FunctionCallExpression functionCall)
        {
            if (functionCall is null)
                throw new ArgumentNullException(nameof(functionCall));

            return $"{functionCall.Identifier.Accept(this)}({string.Join(", ", functionCall.Arguments.Select(arg => arg.Accept(this)))})";
        }

        /// <inheritdoc />
        public string Visit(NumberExpression number)
        {
            if (number is null)
                throw new ArgumentNullException(nameof(number));

            return number.Value.Value.ToString();
        }

        /// <inheritdoc />
        public string Visit(UnaryOperatorExpression unaryOperator)
        {
            if (unaryOperator is null)
                throw new ArgumentNullException(nameof(unaryOperator));

            return unaryOperator.OperatorFix == Definitions.UnaryOperatorFix.Prefix
                ? $"{unaryOperator.Operator.Text}({unaryOperator.Operand.Accept(this)})"
                : $"({unaryOperator.Operand.Accept(this)}){unaryOperator.Operator.Text}";
        }

        /// <inheritdoc />
        public string Visit(ImplicitMultiplicationExpression implicitMultiplication)
        {
            if (implicitMultiplication is null)
                throw new ArgumentNullException(nameof(implicitMultiplication));

            return $"({implicitMultiplication.LeftHandSide.Accept(this)})({implicitMultiplication.RightHandSide.Accept(this)})";
        }

        /// <inheritdoc />
        public string Visit(GroupedExpression grouped)
        {
            if (grouped is null)
                throw new ArgumentNullException(nameof(grouped));

            return $"({grouped.Inner.Accept(this)})";
        }

        /// <inheritdoc />
        public string Visit(SuperscriptExponentiationExpression superscriptExponentiation)
        {
            if (superscriptExponentiation is null)
                throw new ArgumentNullException(nameof(superscriptExponentiation));

            return $"({superscriptExponentiation.Base.Accept(this)}){SuperscriptChars.TranslateNumber((int) superscriptExponentiation.Exponent.Value)}";
        }
    }
}