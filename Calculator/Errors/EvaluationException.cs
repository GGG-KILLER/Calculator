using System;
using GParse;
using GParse.Errors;

namespace Calculator.Errors
{
    /// <summary>
    /// Exception thrown when evaluating an expression
    /// </summary>
    public class EvaluationException : FatalParsingException
    {
        /// <summary>
        /// Initializes this <see cref="EvaluationException" />
        /// </summary>
        /// <param name="location"></param>
        /// <param name="message"></param>
        public EvaluationException ( SourceLocation location, String message ) : base ( location, message )
        {
        }

        /// <summary>
        /// Initializes this <see cref="EvaluationException" />
        /// </summary>
        /// <param name="range"></param>
        /// <param name="message"></param>
        public EvaluationException ( SourceRange range, String message ) : base ( range, message )
        {
        }

        /// <summary>
        /// Initializes this <see cref="EvaluationException" />
        /// </summary>
        /// <param name="location"></param>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public EvaluationException ( SourceLocation location, String message, Exception innerException ) : base ( location, message, innerException )
        {
        }

        /// <summary>
        /// Initializes this <see cref="EvaluationException" />
        /// </summary>
        /// <param name="range"></param>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public EvaluationException ( SourceRange range, String message, Exception innerException ) : base ( range, message, innerException )
        {
        }
    }
}
