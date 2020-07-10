using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NotificationApi.Domain.Enum
{
    public enum ConsoleType
    {
        [Display(Name = "PlayStation")]
        Playstation,

        [Display(Name = "Switch")]
        Switch,

        [Display(Name = "XBox")]
        XBox,
    }
}
