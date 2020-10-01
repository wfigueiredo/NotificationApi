using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationApi.Domain.Enum
{
    public enum ChannelType
    {
        [Display(Name = "sqs")]
        Sqs,

        [Display(Name = "sns")]
        Sns,

        [Display(Name = "masstransit")]
        MassTransit,

        [Display(Name = "rabbit")]
        Rabbit
    }
}
