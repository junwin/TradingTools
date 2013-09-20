using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K2ServiceInterface
{
    public interface IPriceHandler
    {
        /// <summary>
        /// Apply a price update to any publishers for the
        /// product being updated
        /// </summary>
        /// <param name="update"></param>
        void ApplyPriceUpdate(KaiTrade.Interfaces.IPXUpdate update);
        void ApplyDOMUpdate(KaiTrade.Interfaces.IProduct product, KaiTrade.Interfaces.IDOMSlot[] slots);
        void ApplyDOMUpdate(KaiTrade.Interfaces.IProduct product, KaiTrade.Interfaces.IPXUpdate update);

        KaiTrade.Interfaces.IPublisher CreatePxPub(KaiTrade.Interfaces.IProduct product);
        KaiTrade.Interfaces.IPublisher GetPXPublisher(KaiTrade.Interfaces.IProduct product);

        KaiTrade.Interfaces.IDOM GetProductDOM(KaiTrade.Interfaces.IProduct product, decimal startPx);
        KaiTrade.Interfaces.IDOM GetProductDOM(KaiTrade.Interfaces.IProduct product);
    }
}
