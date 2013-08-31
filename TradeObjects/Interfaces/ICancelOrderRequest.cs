using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaiTrade.Interfaces
{
    public interface ICancelOrderRequest
    {
        string Mnemonic { get; set; }
        string ClOrdID { get; set; }
        string OrigClOrdID { get; set; }
        int? RetryCount { get; set; }
    }
}
