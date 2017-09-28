using System;
using System.Runtime.Serialization;

namespace Calculator.Core
{
	public class ExpressionException : Exception
	{
		public ExpressionException ( ) : base ( )
		{
		}

		public ExpressionException ( String message ) : base ( message )
		{
		}

		public ExpressionException ( String message, Exception innerException ) : base ( message, innerException )
		{
		}

		protected ExpressionException ( SerializationInfo info, StreamingContext context ) : base ( info, context )
		{
		}
	}
}
