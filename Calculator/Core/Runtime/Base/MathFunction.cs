using System;
using System.Linq;
using System.Reflection;

namespace Calculator.Core.Runtime.Base
{
	public class MathFunction
	{
		/// <summary>
		/// The number of arguments this function accepts
		/// </summary>
		public Int32 ArgumentCount { get; protected set; }

		/// <summary>
		/// Whether this function accepts any number of arguments
		/// </summary>
		public Boolean VariableArgumentCount { get; protected set; }

		/// <summary>
		/// The function itself
		/// </summary>
		private readonly Delegate Action;

		public Double Execute ( Double[] Args ) => ( Double ) ( this.VariableArgumentCount
														? this.Action.DynamicInvoke ( new Object[] { Args } )
														: this.Action.DynamicInvoke ( Args ) );

		public MathFunction ( Delegate Action )
		{
			if ( Action.Method.ReturnParameter.ParameterType != typeof ( Double ) )
				throw new ArgumentException ( "The function should return a double!", nameof ( Action ) );

			ParameterInfo[] @params = Action.Method.GetParameters ( );
			if ( @params.Length == 1 && @params[0].ParameterType == typeof ( Double[] ) )
			{
				this.VariableArgumentCount = true;
				this.ArgumentCount = -1;
			}
			else
			{
				if ( @params.Any ( param => param.ParameterType != typeof ( Double ) ) )
					throw new ArgumentException ( "All arguments should be doubles.", nameof ( Action ) );

				this.ArgumentCount = @params.Length;
				this.VariableArgumentCount = false;
			}

			this.Action = Action;
		}
	}
}
