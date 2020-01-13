using CSSL.Modeling.Elements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSSL.Modeling.CSSLQueue
{
    public class CSSLQueue<CSSLQueueObject> : ModelElement
    {
        public CSSLQueue(ModelElement parent, string name) : base(parent, name)
        {
        }

        /// <summary>
        /// List of items in the queue.
        /// </summary>
        protected List<CSSLQueueObject> Items;

        /// <summary>
        /// Adds an item to the end of the queue.
        /// </summary>
        /// <param name="item">The item to be added.</param>
        public void EnqueueLast(CSSLQueueObject item)
        {
            Items.Add(item);
        }

        /// <summary>
        /// Retrieves and removes an item at a specified position from the queue. 
        /// </summary>
        /// <param name="index">Position of the item in the queue.</param>
        /// <returns></returns>
        public CSSLQueueObject DequeueAt(int index)
        {
            CSSLQueueObject item = Items[index];
            Items.RemoveAt(index);
            return item;
        }

        /// <summary>
        /// Retireves and removes the first item from the queue.
        /// </summary>
        /// <returns></returns>
        public CSSLQueueObject DequeueFirst()
        {
            CSSLQueueObject item = Items.First();
            Items.RemoveAt(0);
            return item;
        }
    }
}
