using Calculator.Core;
using Calculator.Core.Lexing;
using Calculator.Core.Parsing;
using Calculator.Core.Parsing.Nodes.Base;
using Calculator.Core.Runtime.Base;
using Calculator.Core.Runtime.Operators.Mathematics;
using Calculator.Core.Runtime.Operators.Programming;
using GUtils.Benchmarking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Calculator.UI.Forms
{
	public partial class MainForm : Form
	{
		public MainForm ( )
		{
			InitializeComponent ( );

			#region constant: π
			Language.AddConstant ( "PI", Math.PI );
			Language.AddConstant ( "Pi", Math.PI );
			Language.AddConstant ( "pI", Math.PI );
			Language.AddConstant ( "pi", Math.PI );
			Language.AddConstant ( "π", Math.PI );
			#endregion constant: π

			#region constant: e
			Language.AddConstant ( "E", Math.E );
			Language.AddConstant ( "e", Math.E );
			#endregion constant: e

			#region Operators
			// Priority: 1
			Language.AddOperator ( "+", new AdditionOperator ( 1, Associativity.Left ) );
			Language.AddOperator ( "-", new SubtractionOperator ( 1, Associativity.Left ) );

			// Priority: 2
			Language.AddOperator ( "*", new MultiplicationOperator ( 2, Associativity.Left ) );
			Language.AddOperator ( "/", new DivisionOperator ( 2, Associativity.Left ) );
			Language.AddOperator ( "%", new ModuloOperator ( 2, Associativity.Left ) );

			// Priority: 3
			Language.AddOperator ( "**", new ExponentiationOperator ( 3, Associativity.Right ) );

			// Priority: 4
			Language.AddOperator ( "<<", new LShiftOperator ( 4, Associativity.Left ) );
			Language.AddOperator ( ">>", new RShiftOperator ( 4, Associativity.Left ) );

			// Priority: 5
			Language.AddOperator ( "&", new BitwiseAndOperator ( 5, Associativity.Left ) );

			// Priority: 6
			Language.AddOperator ( "^", new XOROperator ( 6, Associativity.Left ) );

			// Priority: 7
			Language.AddOperator ( "|", new BitwiseOrOperator ( 7, Associativity.Left ) );
			#endregion Operators

			#region function

			Language.AddFunction ( Math.Abs );
			Language.AddFunction ( Math.Acos );
			Language.AddFunction ( Math.Asin );
			Language.AddFunction ( Math.Atan );
			Language.AddFunction ( Math.Atan2 );
			Language.AddFunction ( "ceil", Math.Ceiling );
			Language.AddFunction ( Math.Cos );
			Language.AddFunction ( Math.Cosh );
			Language.AddFunction ( Math.Exp );
			Language.AddFunction ( Math.Floor );
			Language.AddFunction ( "ln", ( SingleParamMathFunction ) Math.Log );
			Language.AddFunction ( ( DoubleParamMathFunction ) Math.Log );
			Language.AddFunction ( Math.Log10 );
			Language.AddFunction ( Math.Max );
			Language.AddFunction ( Math.Min );
			Language.AddFunction ( Math.Pow );
			Language.AddFunction ( Math.Round );
			Language.AddFunction ( Math.Sin );
			Language.AddFunction ( Math.Sinh );
			Language.AddFunction ( Math.Sqrt );
			Language.AddFunction ( Math.Tan );
			Language.AddFunction ( Math.Tanh );
			Language.AddFunction ( Math.Truncate );

			#endregion function
		}

		private void BtnEquals_Click ( Object sender, EventArgs e )
		{
			this.listBox1.Items.Clear ( );
			this.listBox2.Items.Clear ( );
			var swPart = new PrecisionStopwatch ( );
			var swTotal = new PrecisionStopwatch ( );
			IEnumerable<Token> tokens;

			try
			{
				swTotal.Start ( );
				swPart.Start ( );
				tokens = Lexer.Process ( this.txtExpression.Text );
				swPart.Stop ( );
				swTotal.Stop ( );

				this.SetLexingTime ( swPart );
				this.listBox1.Items.AddRange ( tokens.ToArray ( ) );
			}
			catch ( ExpressionException ex )
			{
				Console.WriteLine ( ex.ToString ( ) );
				this.listBox2.Items.Add ( $"Syntax Error: {ex.Message}" );
				return;
			}
			catch ( Exception ex )
			{
				Console.WriteLine ( ex.ToString ( ) );
				this.listBox2.Items.Add ( "Unknown error." );
				return;
			}

			ASTNode exp = null;
			try
			{
				swTotal.Start ( );
				swPart.Restart ( );
				exp = Parser.Parse ( tokens, Console.WriteLine );
				swPart.Stop ( );
				swTotal.Stop ( );

				this.SetParsingTime ( swPart );
				if ( exp != null )
				{
					this.listBox2.Items.Add ( exp );
					this.txtAST.Text = exp.ToString ( );
				}
				else
				{
					this.listBox2.Items.Add ( "Expression was null (some error ocurred)." );
				}
			}
			catch ( ExpressionException ex )
			{
				Console.WriteLine ( ex.ToString ( ) );
				this.listBox2.Items.Add ( $"Syntax Error: {ex.Message}" );
				return;
			}
			catch ( Exception ex )
			{
				Console.WriteLine ( ex.ToString ( ) );
				this.listBox2.Items.Add ( "Unknown error." );
				return;
			}

			try
			{
				swTotal.Start ( );
				swPart.Restart ( );
				Double res = ( ( ValueExpression ) exp.Resolve ( ) ).Value;
				swPart.Stop ( );
				swTotal.Stop ( );

				this.txtResult.Text = res.ToString ( );
				this.SetExecutingTime ( swPart );
				this.SetTotalTime ( swTotal );
			}
			catch ( Exception ex )
			{
				Console.WriteLine ( ex.ToString ( ) );
				this.listBox2.Items.Add ( $"Error solving: {ex.Message}" );
			}
		}

		private static String FormatSWTime ( PrecisionStopwatch sw ) => $"{sw.ElapsedMicroseconds}µs";

		private void SetLexingTime ( PrecisionStopwatch sw )
		{
			this.txtTimeLexing.Text = FormatSWTime ( sw );
		}

		private void SetParsingTime ( PrecisionStopwatch sw )
		{
			this.txtTimeParsing.Text = FormatSWTime ( sw );
		}

		private void SetExecutingTime ( PrecisionStopwatch sw )
		{
			this.txtTimeExecuting.Text = FormatSWTime ( sw );
		}

		private void SetTotalTime ( PrecisionStopwatch sw )
		{
			this.txtTimeTotal.Text = FormatSWTime ( sw );
		}

		private void TxtBench_Click ( Object sender, EventArgs e )
		{
			var expr = this.txtExpression.Text;
			var bench = new Benchmark ( );
			var res = bench.Run ( ( ) =>
			{
				try
				{
					IEnumerable<Token> tokens = Lexer.Process ( expr );
					ASTNode ast = Parser.Parse ( tokens );
					Double expres = ( ( ValueExpression ) ast.Resolve ( ) ).Value;
				}
				catch ( Exception ) { }
			} );
			this.listBox2.Items.Add ( $"Benchmark result for {expr}" );
			this.listBox2.Items.Add ( $"	Average time of {res} μs" );
		}
	}
}
