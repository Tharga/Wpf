using System.Collections.ObjectModel;

namespace Tharga.Wpf;

/// <summary>
/// An observable dictionary that supports WPF data binding by implementing both
/// <see cref="ObservableCollection{T}"/> and <see cref="IDictionary{TKey,TValue}"/>.
/// </summary>
/// <typeparam name="TKey">The type of dictionary keys.</typeparam>
/// <typeparam name="TValue">The type of dictionary values.</typeparam>
/// <remarks>
/// Every lookup compares keys with <see cref="EqualityComparer{T}.Default"/>.
/// Unlike <see cref="Dictionary{TKey,TValue}"/>, a <c>null</c> key is allowed when <typeparamref name="TKey"/> is a reference type, and it can be looked up, replaced and removed like any other key.
/// </remarks>
[Serializable]
public class ObservableDictionary<TKey, TValue> : ObservableCollection<ObservableKeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue>
{

    #region IDictionary<TKey,TValue> Members

    /// <inheritdoc />
    public void Add(TKey key, TValue value)
    {
        if (ContainsKey(key))
        {
            throw new ArgumentException("The dictionary already contains the key");
        }
        base.Add(new ObservableKeyValuePair<TKey, TValue>() { Key = key, Value = value });
    }

    /// <inheritdoc />
    public bool ContainsKey(TKey key)
    {
        //var m=base.FirstOrDefault((i) => i.Key == key);
        var r = ThisAsCollection().FirstOrDefault((i) => Equals(key, i.Key));

        return !Equals(default(ObservableKeyValuePair<TKey, TValue>), r);
    }

    bool Equals<T>(T a, T b)
    {
        return EqualityComparer<T>.Default.Equals(a, b);
    }

    private ObservableCollection<ObservableKeyValuePair<TKey, TValue>> ThisAsCollection()
    {
        return this;
    }

    /// <inheritdoc />
    public ICollection<TKey> Keys
    {
        get { return (from i in ThisAsCollection() select i.Key).ToList(); }
    }

    /// <inheritdoc />
    public bool Remove(TKey key)
    {
        var remove = ThisAsCollection().Where(pair => Equals(key, pair.Key)).ToList();
        foreach (var pair in remove)
        {
            ThisAsCollection().Remove(pair);
        }
        return remove.Count > 0;
    }

    /// <inheritdoc />
    public bool TryGetValue(TKey key, out TValue value)
    {
        value = default(TValue);
        var r = GetKvpByTheKey(key);
        if (Equals(r, default))
        {
            return false;
        }
        value = r.Value;
        return true;
    }

    private ObservableKeyValuePair<TKey, TValue> GetKvpByTheKey(TKey key)
    {
        return ThisAsCollection().FirstOrDefault((i) => Equals(key, i.Key));
    }

    /// <inheritdoc />
    public ICollection<TValue> Values
    {
        get { return (from i in ThisAsCollection() select i.Value).ToList(); }
    }

    /// <inheritdoc />
    public TValue this[TKey key]
    {
        get
        {
            TValue result;
            if (!TryGetValue(key, out result))
            {
                throw new ArgumentException("Key not found");
            }
            return result;
        }
        set
        {
            if (ContainsKey(key))
            {
                GetKvpByTheKey(key).Value = value;
            }
            else
            {
                Add(key, value);
            }
        }
    }

    #endregion

    #region ICollection<KeyValuePair<TKey,TValue>> Members

    /// <inheritdoc />
    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
    }

    /// <inheritdoc />
    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        var r = GetKvpByTheKey(item.Key);
        if (Equals(r, default(ObservableKeyValuePair<TKey, TValue>)))
        {
            return false;
        }
        return Equals(r.Value, item.Value);
    }

    /// <inheritdoc />
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        ArgumentNullException.ThrowIfNull(array);
        ArgumentOutOfRangeException.ThrowIfNegative(arrayIndex);
        if (array.Length - arrayIndex < Count)
        {
            throw new ArgumentException("The destination array does not have enough space from the index onward to hold every pair.");
        }

        foreach (var pair in ThisAsCollection())
        {
            array[arrayIndex++] = new KeyValuePair<TKey, TValue>(pair.Key, pair.Value);
        }
    }

    /// <inheritdoc />
    public bool IsReadOnly
    {
        get { return false; }
    }

    /// <inheritdoc />
    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        var r = GetKvpByTheKey(item.Key);
        if (Equals(r, default(ObservableKeyValuePair<TKey, TValue>)))
        {
            return false;
        }
        if (!Equals(r.Value, item.Value))
        {
            return false;
        }
        return ThisAsCollection().Remove(r);
    }

    #endregion

    #region IEnumerable<KeyValuePair<TKey,TValue>> Members

    /// <inheritdoc />
    public new IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return (from i in ThisAsCollection() select new KeyValuePair<TKey, TValue>(i.Key, i.Value)).ToList().GetEnumerator();
    }

    #endregion
}