using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using GParse;

namespace Calculator.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public RoutedUICommand EnterCommand { get; }

        public MainWindow ( )
        {
            this.EnterCommand = new RoutedUICommand ( "Enter key press", "Enter", typeof ( MainWindow ), new InputGestureCollection
            {
                new KeyGesture ( Key.Enter )
            } );
            this.InitializeComponent ( );
        }

        private void EnterCommand_CanExecute ( Object sender, CanExecuteRoutedEventArgs e )
        {
            e.CanExecute = this.txtExpression.Text.Length > 0;
        }

        private void EnterCommand_Executed ( Object sender, ExecutedRoutedEventArgs e )
        {
            Parsing.AST.CalculatorTreeNode ast = CalculatorLanguageSingleton.Instance.Parse ( this.txtExpression.Text, out IEnumerable<Diagnostic> diagnostics );
            this.txtResult.Text = ast.Accept ( CalculatorLanguageSingleton.Instance.TreeEvaluator ).ToString ( );
            this.dgDiagnostics.ItemsSource = diagnostics;
        }
    }
}
