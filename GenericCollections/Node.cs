using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericCollections
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class Node<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Node{T}"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public Node(T value) => this.Value = value;

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public T Value { get; }

        /// <summary>
        /// Gets or sets the next.
        /// </summary>
        /// <value>
        /// The next.
        /// </value>
        public Node<T> Next { get; set; }
    }
}
