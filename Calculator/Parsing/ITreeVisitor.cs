using Calculator.Parsing;

namespace Calculator.Parsing
{
    /// <summary>
    /// The interface that a returnless tree visitor is expected to provide
    /// </summary>
    public interface ITreeVisitor
    {
        /// <summary>
        /// The <see cref="BinaryOperatorExpression" /> visitor
        /// </summary>
        /// <param name="binaryOperator"></param>
        void Visit(BinaryOperatorExpression binaryOperator);

        /// <summary>
        /// The <see cref="IdentifierExpression" /> visitor
        /// </summary>
        /// <param name="identifier"></param>
        void Visit(IdentifierExpression identifier);

        /// <summary>
        /// The <see cref="FunctionCallExpression" /> visitor
        /// </summary>
        /// <param name="functionCall"></param>
        void Visit(FunctionCallExpression functionCall);

        /// <summary>
        /// The <see cref="NumberExpression" /> visitor
        /// </summary>
        /// <param name="number"></param>
        void Visit(NumberExpression number);

        /// <summary>
        /// The <see cref="UnaryOperatorExpression" /> visitor
        /// </summary>
        /// <param name="unaryOperator"></param>
        void Visit(UnaryOperatorExpression unaryOperator);

        /// <summary>
        /// The <see cref="ImplicitMultiplicationExpression" /> visitor
        /// </summary>
        /// <param name="implicitMultiplication"></param>
        void Visit(ImplicitMultiplicationExpression implicitMultiplication);

        /// <summary>
        /// The <see cref="SuperscriptExponentiationExpression" /> visitor
        /// </summary>
        /// <param name="superscriptExponentiation"></param>
        void Visit(SuperscriptExponentiationExpression superscriptExponentiation);

        /// <summary>
        /// The <see cref="GroupedExpression" /> visitor
        /// </summary>
        /// <param name="grouped"></param>
        void Visit(GroupedExpression grouped);
    }

    /// <summary>
    /// The interface that a visitor with a return value is expected to provide
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITreeVisitor<T>
    {
        /// <summary>
        /// The <see cref="BinaryOperatorExpression" /> visitor
        /// </summary>
        /// <param name="binaryOperator"></param>
        /// <returns></returns>
        T Visit(BinaryOperatorExpression binaryOperator);

        /// <summary>
        /// The <see cref="IdentifierExpression" /> visitor
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        T Visit(IdentifierExpression identifier);

        /// <summary>
        /// The <see cref="FunctionCallExpression" /> visitor
        /// </summary>
        /// <param name="functionCall"></param>
        /// <returns></returns>
        T Visit(FunctionCallExpression functionCall);

        /// <summary>
        /// The <see cref="NumberExpression" /> visitor
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        T Visit(NumberExpression number);

        /// <summary>
        /// The <see cref="UnaryOperatorExpression" /> visitor
        /// </summary>
        /// <param name="unaryOperator"></param>
        /// <returns></returns>
        T Visit(UnaryOperatorExpression unaryOperator);

        /// <summary>
        /// The <see cref="ImplicitMultiplicationExpression" /> visitor
        /// </summary>
        /// <param name="implicitMultiplication"></param>
        /// <returns></returns>
        T Visit(ImplicitMultiplicationExpression implicitMultiplication);

        /// <summary>
        /// The <see cref="SuperscriptExponentiationExpression" /> visitor
        /// </summary>
        /// <param name="superscriptExponentiation"></param>
        T Visit(SuperscriptExponentiationExpression superscriptExponentiation);

        /// <summary>
        /// The <see cref="GroupedExpression" /> visitor
        /// </summary>
        /// <param name="grouped"></param>
        /// <returns></returns>
        T Visit(GroupedExpression grouped);
    }
}