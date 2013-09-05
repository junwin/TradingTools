using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DriverBase
{
    public interface IFacade
    {
        ProductManager GetProductManager();
        void AddProduct(string genericName, string tradeVenue);
        KaiTrade.Interfaces.IProduct AddProduct(string mnemonic, string Name, string mySecID, string myExchangeID, string ExDestination, string myCFICode, string myMMY, string myCurrency, double? strikePx, bool doEvent);
        string AppPath { get; set; }
        Factory Factory { get; }
        void RequestProductDetails(KaiTrade.Interfaces.IProduct prod);
        L1PriceSupport.IL1PX GetL1Prices(KaiTrade.Interfaces.IProduct product);
        void SetL1Prices(KaiTrade.Interfaces.IProduct product, L1PriceSupport.IL1PX L1Price);

        KaiTrade.Interfaces.IDOM  GetDOM(KaiTrade.Interfaces.IProduct product);
        void SetDOM(KaiTrade.Interfaces.IProduct product, KaiTrade.Interfaces.IDOM DOM);
        
    }
}
