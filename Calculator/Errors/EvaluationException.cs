using System;
using GParse.Common;
using GParse.Common.Errors;

namespace Calculator.Errors
{
    public class EvaluationException : LocationBasedException
    {
        public EvaluationException ( SourceLocation location, String message ) : base ( location, message )
        {
        }

        public EvaluationException ( SourceLocation location, String message, Exception innerException ) : base ( location, message, innerException )
        {
        }
    }
}
