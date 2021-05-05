using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Data;
using System.ComponentModel;

namespace DictionaryHelper
{
    public abstract class ObservableDictionaryBase<T1, T2> : IDictionary<T1, T2>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        protected IDictionary<T1, T2> dict; // Inner dictionary

        // INotify interfaces
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnCollectionChanged(NotifyCollectionChangedEventArgs e) => CollectionChanged?.Invoke(this, e);
        protected void OnPropertyChanged(string propName) => PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propName));

        // IDictionary interface
        public abstract bool IsReadOnly { get; }
        public abstract T2 this[T1 key] { get; set; }
        ICollection<T1> IDictionary<T1, T2>.Keys => Keys;
        ICollection<T2> IDictionary<T1, T2>.Values => Values;

        public ReadOnlyObservableCollection<T1> Keys { get; protected set; }
        public ReadOnlyObservableCollection<T2> Values { get; protected set; }
        public int Count => dict.Count;


        public ObservableDictionaryBase(IDictionary<T1, T2> innerDict = null) =>
            dict = innerDict ?? new Dictionary<T1, T2>();

        public void Add(T1 key, T2 value) => Add(new KeyValuePair<T1, T2>(key, value));
        public abstract void Add(KeyValuePair<T1, T2> item);
        public bool Remove(T1 key) => ContainsKey(key) ? Remove(new KeyValuePair<T1, T2>(key, dict[key])) : false;
        public abstract bool Remove(KeyValuePair<T1, T2> item);
        public abstract void Clear();

        public bool TryGetValue(T1 key, out T2 value) => dict.TryGetValue(key, out value);
        public bool Contains(KeyValuePair<T1, T2> item) => dict.Contains(item);
        public bool ContainsKey(T1 key) => dict.ContainsKey(key);
        public void CopyTo(KeyValuePair<T1, T2>[] array, int arrayIndex) => dict.CopyTo(array, arrayIndex);

        public IEnumerator<KeyValuePair<T1, T2>> GetEnumerator() => dict.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
    public class ObservableDictionary<T1, T2> : ObservableDictionaryBase<T1, T2>
    {
        // IDictionary interface
        public override bool IsReadOnly => false;
        public override T2 this[T1 key] { get => dict[key]; set => Add(key, value); }

        public ObservableDictionary(IDictionary<T1, T2> innerDict = null)
        {
            dict = innerDict ?? new Dictionary<T1, T2>();
            var keys = new ObservableCollection<T1>(dict.Keys);
            var values = new ObservableCollection<T2>(dict.Values);
            Keys = new ReadOnlyObservableCollection<T1>(keys);
            Values = new ReadOnlyObservableCollection<T2>(values);
            CollectionChanged += ObservableDictionary_CollectionChanged;

            void ObservableDictionary_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                OnPropertyChanged(nameof(Count));
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        foreach (var ni in e.NewItems.Cast<KeyValuePair<T1, T2>>())
                        {
                            keys.Add(ni.Key);
                            values.Add(ni.Value);
                        }
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        foreach (var oi in e.OldItems.Cast<KeyValuePair<T1, T2>>())
                        {
                            keys.Remove(oi.Key);
                            values.Remove(oi.Value);
                        }
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        keys.Clear();
                        values.Clear();
                        break;
                    default:
                        break;
                }
            }
        }

        public override void Add(KeyValuePair<T1, T2> item)
        {
            Remove(item.Key);
            dict.Add(item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }
        public override bool Remove(KeyValuePair<T1, T2> item)
        {
            var index = dict.Contains(item) ? Enumerable.Range(0, dict.Count).First(i => dict.ElementAt(i).Equals(item)) : -1;
            if (dict.Remove(item))
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
                return true;
            }
            return false;
        }
        public override void Clear()
        {
            dict.Clear();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
    public class ReadOnlyObservableDictionary<T1, T2> : ObservableDictionaryBase<T1, T2>
    {
        // IDictionary interface
        protected ObservableDictionary<T1, T2> InnerDict => (ObservableDictionary<T1, T2>)dict;
        public override bool IsReadOnly => false;
        public override T2 this[T1 key] { get => dict[key]; set => Add(key, value); }

        public ReadOnlyObservableDictionary(ObservableDictionary<T1, T2> innerDict = null)
        {
            dict = innerDict ?? new ObservableDictionary<T1, T2>();
            InnerDict.CollectionChanged += InnerDict_CollectionChanged;
            InnerDict.PropertyChanged += InnerDict_PropertyChanged;
            void InnerDict_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) => OnCollectionChanged(e);
            void InnerDict_PropertyChanged(object sender, PropertyChangedEventArgs e) => OnPropertyChanged(e.PropertyName);
            Keys = InnerDict.Keys;
            Values = InnerDict.Values;
        }

        public override void Add(KeyValuePair<T1, T2> item) => dict.Add(item);
        public override bool Remove(KeyValuePair<T1, T2> item) => dict.Remove(item);
        public override void Clear() => dict.Clear();
    }
}
