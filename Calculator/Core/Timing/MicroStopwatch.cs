using System;
using System.Diagnostics;

namespace Calculator.Core.Timing
{
	public class PrecisionStopwatch : Stopwatch
	{
		public const Double TicksPerMicrosecond = TimeSpan.TicksPerMillisecond / 1000D;
		public const Double TicksPerNanosecond = TicksPerMicrosecond / 1000D;

		public Double ElapsedMicroseconds => this.ElapsedTicks / TicksPerMicrosecond;

		public Double ElapsedNanoseconds => this.ElapsedTicks / TicksPerNanosecond;
	}
}
