using System;

namespace Calculator.Lib.Exceptions
{
    public class CalculatorException : Exception
    {
        public CalculatorException ( String message ) : base ( message )
        {
        }

        public CalculatorException ( String message, Exception innerException ) : base ( message, innerException )
        {
        }
    }
}