using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Calculator.Core;

namespace Calculator.UI.Forms
{
    public partial class MainForm : Form
    {
        public MainForm ( )
        {
            InitializeComponent ( );
        }

        private void BtnEquals_Click ( Object sender, EventArgs e )
        {
            this.listBox1.Items.Clear ( );
            this.listBox2.Items.Clear ( );
            var swPart = new Stopwatch ( );
            var swTotal = new Stopwatch ( );

            swTotal.Start ( );
            swPart.Start ( );
            var tokens = Tokenizer.Tokenize ( this.txtExpression.Text );
            swPart.Stop ( );
            swTotal.Stop ( );

            this.txtTimeTokenizing.Text = swPart.ElapsedMilliseconds + "ms";
            this.listBox1.Items.AddRange ( tokens.Select ( token => token.GetType ( ).FullName )
                .ToArray ( ) );

            try
            {
                swTotal.Start ( );
                swPart.Restart ( );

                this.listBox2.Items.Add ( String.Join ( " ", tokens ) );
                foreach ( System.Collections.Generic.IEnumerable<Core.Tokens.Token> partial in new Lexer ( tokens )
                    .ProgressivelySolve ( ) )
                    this.listBox2.Items.Add ( String.Join ( " ", partial ) );
            }
            catch ( Exception ex )
            {
                this.listBox2.Items.Add ( $"Invalid expression: {ex.GetType ( ).Name}" );
                this.listBox2.Items.Add ( ex.Message );
            }
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
