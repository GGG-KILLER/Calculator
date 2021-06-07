using System;
using Calculator.Parsing.Abstractions;
using GParse.Math;

namespace Calculator.Parsing.AST
{
    /// <summary>
    /// The base class fora all AST nodes
    /// </summary>
    public abstract class CalculatorTreeNode
    {
        /// <summary>
        /// The range that forms the expression this node represents
        /// </summary>
        public abstract Range<int> Range { get; }

        /// <summary>
        /// Accepts a visitor that returns nothing
        /// </summary>
        /// <param name="visitor"></param>
        public abstract void Accept(ITreeVisitor visitor);

        /// <summary>
        /// Accepts a visitor that returns <typeparamref name="T" />
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="visitor"></param>
        /// <returns></returns>
        public abstract T Accept<T>(ITreeVisitor<T> visitor);

        /// <summary>
        /// Checks whether this node structurally matches another
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public abstract bool StructurallyEquals(CalculatorTreeNode node);
    }
}
