using System;
using System.Windows.Input;

namespace WpfMediaPlayer.ViewModels
{
    /// <summary>
    /// Generalizirana implementacija ICommand vmesnika za MVVM vzorec
    /// </summary>
    public class Command : ICommand
    {
        // Referenca na metodo, ki se izvede ob kliku
        private readonly Action<object> _execute;

        // Referenca na metodo, ki preveri ali se akcija lahko izvede
        private readonly Func<object, bool> _canExecute;

        /// <summary>
        /// Dogodek, ki obvesti UI da se je stanje CanExecute spremenilo
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Konstruktor z obvezno execute in opcijsko canExecute metodo
        /// </summary>
        public Command(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <summary>
        /// Preveri ali se akcija lahko izvede
        /// </summary>
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        /// <summary>
        /// Izvede akcijo
        /// </summary>
        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        /// <summary>
        /// Ročno sproži CanExecuteChanged dogodek
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}