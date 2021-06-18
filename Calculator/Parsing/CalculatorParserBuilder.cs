using System.Linq;
using Calculator.Definitions;
using Calculator.Lexing;
using Calculator.Parsing.Parselets;
using GParse;
using GParse.Lexing;
using GParse.Parsing;

namespace Calculator.Parsing
{
    /// <summary>
    /// A <see cref="CalculatorParser"/> builder
    /// </summary>
    public class CalculatorParserBuilder : PrattParserBuilder<CalculatorTokenType, CalculatorTreeNode>
    {
        private static bool IsValidIdentifier(in string str) =>
            char.IsLetter(str[0]) && str.Skip(1).All(char.IsLetterOrDigit);

        /// <summary>
        /// Initializes this <see cref="CalculatorParserBuilder"/> with a <see cref="CalculatorLanguage"/>
        /// </summary>
        /// <param name="language"></param>
        public CalculatorParserBuilder(CalculatorLanguage language)
        {
            RegisterLiteral(
                CalculatorTokenType.Identifier,
                static (Token<CalculatorTokenType> tok, DiagnosticList diagnostics) => new IdentifierExpression(tok));

            RegisterLiteral(
                CalculatorTokenType.Number,
                static (Token<CalculatorTokenType> tok, DiagnosticList diagnostics) => new NumberExpression(tok));

            var maxPrecedence = 0;
            foreach (var unaryOp in language.UnaryOperators.Values)
            {
                if (unaryOp.Precedence > maxPrecedence)
                    maxPrecedence = unaryOp.Precedence;

                if (unaryOp.Fix == UnaryOperatorFix.Postfix)
                {
                    RegisterSingleTokenPostfixOperator(
                        IsValidIdentifier(unaryOp.Operator) ? CalculatorTokenType.Identifier : CalculatorTokenType.Operator,
                        unaryOp.Operator,
                        unaryOp.Precedence,
                        static (CalculatorTreeNode expr, Token<CalculatorTokenType> unop, DiagnosticList diagnostics) =>
                            new UnaryOperatorExpression(UnaryOperatorFix.Postfix, unop, expr));
                }
                else
                {
                    RegisterSingleTokenPrefixOperator(
                        IsValidIdentifier(unaryOp.Operator) ? CalculatorTokenType.Identifier : CalculatorTokenType.Operator,
                        unaryOp.Operator,
                        unaryOp.Precedence,
                        static (Token<CalculatorTokenType> unop, CalculatorTreeNode expr, DiagnosticList diagnostics) =>
                            new UnaryOperatorExpression(UnaryOperatorFix.Prefix, unop, expr));
                }
            }

            foreach (var binaryOp in language.BinaryOperators.Values)
            {
                if (binaryOp.Precedence > maxPrecedence)
                    maxPrecedence = binaryOp.Precedence;

                RegisterSingleTokenInfixOperator(
                    IsValidIdentifier(binaryOp.Operator) ? CalculatorTokenType.Identifier : CalculatorTokenType.Operator,
                    binaryOp.Operator,
                    binaryOp.Precedence,
                    binaryOp.Associativity == Associativity.Right,
                    static (CalculatorTreeNode left, Token<CalculatorTokenType> op, CalculatorTreeNode right) =>
                        new BinaryOperatorExpression(op, left, right));
            }

            var hasImplMul = language.HasImplicitMultiplication();
            if (hasImplMul)
            {
                var op = language.GetImplicitMultiplication();
                if (op.Precedence > maxPrecedence)
                    maxPrecedence = op.Precedence;
            }

            if (language.HasSuperscriptExponentiation())
            {
                var op = language.GetSuperscriptExponentiation();
                if (op.Precedence > maxPrecedence)
                    maxPrecedence = op.Precedence;

                Register(CalculatorTokenType.Superscript, new SuperscriptExponentiationExpressionParselet(op.Precedence));
            }

            Register(CalculatorTokenType.LParen, new GroupedExpressionParselet());
            Register(CalculatorTokenType.LParen, new FunctionCallExpressionParselet(maxPrecedence + 1));
            if (hasImplMul)
            {
                var op = language.GetImplicitMultiplication();
                var implicitMultiplicationParselet = new ImplicitMultiplicationExpressionParselet(op.Precedence);
                foreach (var tokenType in new[] { CalculatorTokenType.Identifier, CalculatorTokenType.LParen, CalculatorTokenType.Number })
                    Register(tokenType, implicitMultiplicationParselet);
            }
        }

        /// <summary>
        /// Creates a <see cref="CalculatorParser"/>
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="diagnostics"></param>
        /// <returns></returns>
        public override IPrattParser<CalculatorTokenType, CalculatorTreeNode> CreateParser(
            ITokenReader<CalculatorTokenType> reader,
            DiagnosticList diagnostics) =>
            new CalculatorParser(reader, PrefixModuleTree, InfixModuleTree, diagnostics);
    }
}