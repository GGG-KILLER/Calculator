using Calculator.Core.Runtime.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Calculator.Core
{
	public static class Language
	{
		#region 1. Operators

		/// <summary>
		/// Holds all operators available in the language and
		/// their implementations
		/// </summary>
		public static readonly IDictionary<String, Operator> Operators = new Dictionary<String, Operator> ( );

		/// <summary>
		/// Adds an operator to the language
		/// </summary>
		/// <param name="Operator">
		/// Operator character (as a string since the
		/// <see cref="Lexing.Token" /> class only supports strings.)
		/// </param>
		/// <param name="Description"></param>
		public static void AddOperator ( String Operator, Operator Description )
		{
			if ( String.IsNullOrEmpty ( Operator ) )
				throw new ArgumentException ( "Operator must have a set value and not be empty.", nameof ( Operator ) );

			if ( Operator.Length < 1 )
				throw new InvalidOperationException ( "Cannot add an operator that's empty." );

			if ( IsOperator ( Operator ) )
				throw new InvalidOperationException ( "Cannot add an existing operator." );

			Operators.Add ( Operator, Description );
		}

		/// <summary>
		/// Checks wether a string corresponds to an operator
		/// </summary>
		/// <param name="op"></param>
		/// <returns></returns>
		public static Boolean IsOperator ( String op ) => Operators.ContainsKey ( op );

		#endregion 1. Operators

		#region 2. Constants

		/// <summary>
		/// Represents all available constants in the language
		/// </summary>
		public static readonly IDictionary<String, Double> Constants = new Dictionary<String, Double> ( );

		/// <summary>
		/// Adds a constant value to the language
		/// </summary>
		/// <param name="Name"></param>
		/// <param name="Value"></param>
		public static void AddConstant ( String Name, Double Value )
		{
			if ( String.IsNullOrEmpty ( Name ) )
				throw new ArgumentException ( "Constant name cannot be null.", nameof ( Name ) );

			Constants.Add ( Name, Value );
		}

		/// <summary>
		/// Checks whether a string corresponds to a constant name
		/// </summary>
		/// <param name="Name"></param>
		/// <returns></returns>
		public static Boolean IsConstant ( String Name ) => Constants.ContainsKey ( Name );

		#endregion 2. Constants

		#region 3. Functions

		/// <summary>
		/// The list of functions available in the language
		/// </summary>
		public static readonly IDictionary<String, MathFunction> Functions = new Dictionary<String, MathFunction> ( );

		/// <summary>
		/// Adds a function to the language using
		/// <paramref name="Function" />'s name in lower-case as name
		/// </summary>
		/// <param name="Function"></param>
		public static void AddFunction ( SingleParamMathFunction Function )
		{
			if ( String.IsNullOrEmpty ( Function.Method.Name ) )
				throw new Exception ( "Function must have a name" );
			if ( Function.Method.Name.Contains ( '<' ) )
				throw new Exception ( "Function must not be a lambda." );

			AddFunction ( Function.Method.Name.ToLowerInvariant ( ), Function );
		}

		/// <summary>
		/// Adds a function to the language using
		/// <paramref name="Function" />'s name in lower-case as name
		/// </summary>
		/// <param name="Function"></param>
		public static void AddFunction ( DoubleParamMathFunction Function )
		{
			if ( String.IsNullOrEmpty ( Function.Method.Name ) )
				throw new Exception ( "Function must have a name" );
			if ( Function.Method.Name.Contains ( '<' ) )
				throw new Exception ( "Function must not be a lambda." );

			AddFunction ( Function.Method.Name.ToLowerInvariant ( ), Function );
		}

		/// <summary>
		/// Adds a function to the language using
		/// <paramref name="Function" />'s name in lower-case as name
		/// </summary>
		/// <param name="Function"></param>
		public static void AddFunction ( TripleParamMathFunction Function )
		{
			if ( String.IsNullOrEmpty ( Function.Method.Name ) )
				throw new Exception ( "Function must have a name" );
			if ( Function.Method.Name.Contains ( '<' ) )
				throw new Exception ( "Function must not be a lambda." );

			AddFunction ( Function.Method.Name.ToLowerInvariant ( ), Function );
		}

		/// <summary>
		/// Adds a function to the language using
		/// <paramref name="Function" />'s name in lower-case as name
		/// </summary>
		/// <param name="Function"></param>
		public static void AddFunction ( VarargParamMathFunction Function )
		{
			if ( String.IsNullOrEmpty ( Function.Method.Name ) )
				throw new Exception ( "Function must have a name" );
			if ( Function.Method.Name.Contains ( '<' ) )
				throw new Exception ( "Function must not be a lambda." );

			AddFunction ( Function.Method.Name.ToLowerInvariant ( ), Function );
		}

		/// <summary>
		/// Adds a function to the language
		/// </summary>
		/// <param name="Name"></param>
		/// <param name="Function"></param>
		public static void AddFunction ( String Name, SingleParamMathFunction Function ) => AddFunction ( Name, new MathFunction ( Function ) );

		/// <summary>
		/// Adds a function to the language
		/// </summary>
		/// <param name="Name"></param>
		/// <param name="Function"></param>
		public static void AddFunction ( String Name, DoubleParamMathFunction Function ) => AddFunction ( Name, new MathFunction ( Function ) );

		/// <summary>
		/// Adds a function to the language
		/// </summary>
		/// <param name="Name"></param>
		/// <param name="Function"></param>
		public static void AddFunction ( String Name, TripleParamMathFunction Function ) => AddFunction ( Name, new MathFunction ( Function ) );

		/// <summary>
		/// Adds a function to the language
		/// </summary>
		/// <param name="Name"></param>
		/// <param name="Function"></param>
		public static void AddFunction ( String Name, VarargParamMathFunction Function ) => AddFunction ( Name, new MathFunction ( Function ) );

		/// <summary>
		/// Adds a function to the language
		/// </summary>
		/// <param name="Name"></param>
		/// <param name="Function"></param>
		public static void AddFunction ( String Name, MathFunction Function )
		{
			Name = Name?.Trim ( );

			if ( String.IsNullOrEmpty ( Name ) )
				throw new ArgumentException ( "Name must not be empty or null.", nameof ( Name ) );

			if ( !Char.IsLetter ( Name[0] ) || Name.Any ( ch => !Char.IsLetterOrDigit ( ch ) ) )
				throw new ArgumentException ( "Name must have it's first character as a letter and should be composed only of letters or digits.", nameof ( Name ) );

			Functions.Add ( Name, Function );
		}

		/// <summary>
		/// Checks whether a string corresponds to a function name
		/// or not
		/// </summary>
		/// <param name="Name"></param>
		/// <returns></returns>
		public static Boolean IsFunction ( String Name ) => Functions.ContainsKey ( Name );

		#endregion 3. Functions
	}
}
