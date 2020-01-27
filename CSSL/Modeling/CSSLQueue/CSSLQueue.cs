using CSSL.Modeling.Elements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSSL.Modeling.CSSLQueue
{
    public class CSSLQueue<T> : ModelElementBase where T : CSSLQueueObject<T>
    {
        public CSSLQueue(ModelElementBase parent, string name) : base(parent, name)
        {
            items = new List<T>();
        }

        /// <summary>
        /// List of items in the queue.
        /// </summary>
        protected List<T> items;

        /// <summary>
        /// Returns the current length of the queue.
        /// </summary>
        public int Length => items.Count;

        /// <summary>
        /// Adds an item to the end of the queue.
        /// </summary>
        /// <param name="item">The item to be added.</param>
        public void EnqueueLast(T item)
        {
            items.Add(item);
            item.MyQueue = this;
        }

        /// <summary>
        /// Retrieves and removes an item at a specified position from the queue. 
        /// </summary>
        /// <param name="index">Position of the item in the queue.</param>
        /// <returns></returns>
        public T DequeueAt(int index)
        {
            T item = items[index];
            items.RemoveAt(index);
            item.MyQueue = null;
            return item;
        }



        /// <summary>
        /// Retireves and removes the first item from the queue.
        /// </summary>
        /// <returns></returns>
        public T DequeueFirst()
        {
            T item = items.First();
            items.RemoveAt(0);
            item.MyQueue = null;
            return item;
        }
    }
}
