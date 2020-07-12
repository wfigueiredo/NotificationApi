using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationApi.Domain.Model
{
    public abstract class AbstractEntity<T,K> where T : class
    {
        public virtual K id { get; set; }
    }
}
