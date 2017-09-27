using Calculator.Core;
using Calculator.Core.Lexing;
using Calculator.Core.Parsing;
using Calculator.Core.Parsing.Nodes.Base;
using Calculator.Core.Runtime;
using Calculator.Core.Timing;
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

			Language.AddConstant ( "PI", Math.PI );
			Language.AddConstant ( "Pi", Math.PI );
			Language.AddConstant ( "pI", Math.PI );
			Language.AddConstant ( "pi", Math.PI );

			Language.AddConstant ( "E", Math.E );
			Language.AddConstant ( "e", Math.E );

			Language.AddOperator ( "+", new AdditionOperator
			{
				BackupPriority = 1,
				OwnPriority = 1
			} );

			Language.AddOperator ( "-", new SubtractionOperator
			{
				BackupPriority = 1,
				OwnPriority = 1
			} );

			Language.AddOperator ( "*", new MultiplicationOperator
			{
				BackupPriority = 2,
				OwnPriority = 2
			} );

			Language.AddOperator ( "/", new DivisionOperator
			{
				BackupPriority = 2,
				OwnPriority = 2
			} );

			Language.AddOperator ( "^", new ExponentiationOperator
			{
				BackupPriority = 3,
				OwnPriority = 4
			} );
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
			catch ( Exception ex )
			{
				this.listBox1.Items.Add ( $"Invalid expression: {ex.GetType ( ).Name}" );
				this.listBox1.Items.Add ( ex.Message );
				this.listBox1.Items.AddRange ( ex.StackTrace.Split ( '\n' ) );
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
					this.listBox2.Items.Add ( exp.Resolve ( ) );
					this.listBox2.Items.Add ( exp.Resolve ( ) );
					this.listBox2.Items.Add ( ( ( ValueExpression ) exp.Resolve ( ) ).Value );
				}
				else
				{
					this.listBox2.Items.Add ( "Expression was null (some error ocurred)." );
				}
			}
			catch ( Exception ex )
			{
				this.listBox2.Items.Add ( $"Invalid expression: {ex.GetType ( ).Name}" );
				this.listBox2.Items.Add ( ex.Message );
				this.listBox2.Items.AddRange ( ex.StackTrace.Split ( '\n' ) );
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
				this.listBox2.Items.Add ( $"Error solving: {ex.GetType ( ).Name}" );
				this.listBox2.Items.Add ( ex.Message );
				this.listBox2.Items.AddRange ( ex.StackTrace.Split ( '\n' ) );
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
	}
}
