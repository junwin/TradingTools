using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K2ServiceInterface
{
    public interface IFacade
    {
        IProductManager GetProductManager();
        KaiTrade.Interfaces.IPublisher CreatePxPub(KaiTrade.Interfaces.IProduct product);
        void AddProduct(string genericName, string tradeVenue);
        KaiTrade.Interfaces.IProduct AddProduct(string mnemonic, string Name, string mySecID, string myExchangeID, string ExDestination, string myCFICode, string myMMY, string myCurrency, double? strikePx, bool doEvent);
        string AppPath { get; set; }
        IFactory Factory { get; }
        void RequestProductDetails(KaiTrade.Interfaces.IProduct prod);
        IL1PX GetL1Prices(KaiTrade.Interfaces.IProduct product);
        void SetL1Prices(KaiTrade.Interfaces.IProduct product, IL1PX L1Price);

        KaiTrade.Interfaces.IDOM  GetDOM(KaiTrade.Interfaces.IProduct product);
        void SetDOM(KaiTrade.Interfaces.IProduct product, KaiTrade.Interfaces.IDOM DOM);
        
    }
}
