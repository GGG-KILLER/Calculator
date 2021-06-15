using System;
using GParse.Math;

namespace Calculator.Errors
{
    /// <summary>
    /// Exception thrown when evaluating an expression
    /// </summary>
    public class EvaluationException : ApplicationException
    {
        /// <summary>
        /// The range where the exception ocurred.
        /// </summary>
        public Range<int> Range { get; }

        /// <summary>
        /// Initializes this <see cref="EvaluationException" />
        /// </summary>
        /// <param name="range"></param>
        /// <param name="message"></param>
        public EvaluationException(Range<int> range, string message)
            : base(message) =>
            Range = range;

        /// <summary>
        /// Initializes this <see cref="EvaluationException" />
        /// </summary>
        /// <param name="range"></param>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public EvaluationException(Range<int> range, string message, Exception innerException)
            : base(message, innerException) =>
            Range = range;
    }
}