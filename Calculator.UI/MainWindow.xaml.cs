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
        public MainWindow() => InitializeComponent();

        private void txtExpression_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            try
            {
                var ast = CalculatorLanguageSingleton.Instance.Parse(txtExpression.Text, out var diagnostics);
                txtResult.Text = ast.Accept(new TreeEvaluator(CalculatorLanguageSingleton.Instance)).ToString(CultureInfo.CurrentCulture);

                var diagsVw = new List<DiagnosticViewModel>();
                foreach (var diagnostic in diagnostics)
                {
                    diagsVw.Add(new DiagnosticViewModel(diagnostic));
                }
                dgDiagnostics.ItemsSource = diagsVw;
            }
            catch (FatalParsingException fpEx)
            {
                dgDiagnostics.ItemsSource = new DiagnosticViewModel[]
                {
                    new DiagnosticViewModel(new Diagnostic(DiagnosticSeverity.Error, "FATAL", fpEx.Message, fpEx.Range))
                };
            }
        }
    }
}