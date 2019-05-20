using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoLib.entity
{
    public abstract class ElementBase
    {
        public static int _id;
        public ElementBase()
        {
            this.ID = System.Threading.Interlocked.Increment(ref _id);
            
        }
        /// <summary>
        /// Gets/sets an unique identifer.
        /// We need this to be write enabled because of Entity Framework
        /// </summary>
        public int ID { get; set; }
        public override string ToString()
        {
            return $"ID={this.ID}";
        }
    }
}
