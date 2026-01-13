using System;
using System.Windows.Input;

namespace LibraryWPFApp
{
    /// <summary>
    /// Реализация интерфейса ICommand для делегатов.
    /// Позволяет связывать команды с методами в ViewModel.
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        /// <summary>
        /// Событие, возникающее при изменении возможности выполнения команды.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса RelayCommand.
        /// </summary>
        /// <param name="execute">Делегат для выполнения команды.</param>
        /// <param name="canExecute">Делегат для проверки возможности выполнения команды.</param>
        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException("execute");
            _canExecute = canExecute;
        }

        /// <summary>
        /// Определяет, может ли команда выполняться.
        /// </summary>
        /// <param name="parameter">Параметр команды (не используется).</param>
        /// <returns>True если команда может выполняться, иначе False.</returns>
        public bool CanExecute(object parameter)
        {
            if (_canExecute != null)
                return _canExecute.Invoke();
            return true;
        }

        /// <summary>
        /// Выполняет команду.
        /// </summary>
        /// <param name="parameter">Параметр команды (не используется).</param>
        public void Execute(object parameter)
        {
            _execute();
        }

        /// <summary>
        /// Принудительно вызывает проверку возможности выполнения команды.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}