using System.ComponentModel;

namespace Tharga.Wpf;

/// <summary>
/// An observable key-value pair that raises <see cref="INotifyPropertyChanged"/> when the key or value changes.
/// </summary>
/// <typeparam name="TKey">The type of the key.</typeparam>
/// <typeparam name="TValue">The type of the value.</typeparam>
[Serializable]
public class ObservableKeyValuePair<TKey, TValue> : INotifyPropertyChanged
{
    #region properties
    private TKey key;
    private TValue value;

    /// <summary>Gets or sets the key.</summary>
    public TKey Key
    {
        get { return key; }
        set
        {
            key = value;
            OnPropertyChanged("Key");
        }
    }

    /// <summary>Gets or sets the value.</summary>
    public TValue Value
    {
        get { return value; }
        set
        {
            this.value = value;
            OnPropertyChanged("Value");
        }
    }
    #endregion

    #region INotifyPropertyChanged Members

    /// <inheritdoc />
    [field: NonSerialized]
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="name">The name of the property that changed.</param>
    public void OnPropertyChanged(string name)
    {
        PropertyChangedEventHandler handler = PropertyChanged;
        if (handler != null) handler(this, new PropertyChangedEventArgs(name));
    }

    #endregion
}