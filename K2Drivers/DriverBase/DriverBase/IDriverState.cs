using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DriverBase
{
    public interface IDriverState
    {
        string ConfigPath { get; set; }
        bool AsyncPrices { get; set; }
        bool QueueReplaceRequests { get; set; }
    }
}
