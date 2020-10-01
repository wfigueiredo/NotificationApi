using NotificationApi.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationApi.Domain.Dto
{
    public class Metadata
    {
        public ChannelType channel { get; set; }
        public string queue { get; set; }
    }
}
