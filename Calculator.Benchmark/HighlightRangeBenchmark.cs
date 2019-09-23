using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.CoreRun;
using BenchmarkDotNet.Toolchains.CsProj;
using Calculator.Definitions;
using GParse;

namespace Calculator.Benchmark
{
    [Config(typeof(Config))]
    public class HighlightRangeBenchmark
    {
        private static SourceRange Range ( Int32 startLine, Int32 startColumn, Int32 startByte, Int32 endLine, Int32 endColumn, Int32 endByte ) =>
            new SourceLocation ( startLine, startColumn, startByte ).To ( new SourceLocation ( endLine, endColumn, endByte ) );

        public IEnumerable<(String, SourceRange)> Pairs => new[]
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

        [Benchmark ( Baseline = true )]
        [ArgumentsSource ( nameof ( Pairs ) )]
        public String HighlightRange ( (String expression, SourceRange range) pair ) =>
            CalculatorDiagnostics.HighlightRange ( pair.expression, pair.range );

        //[Benchmark ( )]
        //[ArgumentsSource ( nameof ( Pairs ) )]
        //public String HighlightRange2 ( (String expression, SourceRange range) pair ) =>
        //    CalculatorDiagnostics.HighlightRange2 ( pair.expression, pair.range );

        private class Config : ManualConfig
        {
            public Config ( )
            {
                Job @base = Job.Core.WithEvaluateOverhead ( true );
                this.Add ( @base.With ( CsProjCoreToolchain.NetCoreApp20 ) );
                this.Add ( @base.With ( CsProjCoreToolchain.NetCoreApp21 ) );
                this.Add ( @base.With ( CsProjCoreToolchain.NetCoreApp22 ) );
                this.Add ( @base.With ( CsProjCoreToolchain.NetCoreApp30 ) );
            }
        }
    }
}