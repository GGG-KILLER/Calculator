using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Calculator.Definitions
{
    /// <summary>
    /// Represents a function definition
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1716:Identifiers should not match keywords", Justification = "Renaming would be a breaking change.")]
    public readonly struct Function : IEquatable<Function>
    {
        /// <summary>
        /// The name of the function
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The list of overloads indexed by argument count (-1 is varargs)
        /// </summary>
        public ImmutableDictionary<int, Delegate> Overloads { get; }

        /// <summary>
        /// Initializes a new <see cref="Function" />
        /// <para>
        /// ONLY PASS DELEGATES THAT ACCEPT AND RETURN DOUBLES WITH PARAMETER COUNT BETWEEN 1 AND 4 (OR A
        /// SINGLE DOUBLE ARRAY AS INPUT AND A DOUBLE AS OUTPUT)
        /// </para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="overloads"></param>
        internal Function(string name, ImmutableDictionary<int, Delegate> overloads)
        {
            Name = name;
            Overloads = overloads;
        }

        /// <summary>
        /// Initializes a new <see cref="Function" />
        /// </summary>
        /// <param name="name"></param>
        public Function(string name) : this(name, ImmutableDictionary.Create<int, Delegate>())
        {
        }

        /// <summary>
        /// Initializes a new <see cref="Function" /> with this overload set
        /// </summary>
        /// <param name="overload"></param>
        /// <returns></returns>
        public Function SetOverload(Func<double> overload) =>
            new Function(Name, Overloads.SetItem(0, overload));

        /// <summary>
        /// Initializes a new <see cref="Function" /> with this overload set
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public Function SetOverload(Func<double, double> func) =>
            new Function(Name, Overloads.SetItem(1, func));

        /// <summary>
        /// Initializes a new <see cref="Function" /> with this overload set
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public Function SetOverload(Func<double, double, double> func) =>
            new Function(Name, Overloads.SetItem(2, func));

        /// <summary>
        /// Initializes a new <see cref="Function" /> with this overload set
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public Function SetOverload(Func<double, double, double, double> func) =>
            new Function(Name, Overloads.SetItem(3, func));

        /// <summary>
        /// Initializes a new <see cref="Function" /> with this overload set
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public Function SetOverload(Func<double, double, double, double, double> func) =>
            new Function(Name, Overloads.SetItem(4, func));

        /// <summary>
        /// Initializes a new <see cref="Function" /> with this overload set
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public Function SetOverload(Func<double[], double> func) =>
            new Function(Name, Overloads.SetItem(-1, func));

        #region Generated Code

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) =>
            obj is Function function && Equals(function);

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Function other) =>
            StringComparer.OrdinalIgnoreCase.Equals(Name, other.Name)
            && EqualityComparer<ImmutableDictionary<int, Delegate>>.Default.Equals(Overloads, other.Overloads);

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            var hashCode = -687234156;
            hashCode = hashCode * -1521134295 + StringComparer.OrdinalIgnoreCase.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<ImmutableDictionary<int, Delegate>>.Default.GetHashCode(Overloads);
            return hashCode;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="definition1"></param>
        /// <param name="definition2"></param>
        /// <returns></returns>
        public static bool operator ==(Function definition1, Function definition2) => definition1.Equals(definition2);

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        /// <param name="definition1"></param>
        /// <param name="definition2"></param>
        /// <returns></returns>
        public static bool operator !=(Function definition1, Function definition2) => !(definition1 == definition2);

        #endregion Generated Code
    }
}