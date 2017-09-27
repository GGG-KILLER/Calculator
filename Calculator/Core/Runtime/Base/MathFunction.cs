using System;
using System.Linq;
using System.Reflection;

namespace Calculator.Core.Runtime.Base
{
	#region Delegates

	public delegate Double SingleParamMathFunction ( Double x );

	public delegate Double DoubleParamMathFunction ( Double x, Double y );

	public delegate Double TripleParamMathFunction ( Double x, Double y, Double z );

	public delegate Double VarargParamMathFunction ( params Double[] args );

	#endregion Delegates

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

		public Double Execute ( Double[] Args )
		{
			if ( this.VariableArgumentCount )
				return ( Double ) this.Action.DynamicInvoke ( new Object[] { Args } );
			else
			{
				var oArgs = new Object[Args.Length];
				Array.Copy ( Args, oArgs, Args.Length );
				return ( Double ) this.Action.DynamicInvoke ( oArgs );
			}
		}

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
