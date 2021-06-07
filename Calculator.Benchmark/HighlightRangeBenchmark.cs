using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using GParse;

namespace Calculator.Benchmark
{
    [Config(typeof(Config))]
    public class HighlightRangeBenchmark
    {
        private class Config : ManualConfig
        {
            public Config()
            {
                var @base = Job.Default.WithEvaluateOverhead(true);
                AddJob(@base.WithRuntime(CoreRuntime.Core31));
                AddJob(@base.WithRuntime(CoreRuntime.Core50));
                AddJob(@base.WithRuntime(CoreRuntime.Core60));
            }
        }

        private static SourceRange Range(int startLine, int startColumn, int startByte, int endLine, int endColumn, int endByte) =>
            new SourceLocation(startLine, startColumn, startByte).To(new SourceLocation(endLine, endColumn, endByte));

        public IEnumerable<(string, SourceRange)> Pairs => new[]
        {
            (@"one line", Range(1, 3, 3, 1, 4, 4)),
            (@"two
line", Range(1, 3, 3, 2, 0, 4)),
            (@"three
line
code", Range(1, 5, 5, 2, 4, 10)),
            (@"long three
line code
over here", Range(1, 6, 6, 3, 4, 21))
        };

        [Benchmark(Baseline = true)]
        [ArgumentsSource(nameof(Pairs))]
        public string HighlightRange((string expression, SourceRange range) pair) =>
            CalculatorDiagnostics.HighlightRange(pair.expression, pair.range);

        //[Benchmark ( )]
        //[ArgumentsSource ( nameof ( Pairs ) )]
        //public String HighlightRange2 ( (String expression, SourceRange range) pair ) =>
        //    CalculatorDiagnostics.HighlightRange2 ( pair.expression, pair.range );
    }
}