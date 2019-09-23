using System;
using BenchmarkDotNet.Running;

namespace Calculator.Benchmark
{
    class Program
    {
        static void Main ( String[] args ) =>
            BenchmarkSwitcher.FromAssembly ( typeof ( Program ).Assembly ).Run ( args );
    }
}
