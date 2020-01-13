using System;
using Calculator.Definitions;
using Calculator.Lexing;
using Calculator.Parsing.Abstractions;
using GParse;
using GParse.Lexing;

namespace Calculator.Parsing.AST
{
    /// <summary>
    /// Represents a prefix/postfix operation
    /// </summary>
    public class UnaryOperatorExpression : CalculatorTreeNode
    {
        /// <summary>
        /// The operator's location
        /// </summary>
        public UnaryOperatorFix OperatorFix { get; }

        /// <summary>
        /// The operator itself
        /// </summary>
        public Token<CalculatorTokenType> Operator { get; }

        /// <summary>
        /// The operand expression
        /// </summary>
        public CalculatorTreeNode Operand { get; }

        /// <inheritdoc />
        public override SourceRange Range { get; }

        /// <summary>
        /// Initializes this <see cref="UnaryOperatorExpression" />
        /// </summary>
        /// <param name="fix"></param>
        /// <param name="operator"></param>
        /// <param name="operand"></param>
        public UnaryOperatorExpression ( UnaryOperatorFix fix, Token<CalculatorTokenType> @operator, CalculatorTreeNode operand )
        {
            this.OperatorFix = fix;
            this.Operator = @operator;
            this.Operand = operand ?? throw new ArgumentNullException ( nameof ( operand ) );

            if ( fix == UnaryOperatorFix.Postfix )
                this.Range = operand.Range.Start.To ( @operator.Range.End );
            else
                this.Range = @operator.Range.Start.To ( operand.Range.End );
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
            node is UnaryOperatorExpression unaryOperatorExpression
                && this.Operator.Raw.Equals ( unaryOperatorExpression.Operator.Raw, StringComparison.OrdinalIgnoreCase )
                && this.OperatorFix.Equals ( unaryOperatorExpression.OperatorFix )
                && this.Operand.StructurallyEquals ( unaryOperatorExpression.Operand );
    }
}
