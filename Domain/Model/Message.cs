using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationApi.Domain.Model
{
    [Table(name: "message")]
    public class Message : AbstractEntity<Message, long>
    {
        [Column(name: "uniqueid")]
        public string UniqueId { get; set; }
        
        [Column(name: "groupid")]
        public string GroupId { get; set; }

        [Column(name: "source")]
        public string Source { get; set; }

        [Column(name: "content")]
        public string Content { get; set; }

        [Column(name: "createdat")]
        public DateTime CreatedAt { get; set; }
    }
}
