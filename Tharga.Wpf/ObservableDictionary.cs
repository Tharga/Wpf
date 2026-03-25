using System.Collections.ObjectModel;

namespace Tharga.Wpf;

/// <summary>
/// An observable dictionary that supports WPF data binding by implementing both
/// <see cref="ObservableCollection{T}"/> and <see cref="IDictionary{TKey,TValue}"/>.
/// </summary>
/// <typeparam name="TKey">The type of dictionary keys.</typeparam>
/// <typeparam name="TValue">The type of dictionary values.</typeparam>
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

    bool Equals<TKey>(TKey a, TKey b)
    {
        return EqualityComparer<TKey>.Default.Equals(a, b);
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
        return ThisAsCollection().FirstOrDefault((i) => i.Key.Equals(key));
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
        throw new NotImplementedException();
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