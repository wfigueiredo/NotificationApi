using NotificationApi.Domain.Dto;
using NotificationApi.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationApi.Domain.Model
{
    public class MessageDto
    {
        public string UniqueId { get; set; } = Guid.NewGuid().ToString();
        public string GroupId { get; set; }
        public string Source { get; set; }
        public Metadata Metadata { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string Content { get; set; }
    }
}
