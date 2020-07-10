using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationApi.Domain.Model
{
    public class Message
    {
        public string UniqueId { get; set; }
        public string GroupId { get; set; }
        public string Source { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
