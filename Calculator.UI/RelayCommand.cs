using System;
using System.Windows.Input;

namespace Calculator.UI
{
    // Copied from https://github.com/Mijyuoon/Akyuu all credit goes to Mijyuoon
    public class RelayCommand : ICommand
    {
        private readonly Action<Object> execute;
        private readonly Predicate<Object> canExecute;

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public RelayCommand ( Action<Object> execute, Predicate<Object> canExecute )
        {
            this.execute = execute ?? throw new ArgumentNullException ( nameof ( execute ) );
            this.canExecute = canExecute;
        }

        public RelayCommand ( Action<Object> execute ) : this ( execute, null )
        {
        }

        public void Execute ( Object parameter ) =>
            this.execute ( parameter );

        public Boolean CanExecute ( Object parameter ) =>
            this.canExecute?.Invoke ( parameter ) ?? true;
    }
}
