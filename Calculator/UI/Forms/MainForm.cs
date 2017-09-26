using Calculator.Core;
using Calculator.Core.Nodes;
using Calculator.Core.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace Calculator.UI.Forms
{
	public partial class MainForm : Form
	{
		public MainForm ( )
		{
			InitializeComponent ( );
			ConstantValue.Values["pi"] = Math.PI;
			ConstantValue.Values["Pi"] = Math.PI;
			ConstantValue.Values["pI"] = Math.PI;
			ConstantValue.Values["PI"] = Math.PI;
		}

		private void BtnEquals_Click ( Object sender, EventArgs e )
		{
			this.listBox1.Items.Clear ( );
			this.listBox2.Items.Clear ( );
			var swPart = new Stopwatch ( );
			var swTotal = new Stopwatch ( );

			swTotal.Start ( );
			swPart.Start ( );
			IEnumerable<Token> tokens = Lexer.Process ( this.txtExpression.Text, Console.WriteLine );
			swPart.Stop ( );
			swTotal.Stop ( );

			this.txtTimeTokenizing.Text = swPart.ElapsedMilliseconds + "ms";
			this.listBox1.Items.AddRange ( tokens.ToArray ( ) );

			try
			{
				swTotal.Start ( );
				swPart.Restart ( );
				var parser = new Parser ( tokens );
				ASTNode exp = parser.ParseExpression ( Console.WriteLine );
				if ( exp != null )
				{
					this.listBox2.Items.Add ( exp );
					this.listBox2.Items.Add ( exp.Resolve ( ) );
				}
				else
					this.listBox2.Items.Add ( "Expression null." );
			}
			//catch ( Exception ex )
			//{
			//	this.listBox2.Items.Add ( $"Invalid expression: {ex.GetType ( ).Name}" );
			//	this.listBox2.Items.Add ( ex.Message );
			//	this.listBox2.Items.AddRange ( ex.StackTrace.Split ( '\n' ) );
			//}
			finally
			{
				swPart.Stop ( );
				swTotal.Stop ( );
			}

			this.txtTimeLexing.Text = swPart.ElapsedMilliseconds + "ms";
			this.txtTimeTotal.Text = swTotal.ElapsedMilliseconds + "ms";
		}
	}
}
