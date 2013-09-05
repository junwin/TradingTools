using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K2ServiceInterface
{
    public interface IPriceAgregator
    {
        KaiTrade.Interfaces.ITSSet TSSet { get; set; }
        void ProcessPrice(KaiTrade.Interfaces.IPXUpdate update);
    }
}
