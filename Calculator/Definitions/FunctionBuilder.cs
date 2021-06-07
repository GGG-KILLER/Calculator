using System;
using System.Collections.Immutable;

namespace Calculator.Definitions
{
    /// <summary>
    /// A class to build function definitions
    /// </summary>
    public class FunctionBuilder
    {
        private readonly string name;
        private readonly ImmutableDictionary<int, Delegate>.Builder overloads;

        /// <summary>
        /// Initializes this <see cref="FunctionBuilder" />
        /// </summary>
        /// <param name="name"></param>
        public FunctionBuilder(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("message", nameof(name));

            this.name = name;
            overloads = ImmutableDictionary.CreateBuilder<int, Delegate>();
        }

        #region AddOverload

        /// <summary>
        /// Adds an overload to the function being built
        /// </summary>
        /// <param name="overload"></param>
        /// <returns></returns>
        public FunctionBuilder AddOverload(Func<double> overload)
        {
            overloads[0] = overload ?? throw new ArgumentNullException(nameof(overload));
            return this;
        }

        /// <summary>
        /// Adds an overload to the function being built
        /// </summary>
        /// <param name="overload"></param>
        /// <returns></returns>
        public FunctionBuilder AddOverload(Func<double, double> overload)
        {
            overloads[1] = overload ?? throw new ArgumentNullException(nameof(overload));
            return this;
        }

        /// <summary>
        /// Adds an overload to the function being built
        /// </summary>
        /// <param name="overload"></param>
        /// <returns></returns>
        public FunctionBuilder AddOverload(Func<double, double, double> overload)
        {
            overloads[2] = overload ?? throw new ArgumentNullException(nameof(overload));
            return this;
        }

        /// <summary>
        /// Adds an overload to the function being built
        /// </summary>
        /// <param name="overload"></param>
        /// <returns></returns>
        public FunctionBuilder AddOverload(Func<double, double, double, double> overload)
        {
            overloads[3] = overload ?? throw new ArgumentNullException(nameof(overload));
            return this;
        }

        /// <summary>
        /// Adds an overload to the function being built
        /// </summary>
        /// <param name="overload"></param>
        /// <returns></returns>
        public FunctionBuilder AddOverload(Func<double, double, double, double, double> overload)
        {
            overloads[4] = overload ?? throw new ArgumentNullException(nameof(overload));
            return this;
        }

        /// <summary>
        /// Adds an overload to the function being built
        /// </summary>
        /// <param name="overload"></param>
        /// <returns></returns>
        public FunctionBuilder AddOverload(Func<double[], double> overload)
        {
            overloads[-1] = overload ?? throw new ArgumentNullException(nameof(overload));
            return this;
        }

        #endregion AddOverload

        /// <summary>
        /// Generates a <see cref="Function" />
        /// </summary>
        /// <returns></returns>
        public Function GetFunctionDefinition() =>
            new Function(name, overloads.ToImmutable());
    }
}
