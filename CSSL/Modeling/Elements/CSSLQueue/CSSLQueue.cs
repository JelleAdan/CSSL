﻿using CSSL.Modeling.Elements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSSL.Modeling.CSSLQueue
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">The class of the items which will be in the queue.</typeparam>
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
        /// Retrieves and removes the first item from the queue.
        /// </summary>
        /// <returns></returns>
        public T DequeueFirst()
        {
            T item = items.First();
            items.RemoveAt(0);
            item.MyQueue = null;
            return item;
        }

        /// <summary>
        /// Retrieves and removes the specified item from the queue.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public T Dequeue(T item)
        {
            items.Remove(item);
            return item;
        }

        /// <summary>
        /// Retrieves first item from the queue, but does not remove it from the queue.
        /// </summary>
        /// <returns></returns>
        public T PeekFirst()
        {
            return items.First();
        }

        /// <summary>
        /// Retrieves item from the queue at specified position, but does not remove it from the queue.
        /// </summary>
        /// <param name="index">Position of the item in the queue.</param>
        /// <returns></returns>
        public T PeekAt(int index)
        {
            return items[index];
        }

        protected override void OnReplicationStart()
        {
            items.Clear();
        }
    }
}
