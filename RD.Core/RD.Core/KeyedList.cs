using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.ComponentModel;
using System.Reflection;

namespace RDCore
{
    public class KeyedList<K, T> : Dictionary<K, T>, IEnumerable<T>
    {
        private Func<T, K> LinkedValue;

        private Expression<Func<T, K>> _LinkedValueExpression;
        public Expression<Func<T, K>> LinkedValueExpression
        {
            get { return _LinkedValueExpression; }
            set
            {
                _LinkedValueExpression = value;
                LinkedValue = (value == null) ? null : _LinkedValueExpression.Compile();
            }
        }

        public KeyedList()
        {
            LinkedValueExpression = null;
        }

        public KeyedList(Expression<Func<T, K>> LinkedValueExpression)
        {
            this.LinkedValueExpression = LinkedValueExpression;
        }

        public void Add(T item)
        {
            if (LinkedValue == null)
                throw new InvalidOperationException("Can't call KeyedList<K, T>.Add(T) " +
                    "unless a LinkedValue function has been assigned");

            Add(LinkedValue(item), item);
        }

        public new void Add(K key, T item)
        {
            base.Add(key, item);

            if (typeof(INotifyPropertyChanged).IsAssignableFrom(typeof(T)))
                (this[key] as INotifyPropertyChanged).PropertyChanged += new PropertyChangedEventHandler(KeyList_PropertyChanged);
        }

        private void KeyList_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var lambda = LinkedValueExpression as LambdaExpression;
            if (lambda == null)
                return;

            var expr = lambda.Body as MemberExpression;
            if (expr == null)
                return;

            MemberInfo[] members = typeof(T).GetMember(e.PropertyName,
                MemberTypes.Property | MemberTypes.Field,
                BindingFlags.Instance | BindingFlags.Public);
            if (members.Length == 0)
                throw new ApplicationException("Field or property " + e.PropertyName + " not found in type " + typeof(T).FullName);

            MemberInfo mi = members[0];
            if (mi == expr.Member)
            {
                // we don't know what the old key was, so we have to find the object in the dictionary
                // then remove it and re-add it
                foreach (var kvp in KeyValuePairs)
                {
                    if ((typeof(T).IsValueType && kvp.Value.Equals(sender))
                        || (!typeof(T).IsValueType && (kvp.Value as object) == (sender as object)))
                    {
                        T item = this[kvp.Key];
                        Remove(kvp.Key);
                        Add(item);
                        return;
                    }
                }
            }
        }

        public new IEnumerator<T> GetEnumerator()
        {
            foreach (var item in Values)
            {
                yield return item;
            }
            yield break;
        }

        public IEnumerable<KeyValuePair<K, T>> KeyValuePairs
        {
            get
            {
                // because GetEnumerator is shadowed (to provide the more intuitive IEnumerable<T>), 
                // and foreach'ing over "base" isn't allowed,
                // we use a Dictionary variable pointing to "this"
                // so we can use its IEnumerable<KeyValuePair<K, T>>
                Dictionary<K, T> dict = this;
                foreach (var kvp in dict)
                {
                    yield return kvp;
                }
                yield break;
            }
        }
    }
}
