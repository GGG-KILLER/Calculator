using System;
using Calculator.Lexing;
using Calculator.Parsing.Abstractions;
using GParse;
using GParse.Lexing;

namespace Calculator.Parsing.AST
{
    /// <summary>
    /// Represents an infix expression
    /// </summary>
    public class BinaryOperatorExpression : CalculatorTreeNode
    {
        /// <summary>
        /// The expression to the left of the operator
        /// </summary>
        public CalculatorTreeNode LeftHandSide { get; }

        /// <summary>
        /// The expression to the right of the operator
        /// </summary>
        public CalculatorTreeNode RightHandSide { get; }

        /// <summary>
        /// The operator itself
        /// </summary>
        public Token<CalculatorTokenType> Operator { get; }

        /// <inheritdoc />
        public override SourceRange Range { get; }

        /// <summary>
        /// Initializes this <see cref="BinaryOperatorExpression" />
        /// </summary>
        /// <param name="operator"></param>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        public BinaryOperatorExpression ( Token<CalculatorTokenType> @operator, CalculatorTreeNode lhs, CalculatorTreeNode rhs )
        {
            this.Operator = @operator;
            this.LeftHandSide = lhs ?? throw new ArgumentNullException ( nameof ( lhs ) );
            this.RightHandSide = rhs ?? throw new ArgumentNullException ( nameof ( rhs ) );
            this.Range = lhs.Range.Start.To ( rhs.Range.End );
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
            node is BinaryOperatorExpression binaryOperatorExpression
                && this.Operator.Raw.Equals ( binaryOperatorExpression.Operator.Raw, StringComparison.OrdinalIgnoreCase )
                && this.LeftHandSide.StructurallyEquals ( binaryOperatorExpression.LeftHandSide )
                && this.RightHandSide.StructurallyEquals ( binaryOperatorExpression.RightHandSide );
    }
}
