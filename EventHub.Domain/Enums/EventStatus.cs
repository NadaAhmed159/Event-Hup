using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Domain.Enums
{
    public enum EventStatus
    {

        Pending = 1,
        Approved = 2,
        Rejected = 3,
        Completed = 4
    }
}
