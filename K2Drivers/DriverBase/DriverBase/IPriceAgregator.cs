using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DriverBase
{
    public interface IPriceAgregator
    {
        KaiTrade.Interfaces.ITSSet TSSet { get; set; }
        void ProcessPrice(KaiTrade.Interfaces.IPXUpdate update);
    }
}
