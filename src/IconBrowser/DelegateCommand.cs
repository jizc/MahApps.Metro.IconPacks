using System;
using System.Windows.Input;

namespace IconBrowser;

/// <summary>
/// Generic implementation of <see cref="ICommand"/>. Parameters are not supported.
/// </summary>
public class DelegateCommand : ICommand
{
    private readonly Action execute;
    private readonly Func<bool>? canExecute;

    /// <summary>
    /// Initializes a new instance of <see cref="DelegateCommand"/>.
    /// </summary>
    /// <param name="execute">The execution logic.</param>
    /// <param name="canExecute">The execution status logic.</param>
    public DelegateCommand(Action execute, Func<bool>? canExecute = null)
    {
        this.execute = execute;
        this.canExecute = canExecute;
    }

    /// <inheritdoc />
    public event EventHandler? CanExecuteChanged;

    /// <summary>
    /// Raises the <see cref="CanExecuteChanged" /> event.
    /// </summary>
    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

    /// <inheritdoc cref="ICommand.CanExecute" />
    public bool CanExecute() => canExecute is null || canExecute.Invoke();

    /// <inheritdoc cref="ICommand.Execute" />
    public void Execute() => execute.Invoke();

    /// <inheritdoc />
    bool ICommand.CanExecute(object? parameter) => CanExecute();

    /// <inheritdoc />
    void ICommand.Execute(object? parameter) => Execute();
}

/// <summary>
/// Generic implementation of <see cref="ICommand"/>.
/// </summary>
public class DelegateCommand<T> : ICommand
{
    private readonly Action<T> execute;
    private readonly Func<T, bool>? canExecute;

    /// <summary>
    /// Initializes a new instance of <see cref="DelegateCommand{T}"/>.
    /// </summary>
    /// <param name="execute">The execution logic.</param>
    /// <param name="canExecute">The execution status logic.</param>
    public DelegateCommand(Action<T> execute, Func<T, bool>? canExecute = null)
    {
        this.execute = execute;
        this.canExecute = canExecute;
    }

    /// <inheritdoc />
    public event EventHandler? CanExecuteChanged;

    /// <summary>
    /// Raises the <see cref="CanExecuteChanged" /> event.
    /// </summary>
    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

    /// <inheritdoc cref="ICommand.CanExecute" />
    public bool CanExecute(T parameter) => canExecute is null || canExecute.Invoke(parameter);

    /// <inheritdoc cref="ICommand.Execute" />
    public void Execute(T parameter) => execute.Invoke(parameter);

    /// <inheritdoc />
    bool ICommand.CanExecute(object? parameter)
    {
        if ((parameter is null && !IsNullValidParameter())
            || (parameter is not null && !IsSameOrSubclass(typeof(T), parameter.GetType())))
        {
            System.Diagnostics.Debug.WriteLine("ERROR: Parameter is not of type " + typeof(T));
            return false;
        }

        return CanExecute((T)parameter!);
    }

    /// <inheritdoc />
    void ICommand.Execute(object? parameter)
    {
        if ((parameter is null && !IsNullValidParameter())
            || (parameter is not null && !IsSameOrSubclass(typeof(T), parameter.GetType())))
        {
            throw new ArgumentException("Parameter is not of type " + typeof(T), nameof(parameter));
        }

        Execute((T)parameter!);
    }

    private static bool IsNullValidParameter() => default(T) is null;

    private static bool IsSameOrSubclass(Type potentialBase, Type potentialDescendant)
    {
        return potentialDescendant == potentialBase
               || potentialDescendant.IsSubclassOf(potentialBase)
               || potentialBase.IsAssignableFrom(potentialDescendant);
    }
}
