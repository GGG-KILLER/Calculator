using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using Calculator.Parsing.Visitors;
using GParse;
using GParse.Errors;

namespace Calculator.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow ( )
        {
            this.InitializeComponent ( );
        }

        private void txtExpression_TextChanged ( Object sender, System.Windows.Controls.TextChangedEventArgs e )
        {
            try
            {
                Parsing.AST.CalculatorTreeNode ast = CalculatorLanguageSingleton.Instance.Parse ( this.txtExpression.Text, out IEnumerable<Diagnostic> diagnostics );
                this.txtResult.Text = ast.Accept ( new TreeEvaluator ( CalculatorLanguageSingleton.Instance ) ).ToString ( CultureInfo.CurrentCulture );
                this.dgDiagnostics.ItemsSource = diagnostics;
            }
            catch ( FatalParsingException fpEx )
            {
                this.dgDiagnostics.ItemsSource = new Diagnostic[]
                {
                    new Diagnostic ( "FATAL", fpEx.Range, DiagnosticSeverity.Error, fpEx.Message )
                };
            }
        }
    }
}