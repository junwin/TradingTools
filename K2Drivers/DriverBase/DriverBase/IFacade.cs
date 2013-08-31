using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DriverBase
{
    public interface IFacade
    {
        void AddProduct(string genericName, string tradeVenue);
        KaiTrade.Interfaces.IProduct AddProduct(string mnemonic, string Name, string mySecID, string myExchangeID, string ExDestination, string myCFICode, string myMMY, string myCurrency, double? strikePx, bool doEvent);
        string AppPath { get; set; }
        Factory Factory { get; }
        void RequestProductDetails(KaiTrade.Interfaces.IProduct prod);
    }
}
