// Copyright (c) 2019-2021 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SoftCircuits.Silk
{
    /// <summary>
    /// Implements a dictionary that also manages an ordered, indexable
    /// list of its items.
    /// </summary>
    /// <remarks>
    /// To minimize data duplication, only the index into the list is stored
    /// in the dictionary with the key.
    /// </remarks>
    internal class OrderedDictionary<TKey, TValue> : IEnumerable<TValue> where TKey : notnull
    {
        private readonly Dictionary<TKey, int> IndexLookup;
        private readonly List<TValue> Items;

        public OrderedDictionary()
        {
            IndexLookup = new Dictionary<TKey, int>();
            Items = new List<TValue>();
        }

        public OrderedDictionary(IEqualityComparer<TKey> comparer)
        {
            IndexLookup = new Dictionary<TKey, int>(comparer);
            Items = new List<TValue>();
        }

        public int Add(TKey key, TValue value)
        {
            int index = Items.Count;
            IndexLookup.Add(key, index);
            Items.Add(value);
            return index;
        }

        public void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> collection)
        {
            foreach (var pair in collection)
                Add(pair.Key, pair.Value);
        }

        public void AddRange(OrderedDictionary<TKey, TValue> collection)
        {
            AddRange(collection.GetKeyValuePairs());
        }

        public TValue this[int index]
        {
            get => Items[index];
            set => Items[index] = value;
        }

        public TValue this[TKey key]
        {
            get => Items[IndexLookup[key]];
            set => Items[IndexLookup[key]] = value;
        }

#if NETSTANDARD2_0
        public bool TryGetValue(TKey key, out TValue value)
#else
        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
#endif
        {
            if (IndexLookup.TryGetValue(key, out int index))
            {
                value = Items[index];
                return true;
            }
            value = default;
            return false;
        }

        public void Clear()
        {
            Items.Clear();
            IndexLookup.Clear();
        }

        public int Count => Items.Count;

        public bool ContainsKey(TKey key) => IndexLookup.ContainsKey(key);

        public int IndexOf(TKey key) => IndexLookup.TryGetValue(key, out int index) ? index : -1;

        public List<TKey> Keys => new(IndexLookup.Keys);

        public List<TValue> Values => new(Items);      // Don't return original list

        public IEnumerable<KeyValuePair<TKey, TValue>> GetKeyValuePairs() => IndexLookup.Keys.Select(k => new KeyValuePair<TKey, TValue>(k, Items[IndexLookup[k]]));

        #region IEnumerable

        public IEnumerator<TValue> GetEnumerator() => Items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Items.GetEnumerator();

        #endregion

    }
}
