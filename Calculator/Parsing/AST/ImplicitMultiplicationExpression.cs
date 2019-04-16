using System;
using Calculator.Parsing.Abstractions;

namespace Calculator.Parsing.AST
{
    /// <summary>
    /// Represents an implicit multiplication operation
    /// </summary>
    public class ImplicitMultiplicationExpression : CalculatorTreeNode
    {
        /// <summary>
        /// The expression on the left
        /// </summary>
        public CalculatorTreeNode LeftHandSide { get; }

        /// <summary>
        /// The expression on the right
        /// </summary>
        public CalculatorTreeNode RightHandSide { get; }

        /// <summary>
        /// Initializes this <see cref="ImplicitMultiplicationExpression" />
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public ImplicitMultiplicationExpression ( CalculatorTreeNode left, CalculatorTreeNode right )
        {
            this.LeftHandSide = left;
            this.RightHandSide = right;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="visitor"></param>
        public override void Accept ( ITreeVisitor visitor ) =>
            visitor.Visit ( this );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="visitor"></param>
        /// <returns></returns>
        public override T Accept<T> ( ITreeVisitor<T> visitor ) =>
            visitor.Visit ( this );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public override Boolean StructurallyEquals ( CalculatorTreeNode node ) =>
            node is ImplicitMultiplicationExpression implicitMultiplication
            && this.LeftHandSide.StructurallyEquals ( implicitMultiplication.LeftHandSide )
            && this.RightHandSide.StructurallyEquals ( implicitMultiplication.RightHandSide );
    }
}
