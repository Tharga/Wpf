using System.Collections.Specialized;

namespace Tharga.Wpf.Tests;

public class ObservableDictionaryTests
{
    [Fact]
    public void Add_Stores_The_Value_Under_The_Key()
    {
        var dictionary = new ObservableDictionary<string, int> { { "a", 1 } };

        Assert.Equal(1, dictionary["a"]);
        Assert.Single(dictionary);
    }

    [Fact]
    public void Add_Throws_When_The_Key_Exists()
    {
        var dictionary = new ObservableDictionary<string, int> { { "a", 1 } };

        Assert.Throws<ArgumentException>(() => dictionary.Add("a", 2));
        Assert.Equal(1, dictionary["a"]);
    }

    [Fact]
    public void Add_Raises_CollectionChanged()
    {
        var dictionary = new ObservableDictionary<string, int>();
        NotifyCollectionChangedAction? action = null;
        dictionary.CollectionChanged += (_, e) => action = e.Action;

        dictionary.Add("a", 1);

        Assert.Equal(NotifyCollectionChangedAction.Add, action);
    }

    [Fact]
    public void ContainsKey_Reports_Whether_The_Key_Exists()
    {
        var dictionary = new ObservableDictionary<int, string> { { 1, "one" } };

        Assert.True(dictionary.ContainsKey(1));
        Assert.False(dictionary.ContainsKey(2));
    }

    [Fact]
    public void TryGetValue_Returns_The_Value_For_An_Existing_Key()
    {
        var dictionary = new ObservableDictionary<string, int> { { "a", 1 } };

        var found = dictionary.TryGetValue("a", out var value);

        Assert.True(found);
        Assert.Equal(1, value);
    }

    [Fact]
    public void TryGetValue_Returns_False_For_A_Missing_Key()
    {
        var dictionary = new ObservableDictionary<string, int> { { "a", 1 } };

        var found = dictionary.TryGetValue("b", out var value);

        Assert.False(found);
        Assert.Equal(default, value);
    }

    [Fact]
    public void Indexer_Get_Throws_For_A_Missing_Key()
    {
        var dictionary = new ObservableDictionary<string, int> { { "a", 1 } };

        Assert.Throws<ArgumentException>(() => dictionary["missing"]);
    }

    [Fact]
    public void Indexer_Set_Replaces_The_Value_Of_An_Existing_Key()
    {
        var dictionary = new ObservableDictionary<string, int> { { "a", 1 } };

        dictionary["a"] = 2;

        Assert.Equal(2, dictionary["a"]);
        Assert.Single(dictionary);
    }

    [Fact]
    public void Indexer_Set_Adds_A_Missing_Key()
    {
        var dictionary = new ObservableDictionary<string, int>();

        dictionary["a"] = 1;

        Assert.Equal(1, dictionary["a"]);
    }

    [Fact]
    public void Remove_By_Key_Removes_The_Pair()
    {
        var dictionary = new ObservableDictionary<string, int> { { "a", 1 }, { "b", 2 } };

        var removed = dictionary.Remove("a");

        Assert.True(removed);
        Assert.False(dictionary.ContainsKey("a"));
        Assert.True(dictionary.ContainsKey("b"));
    }

    [Fact]
    public void Remove_By_Key_Returns_False_For_A_Missing_Key()
    {
        var dictionary = new ObservableDictionary<string, int> { { "a", 1 } };

        Assert.False(dictionary.Remove("b"));
        Assert.Single(dictionary);
    }

    [Fact]
    public void Contains_Pair_Requires_Both_Key_And_Value_To_Match()
    {
        var dictionary = new ObservableDictionary<string, int> { { "a", 1 } };

        Assert.True(dictionary.Contains(new KeyValuePair<string, int>("a", 1)));
        Assert.False(dictionary.Contains(new KeyValuePair<string, int>("a", 2)));
        Assert.False(dictionary.Contains(new KeyValuePair<string, int>("b", 1)));
    }

    [Fact]
    public void Remove_Pair_Only_Removes_When_The_Value_Matches()
    {
        var dictionary = new ObservableDictionary<string, int> { { "a", 1 } };

        Assert.False(dictionary.Remove(new KeyValuePair<string, int>("a", 2)));
        Assert.True(dictionary.ContainsKey("a"));

        Assert.True(dictionary.Remove(new KeyValuePair<string, int>("a", 1)));
        Assert.False(dictionary.ContainsKey("a"));
    }

    [Fact]
    public void Keys_And_Values_List_The_Contents_In_Insertion_Order()
    {
        var dictionary = new ObservableDictionary<string, int> { { "a", 1 }, { "b", 2 } };

        Assert.Equal(new[] { "a", "b" }, dictionary.Keys);
        Assert.Equal(new[] { 1, 2 }, dictionary.Values);
    }

    [Fact]
    public void Enumerating_As_A_Dictionary_Yields_KeyValuePairs()
    {
        var dictionary = new ObservableDictionary<string, int> { { "a", 1 }, { "b", 2 } };

        var pairs = new List<KeyValuePair<string, int>>();
        foreach (var pair in (IEnumerable<KeyValuePair<string, int>>)dictionary)
        {
            pairs.Add(pair);
        }

        Assert.Equal(new[] { new KeyValuePair<string, int>("a", 1), new KeyValuePair<string, int>("b", 2) }, pairs);
    }

    private static readonly KeyValuePair<string, int>[] ExpectedPairs =
    [
        new KeyValuePair<string, int>("a", 1),
        new KeyValuePair<string, int>("b", 2)
    ];

    private static ObservableDictionary<string, int> CreateTwoPairDictionary()
    {
        return new ObservableDictionary<string, int> { { "a", 1 }, { "b", 2 } };
    }

    [Fact]
    public void ToList_Returns_Every_Pair()
    {
        var dictionary = CreateTwoPairDictionary();

        var list = dictionary.ToList<KeyValuePair<string, int>>();

        Assert.Equal(ExpectedPairs, list);
    }

    [Fact]
    public void ToArray_Returns_Every_Pair()
    {
        var dictionary = CreateTwoPairDictionary();

        var array = dictionary.ToArray<KeyValuePair<string, int>>();

        Assert.Equal(ExpectedPairs, array);
    }

    [Fact]
    public void List_Constructor_Copies_Every_Pair()
    {
        var dictionary = CreateTwoPairDictionary();

        var list = new List<KeyValuePair<string, int>>(dictionary);

        Assert.Equal(ExpectedPairs, list);
    }

    [Fact]
    public void CopyTo_Copies_The_Pairs_From_The_Index()
    {
        ICollection<KeyValuePair<string, int>> collection = CreateTwoPairDictionary();
        var array = new KeyValuePair<string, int>[4];

        collection.CopyTo(array, 1);

        Assert.Equal(new[] { default, ExpectedPairs[0], ExpectedPairs[1], default }, array);
    }

    [Fact]
    public void CopyTo_Throws_For_A_Null_Array()
    {
        ICollection<KeyValuePair<string, int>> collection = CreateTwoPairDictionary();

        Assert.Throws<ArgumentNullException>(() => collection.CopyTo(null!, 0));
    }

    [Fact]
    public void CopyTo_Throws_For_A_Negative_Index()
    {
        ICollection<KeyValuePair<string, int>> collection = CreateTwoPairDictionary();

        Assert.Throws<ArgumentOutOfRangeException>(() => collection.CopyTo(new KeyValuePair<string, int>[2], -1));
    }

    [Theory]
    [InlineData(1, 0)]
    [InlineData(2, 1)]
    [InlineData(2, 3)]
    public void CopyTo_Throws_When_The_Array_Has_Too_Little_Space_From_The_Index(int arrayLength, int arrayIndex)
    {
        ICollection<KeyValuePair<string, int>> collection = CreateTwoPairDictionary();
        var array = new KeyValuePair<string, int>[arrayLength];

        Assert.Throws<ArgumentException>(() => collection.CopyTo(array, arrayIndex));
        Assert.All(array, pair => Assert.Equal(default, pair));
    }

    private static ObservableDictionary<string?, int> CreateDictionaryWithNullKeyFirst()
    {
        return new ObservableDictionary<string?, int> { { null, 1 }, { "a", 2 } };
    }

    [Fact]
    public void TryGetValue_Finds_A_Null_Key()
    {
        var dictionary = CreateDictionaryWithNullKeyFirst();

        var found = dictionary.TryGetValue(null, out var value);

        Assert.True(found);
        Assert.Equal(1, value);
    }

    [Fact]
    public void Lookups_Of_Other_Keys_Pass_A_Stored_Null_Key()
    {
        var dictionary = CreateDictionaryWithNullKeyFirst();

        Assert.True(dictionary.TryGetValue("a", out var value));
        Assert.Equal(2, value);
        Assert.Equal(2, dictionary["a"]);
        Assert.False(dictionary.TryGetValue("missing", out _));
    }

    [Fact]
    public void Indexer_Gets_And_Replaces_The_Value_Of_A_Null_Key()
    {
        var dictionary = CreateDictionaryWithNullKeyFirst();

        dictionary[null] = 3;

        Assert.Equal(3, dictionary[null]);
        Assert.Equal(2, dictionary.Count);
    }

    [Fact]
    public void Contains_And_Remove_Pair_Handle_A_Null_Key()
    {
        var dictionary = CreateDictionaryWithNullKeyFirst();

        Assert.True(dictionary.Contains(new KeyValuePair<string?, int>(null, 1)));
        Assert.False(dictionary.Contains(new KeyValuePair<string?, int>(null, 2)));
        Assert.True(dictionary.Remove(new KeyValuePair<string?, int>(null, 1)));
        Assert.False(dictionary.ContainsKey(null));
        Assert.True(dictionary.ContainsKey("a"));
    }
}
