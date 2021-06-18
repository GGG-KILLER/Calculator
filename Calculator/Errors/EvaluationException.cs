using System;
using System.Runtime.Serialization;
using GParse.Errors;
using GParse.Math;

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
        public EvaluationException(int location, string message) : base(location, message)
        {
        }

        /// <summary>
        /// Initializes this <see cref="EvaluationException" />
        /// </summary>
        /// <param name="range"></param>
        /// <param name="message"></param>
        public EvaluationException(Range<int> range, string message) : base(range, message)
        {
        }

        /// <summary>
        /// Initializes this <see cref="EvaluationException" />
        /// </summary>
        /// <param name="location"></param>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public EvaluationException(int location, string message, Exception innerException) : base(location, message, innerException)
        {
        }

        /// <summary>
        /// Initializes this <see cref="EvaluationException" />
        /// </summary>
        /// <param name="range"></param>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public EvaluationException(Range<int> range, string message, Exception innerException)
            : base(range, message, innerException)
        {
        }

        /// <summary>
        /// Initializes this <see cref="EvaluationException" />
        /// </summary>
        /// <param name="serializationInfo"></param>
        /// <param name="streamingContext"></param>
        protected EvaluationException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
        {
        }
    }
}