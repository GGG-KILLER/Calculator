using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Calculator.Definitions
{
    /// <summary>
    /// Represents a function definition
    /// </summary>
    public readonly struct Function : IEquatable<Function>
    {
        /// <summary>
        /// The name of the function
        /// </summary>
        public String Name { get; }

        /// <summary>
        /// The list of overloads indexed by argument count (-1 is varargs)
        /// </summary>
        public ImmutableDictionary<Int32, Delegate> Overloads { get; }

        /// <summary>
        /// Initializes a new <see cref="Function" />
        /// <para>
        /// ONLY PASS DELEGATES THAT ACCEPT AND RETURN DOUBLES WITH PARAMETER COUNT BETWEEN 1 AND 4 (OR A
        /// SINGLE DOUBLE ARRAY AS INPUT AND A DOUBLE AS OUTPUT)
        /// </para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="overloads"></param>
        internal Function ( String name, ImmutableDictionary<Int32, Delegate> overloads )
        {
            this.Name      = name;
            this.Overloads = overloads;
        }

        /// <summary>
        /// Initializes a new <see cref="Function" />
        /// </summary>
        /// <param name="name"></param>
        public Function ( String name ) : this ( name, ImmutableDictionary.Create<Int32, Delegate> ( ) )
        {
        }

        /// <summary>
        /// Initializes a new <see cref="Function" /> with this overload set
        /// </summary>
        /// <param name="overload"></param>
        /// <returns></returns>
        public Function SetOverload ( Func<Double> overload ) =>
            new Function ( this.Name, this.Overloads.SetItem ( 0, overload ) );

        /// <summary>
        /// Initializes a new <see cref="Function" /> with this overload set
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public Function SetOverload ( Func<Double, Double> func ) =>
            new Function ( this.Name, this.Overloads.SetItem ( 1, func ) );

        /// <summary>
        /// Initializes a new <see cref="Function" /> with this overload set
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public Function SetOverload ( Func<Double, Double, Double> func ) =>
            new Function ( this.Name, this.Overloads.SetItem ( 2, func ) );

        /// <summary>
        /// Initializes a new <see cref="Function" /> with this overload set
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public Function SetOverload ( Func<Double, Double, Double, Double> func ) =>
            new Function ( this.Name, this.Overloads.SetItem ( 3, func ) );

        /// <summary>
        /// Initializes a new <see cref="Function" /> with this overload set
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public Function SetOverload ( Func<Double, Double, Double, Double, Double> func ) =>
            new Function ( this.Name, this.Overloads.SetItem ( 4, func ) );

        /// <summary>
        /// Initializes a new <see cref="Function" /> with this overload set
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public Function SetOverload ( Func<Double[], Double> func ) =>
            new Function ( this.Name, this.Overloads.SetItem ( -1, func ) );

        #region Generated Code

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override Boolean Equals ( Object obj ) => obj is Function && this.Equals ( ( Function ) obj );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Boolean Equals ( Function other ) => this.Name == other.Name && EqualityComparer<ImmutableDictionary<Int32, Delegate>>.Default.Equals ( this.Overloads, other.Overloads );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <returns></returns>
        public override Int32 GetHashCode ( )
        {
            var hashCode = -687234156;
            hashCode = hashCode * -1521134295 + EqualityComparer<String>.Default.GetHashCode ( this.Name );
            hashCode = hashCode * -1521134295 + EqualityComparer<ImmutableDictionary<Int32, Delegate>>.Default.GetHashCode ( this.Overloads );
            return hashCode;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="definition1"></param>
        /// <param name="definition2"></param>
        /// <returns></returns>
        public static Boolean operator == ( Function definition1, Function definition2 ) => definition1.Equals ( definition2 );

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="definition1"></param>
        /// <param name="definition2"></param>
        /// <returns></returns>
        public static Boolean operator != ( Function definition1, Function definition2 ) => !( definition1 == definition2 );

        #endregion Generated Code
    }
}
