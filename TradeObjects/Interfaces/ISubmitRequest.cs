using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaiTrade.Interfaces
{
    public interface ISubmitRequest
    {

        long OrderQty { get; set; }
        string OrdType { get; set; }
        decimal? Price { get; set; }
        decimal? StopPx { get; set; }
        string ClOrdID { get; set; }
        string Side { get; set; }
        string Mnemonic { get; set; }
        string SecurityID { get; set; }
        string Account { get; set; }

    }
}
