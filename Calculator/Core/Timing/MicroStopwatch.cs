using System;
using System.Diagnostics;

namespace Calculator.Core.Timing
{
	public class PrecisionStopwatch : Stopwatch
	{
		public const Int64 TicksPerMicrosecond = TimeSpan.TicksPerMillisecond / 1000;
		public Double ElapsedMicroseconds => ( Double ) this.ElapsedTicks / TicksPerMicrosecond;
	}
}
