using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K2ServiceInterface
{
    public interface IDriverState
    {
        string ConfigPath { get; set; }
        bool AsyncPrices { get; set; }
        bool QueueReplaceRequests { get; set; }
        bool HideDriverUI { get; set; }
    }
}
