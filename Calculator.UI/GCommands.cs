using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Calculator.UI
{
    public static class GCommands
    {
        public static RoutedUICommand Enter { get; } = new RoutedUICommand ( "Enter key press command", nameof ( Enter ), typeof ( GCommands ), new InputGestureCollection { new KeyGesture ( Key.Enter, ModifierKeys.None ) } );
    }
}
