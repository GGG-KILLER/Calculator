using Calculator.Core;
using Calculator.Core.Parsing.Nodes.Base;
using Calculator.Core.Parsing.Nodes.Literals;
using Calculator.Core.Lexing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Calculator.Core.Parsing;
using Calculator.Core.Runtime;
using Calculator.Core.Timing;

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
				OwnPriority = 3
			} );
		}

		private void BtnEquals_Click ( Object sender, EventArgs e )
		{
			this.listBox1.Items.Clear ( );
			this.listBox2.Items.Clear ( );
			var swPart = new PrecisionStopwatch ( );
			var swTotal = new PrecisionStopwatch ( );
			IEnumerable<Token> tokens;

			//try
			//{
			swTotal.Start ( );
			swPart.Start ( );
			tokens = Lexer.Process ( this.txtExpression.Text );
			swPart.Stop ( );
			swTotal.Stop ( );

			this.txtTimeTokenizing.Text = swPart.ElapsedMicroseconds + " μs";
			this.listBox1.Items.AddRange ( tokens.ToArray ( ) );
			//}
			//catch ( Exception ex )
			//{
			//	this.listBox1.Items.Add ( $"Invalid expression: {ex.GetType ( ).Name}" );
			//	this.listBox1.Items.Add ( ex.Message );
			//	this.listBox1.Items.AddRange ( ex.StackTrace.Split ( '\n' ) );
			//	return;
			//}

			try
			{
				swTotal.Start ( );
				swPart.Restart ( );
				var parser = new Parser ( tokens, Console.WriteLine );
				ASTNode exp = parser.Parse ( );
				if ( exp != null )
				{
					this.listBox2.Items.Add ( exp );
					this.listBox2.Items.Add ( exp.Resolve ( ) );
				}
				else
					this.listBox2.Items.Add ( "Expression null." );
			}
			catch ( Exception ex )
			{
				this.listBox2.Items.Add ( $"Invalid expression: {ex.GetType ( ).Name}" );
				this.listBox2.Items.Add ( ex.Message );
				this.listBox2.Items.AddRange ( ex.StackTrace.Split ( '\n' ) );
			}
			finally
			{
				swPart.Stop ( );
				swTotal.Stop ( );
			}

			this.txtTimeLexing.Text = swPart.ElapsedMicroseconds + " μs";
			this.txtTimeTotal.Text = swTotal.ElapsedMicroseconds + " μs";
		}
	}
}
