using System;
using System.Collections.Immutable;

namespace Calculator.Definitions
{
    /// <summary>
    /// A class to build function definitions
    /// </summary>
    public class FunctionBuilder
    {
        private readonly String name;
        private readonly ImmutableDictionary<Int32, Delegate>.Builder overloads;

        /// <summary>
        /// Initializes this <see cref="FunctionBuilder" />
        /// </summary>
        /// <param name="name"></param>
        public FunctionBuilder ( String name )
        {
            if ( String.IsNullOrWhiteSpace ( name ) )
                throw new ArgumentException ( "message", nameof ( name ) );

            this.name = name;
            this.overloads = ImmutableDictionary.CreateBuilder<Int32, Delegate> ( );
        }

        #region AddOverload

        /// <summary>
        /// Adds an overload to the function being built
        /// </summary>
        /// <param name="overload"></param>
        /// <returns></returns>
        public FunctionBuilder AddOverload ( Func<Double> overload )
        {
            this.overloads[0] = overload ?? throw new ArgumentNullException ( nameof ( overload ) );
            return this;
        }

        /// <summary>
        /// Adds an overload to the function being built
        /// </summary>
        /// <param name="overload"></param>
        /// <returns></returns>
        public FunctionBuilder AddOverload ( Func<Double, Double> overload )
        {
            this.overloads[1] = overload ?? throw new ArgumentNullException ( nameof ( overload ) );
            return this;
        }

        /// <summary>
        /// Adds an overload to the function being built
        /// </summary>
        /// <param name="overload"></param>
        /// <returns></returns>
        public FunctionBuilder AddOverload ( Func<Double, Double, Double> overload )
        {
            this.overloads[2] = overload ?? throw new ArgumentNullException ( nameof ( overload ) );
            return this;
        }

        /// <summary>
        /// Adds an overload to the function being built
        /// </summary>
        /// <param name="overload"></param>
        /// <returns></returns>
        public FunctionBuilder AddOverload ( Func<Double, Double, Double, Double> overload )
        {
            this.overloads[3] = overload ?? throw new ArgumentNullException ( nameof ( overload ) );
            return this;
        }

        /// <summary>
        /// Adds an overload to the function being built
        /// </summary>
        /// <param name="overload"></param>
        /// <returns></returns>
        public FunctionBuilder AddOverload ( Func<Double, Double, Double, Double, Double> overload )
        {
            this.overloads[4] = overload ?? throw new ArgumentNullException ( nameof ( overload ) );
            return this;
        }

        /// <summary>
        /// Adds an overload to the function being built
        /// </summary>
        /// <param name="overload"></param>
        /// <returns></returns>
        public FunctionBuilder AddOverload ( Func<Double[], Double> overload )
        {
            this.overloads[-1] = overload ?? throw new ArgumentNullException ( nameof ( overload ) );
            return this;
        }

        #endregion AddOverload

        /// <summary>
        /// Generates a <see cref="Function" />
        /// </summary>
        /// <returns></returns>
        public Function GetFunctionDefinition ( ) =>
            new Function ( this.name, this.overloads.ToImmutable ( ) );
    }
}
