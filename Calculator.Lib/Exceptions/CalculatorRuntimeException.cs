using System;
using System.Collections.Generic;
using System.Text;

namespace Calculator.Lib.Exceptions
{
    public class CalculatorRuntimeException : CalculatorException
    {
        public CalculatorRuntimeException ( String message ) : base ( message )
        {
        }

        public CalculatorRuntimeException ( String message, Exception innerException ) : base ( message, innerException )
        {
        }
    }
}
