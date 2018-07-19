﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MP.Contacts.Support
{
    public class RelayCommandAsync : ICommand
    {
        private readonly Func<object, Task> executedMethod;
        private readonly Func<object, bool> canExecuteMethod;

        public event EventHandler CanExecuteChanged;

        public RelayCommandAsync(Func<object, Task> execute) : this(execute, null)
        {
        }

        public RelayCommandAsync(Func<object, Task> execute, Func<object, bool> canExecute)
        {
            this.executedMethod = execute ?? throw new ArgumentNullException("execute");
            this.canExecuteMethod = canExecute;
        }

        public bool CanExecute(object parameter) => this.canExecuteMethod == null || this.canExecuteMethod((object)parameter);

        public async void Execute(object parameter) => await this.executedMethod((object)parameter);

        public void OnCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}