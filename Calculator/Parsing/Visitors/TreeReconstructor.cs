using System;
using System.Collections.Generic;
using System.Text;
using Calculator.Parsing.Abstractions;
using Calculator.Parsing.AST;

namespace Calculator.Parsing.Visitors
{
    public class TreeReconstructor : ITreeVisitor<String>
    {
        public String Visit ( BinaryOperatorExpression binaryOperator ) =>
            $"({binaryOperator.LeftHandSide.Accept ( this )}) {binaryOperator.Operator.Raw} ({binaryOperator.RightHandSide.Accept ( this )})";

        public String Visit ( IdentifierExpression identifier ) => identifier.Identifier.Raw;

        public String Visit ( FunctionCallExpression functionCall ) =>
            $"{functionCall.Identifier.Raw}({String.Join ( ", ", Array.ConvertAll ( functionCall.Arguments, arg => arg.Accept ( this ) ) )})";

        public String Visit ( NumberExpression number ) => number.Value.ToString ( );

        public String Visit ( UnaryOperatorExpression unaryOperator ) =>
            unaryOperator.OperatorFix == Definitions.UnaryOperatorFix.Prefix
                ? $"{unaryOperator.Operator.Raw}{unaryOperator.Operand.Accept ( this )}"
                : $"{unaryOperator.Operand.Accept ( this )}{unaryOperator.Operator.Raw}";
    }
}
