using System;
using System.Collections.Generic;

namespace Calculator.Core.Extensions
{
    internal class ActionSwitch<T>
    {
        private IDictionary<Func<T, Boolean>, Action<T>> Cases;

        public ActionSwitch ( IDictionary<Func<T, Boolean>, Action<T>> Cases )
        {
            this.Cases = Cases;
        }

        public void Test ( T Value )
        {
            foreach ( KeyValuePair<Func<T, Boolean>, Action<T>> @case in this.Cases )
            {
                if ( @case.Key ( Value ) )
                    @case.Value ( Value );
            }
        }
    }

    internal class ActionSwitch
    {
        public static void Test<T> ( IDictionary<Func<T, Boolean>, Action<T>> Cases, T Value )
        {
            new ActionSwitch<T> ( Cases ).Test ( Value );
        }
    }
}
