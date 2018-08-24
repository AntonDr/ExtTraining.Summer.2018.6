using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericCollections
{
    public class Set<T>:IEnumerable<T>,ISet<T>
    {
        #region Contants
        /// <summary>
        /// The default capacity
        /// </summary>
        private const int DEFAULT_CAPACITY = 3;
        /// <summary>
        /// The comparer
        /// </summary>
        private readonly IEqualityComparer<T> comparer;
        #endregion

        #region Private fields

        /// <summary>
        /// The buckets
        /// </summary>
        private Node<T>[] buckets;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Set{T}"/> class.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        /// <param name="comparer">The comparer.</param>
        /// <exception cref="ArgumentNullException">comparer</exception>
        /// <exception cref="ArgumentException">capacity</exception>
        public Set(int capacity, EqualityComparer<T> comparer)
        {
            this.comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));

            if (capacity <= 0)
            {
                throw new ArgumentException($"{nameof(capacity)} can not be non-positive");
            }

            buckets = new Node<T>[capacity];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Set{T}"/> class.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="comparer">The comparer.</param>
        /// <exception cref="ArgumentNullException">
        /// comparer
        /// or
        /// values
        /// </exception>
        public Set(IEnumerable<T> values, EqualityComparer<T> comparer)
        {
            this.comparer = comparer ?? throw new ArgumentNullException(nameof(comparer));

            if (values == null)
            {
                throw new ArgumentNullException($"{nameof(values)} can not be null");
            }

            buckets = new Node<T>[GetNextPrimeNumber(values.Count())];

            foreach (var item in values)
            {
                Add(item);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Set{T}"/> class.
        /// </summary>
        public Set() : this(DEFAULT_CAPACITY, EqualityComparer<T>.Default) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Set{T}"/> class.
        /// </summary>
        /// <param name="values">The values.</param>
        public Set(IEnumerable<T> values) : this(values, EqualityComparer<T>.Default) { }
        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
        /// </summary>
        public bool IsReadOnly { get; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        private int Version { get; set; }

        /// <summary>
        /// Gets the capacity.
        /// </summary>
        /// <value>
        /// The capacity.
        /// </value>
        public int Capacity => buckets.Length;

        #endregion

        #region Public methods

        /// <summary>
        /// Determines whether [contains] [the specified value].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if [contains] [the specified value]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(T value)
        {
            int bucketIndex = GetBucketIndex(value);

            Node<T> currentNode = buckets[bucketIndex];

            while (currentNode != null)
            {
                if (comparer.Equals(currentNode.Value, value))
                {
                    return true;
                }

                currentNode = currentNode.Next;
            }

            return false;
        }

        /// <summary>
        /// Adds the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public bool Add(T value)
        {
            if (Count >= Capacity)
            {
                int newSize = GetNextPrimeNumber(Count);
                Array.Resize(ref buckets, newSize);
            }

            int bucketIndex = GetBucketIndex(value);

            if (buckets[bucketIndex] == null)
            {
                buckets[bucketIndex] = new Node<T>(value);
            }
            else
            {
                Node<T> currentNode = this.buckets[bucketIndex];
                Node<T> parentNode = this.buckets[bucketIndex];
                while (currentNode != null)
                {
                    if (Contains(value))
                    {
                        return false;
                    }

                    parentNode = currentNode;
                    currentNode = currentNode.Next;
                }

                parentNode.Next = new Node<T>(value);
            }
       
            Count++;
            Version++;
            return true;
        }

        /// <summary>
        /// Removes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public bool Remove(T value)
        {
            if (!this.Contains(value))
            {
                return false;
            }

            int bucketIndex = GetBucketIndex(value);

            if (comparer.Equals(this.buckets[bucketIndex].Value, value))
            {
                this.buckets[bucketIndex] = null;
                Count--;
                Version++;
                return true;
            }

            var parentNode = buckets[bucketIndex];
            var currentNode = buckets[bucketIndex];

            while (currentNode != null)
            {
                if (comparer.Equals(currentNode.Value, value))
                {
                    parentNode.Next = currentNode.Next;
                    Count--;
                    Version++;
                    return true;
                }

                parentNode = currentNode;
                currentNode = currentNode.Next;
            }

            Count--;
            Version++;
            return true;
        }

        /// <summary>
        /// Modifies the current set so that it contains all elements that are present in the current set, in the specified collection, or in both.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <exception cref="ArgumentNullException">other</exception>
        public void UnionWith(IEnumerable<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException($"{nameof(other)} can not be null");
            }

            foreach (var item in other)
            {
               this.Add(item);
            }

            Version++;
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        public void Clear()
        {
            if (Count == 0)
            {
                return;
            }

            Array.Clear(buckets, 0, Capacity);
            this.Count = 0;
            Version++;
        }
        #endregion

        #region Static methods

        /// <summary>
        /// Unions the specified LHS.
        /// </summary>
        /// <param name="lhs">The LHS.</param>
        /// <param name="rhs">The RHS.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Set<T> Union(Set<T> lhs, Set<T> rhs)
        {
            if (lhs == null || rhs == null)
            {
                throw new ArgumentNullException($"Operands can not be null");
            }

            foreach (var item in lhs)
            {
                rhs.Add(item);
            }

            return rhs;
        }

        /// <summary>
        /// Determines whether the specified number is prime.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns>
        ///   <c>true</c> if the specified number is prime; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsPrime(int number)
        {
            if (number < 2)
            {
                return false;
            }

            for (int i = 2; i <= (int) Math.Sqrt(number); i++)
            {
                if (number % i == 0)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Gets the next prime number.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns></returns>
        private static int GetNextPrimeNumber(int number)
        {
            while (true)
            {
                if (IsPrime(number))
                {
                    return number;
                }

                number++;
            }
        }

        #endregion

        #region Interface members

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        /// <exception cref="InvalidOperationException">Collecttion can not be change when enumerating</exception>
        public IEnumerator<T> GetEnumerator()
        {
            int version = Version;

            foreach (var item in buckets)
            {
                if (version != Version)
                {
                    throw new InvalidOperationException("Collecttion can not be change when enumerating");
                }

                if (item != null)
                {
                    var currentNode = item;

                    while (currentNode != null)
                    {
                        yield return currentNode.Value;
                        currentNode = currentNode.Next;
                    }

                }

            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Modifies the current set so that it contains only elements that are also in a specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <exception cref="NotImplementedException"></exception>
        public void IntersectWith(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes all elements in the specified collection from the current set.
        /// </summary>
        /// <param name="other">The collection of items to remove from the set.</param>
        /// <exception cref="NotImplementedException"></exception>
        public void ExceptWith(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Modifies the current set so that it contains only elements that are present either in the current set or in the specified collection, but not both.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <exception cref="NotImplementedException"></exception>
        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines whether a set is a subset of a specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <returns>
        ///   <see langword="true" /> if the current set is a subset of <paramref name="other" />; otherwise, <see langword="false" />.
        /// </returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool IsSubsetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines whether the current set is a superset of a specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <returns>
        ///   <see langword="true" /> if the current set is a superset of <paramref name="other" />; otherwise, <see langword="false" />.
        /// </returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool IsSupersetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines whether the current set is a proper (strict) superset of a specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <returns>
        ///   <see langword="true" /> if the current set is a proper superset of <paramref name="other" />; otherwise, <see langword="false" />.
        /// </returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines whether the current set is a proper (strict) subset of a specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <returns>
        ///   <see langword="true" /> if the current set is a proper subset of <paramref name="other" />; otherwise, <see langword="false" />.
        /// </returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines whether the current set overlaps with the specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <returns>
        ///   <see langword="true" /> if the current set and <paramref name="other" /> share at least one common element; otherwise, <see langword="false" />.
        /// </returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool Overlaps(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines whether the current set and the specified collection contain the same elements.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <returns>
        ///   <see langword="true" /> if the current set is equal to <paramref name="other" />; otherwise, false.
        /// </returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool SetEquals(IEnumerable<T> other)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1" /> to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        /// <exception cref="NotImplementedException"></exception>
        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <exception cref="NotImplementedException"></exception>
        void ICollection<T>.Add(T item)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Private methods

        /// <summary>
        /// Gets the index of the bucket.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private int GetBucketIndex(T value) => Math.Abs(value.GetHashCode() % Capacity);

        #endregion
    }
}
