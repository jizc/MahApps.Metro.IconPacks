#nullable enable
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace IconBrowser;

/// <summary>
/// A base class for all Models that can generate NotifyPropertyChanged events
/// </summary>
public abstract class NotifyPropertyChangedBase : INotifyPropertyChanged, INotifyPropertyChanging
{
    public event PropertyChangingEventHandler? PropertyChanging;
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Sets property if it does not equal existing value. Notifies listeners if change occurs.
    /// </summary>
    /// <typeparam name="T">Type of property.</typeparam>
    /// <param name="member">The property's backing field.</param>
    /// <param name="value">The new value.</param>
    /// <param name="propertyName">Name of the property used to notify listeners.  This
    /// value is optional and can be provided automatically when invoked from compilers
    /// that support <see cref="CallerMemberNameAttribute"/>.</param>
    [NotifyPropertyChangedInvocator]
    protected virtual bool SetProperty<T>([NotNullIfNotNull("value")] ref T member, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(member, value))
        {
            return false;
        }

        NotifyPropertyChanging(propertyName);
        member = value;
        NotifyPropertyChanged(propertyName);
        return true;
    }

    /// <summary>
    /// Notifies listeners that a property value is about to change.
    /// </summary>
    /// <param name="propertyName">Name of the property, used to notify listeners.</param>
    protected void NotifyPropertyChanging([CallerMemberName] string? propertyName = null)
    {
        var args = new PropertyChangingEventArgs(propertyName);
        OnPropertyChanging(args);
        PropertyChanging?.Invoke(this, args);
    }

    /// <summary>
    /// Notifies listeners that a property value has changed.
    /// </summary>
    /// <param name="propertyName">Name of the property, used to notify listeners.</param>
    [NotifyPropertyChangedInvocator]
    protected void NotifyPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        var args = new PropertyChangedEventArgs(propertyName);
        OnPropertyChanged(args);
        PropertyChanged?.Invoke(this, args);
    }

    protected virtual void OnPropertyChanging(PropertyChangingEventArgs e)
    {
    }

    protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
    {
    }
}
