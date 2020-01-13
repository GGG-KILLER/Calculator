using System;
using Calculator.Parsing.Abstractions;
using GParse;

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

        /// <inheritdoc />
        public override SourceRange Range { get; }

        /// <summary>
        /// Initializes this <see cref="ImplicitMultiplicationExpression" />
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public ImplicitMultiplicationExpression ( CalculatorTreeNode left, CalculatorTreeNode right )
        {
            this.LeftHandSide = left ?? throw new ArgumentNullException ( nameof ( left ) );
            this.RightHandSide = right ?? throw new ArgumentNullException ( nameof ( right ) );
            this.Range = this.LeftHandSide.Range.Start.To ( this.RightHandSide.Range.End );
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="visitor"></param>
        public override void Accept ( ITreeVisitor visitor )
        {
            if ( visitor is null )
                throw new ArgumentNullException ( nameof ( visitor ) );

            visitor.Visit ( this );
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="visitor"></param>
        /// <returns></returns>
        public override T Accept<T> ( ITreeVisitor<T> visitor )
        {
            if ( visitor is null )
                throw new ArgumentNullException ( nameof ( visitor ) );

            return visitor.Visit ( this );
        }

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
