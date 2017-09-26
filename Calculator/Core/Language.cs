using Calculator.Core.Runtime;
using System;
using System.Collections.Generic;

namespace Calculator.Core
{
	public static class Language
	{
		#region 1. Operators
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

			if ( Operator.Length > 1 )
				throw new InvalidOperationException ( "Cannot add an operator that's composed of more than 1 character." );

			Operators.Add ( Operator, Description );
		}

		public static Boolean IsOperator ( String op ) => Operators.ContainsKey ( op );

		public static Boolean IsOperator ( Char ch ) => Operators.ContainsKey ( ch.ToString ( ) );

		#endregion 1. Operators

		#region 2. Constants

		public static readonly IDictionary<String, Double> Constants = new Dictionary<String, Double> ( );

		public static void AddConstant ( String Name, Double Value )
		{
			Constants.Add ( Name, Value );
		}

		public static Boolean IsConstant ( String Name ) => Constants.ContainsKey ( Name );

		#endregion 2. Constants
	}
}