using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DriverBase
{
    public class Facade : IFacade
    {
        private string _appPath = "";

        public string AppPath
        {
            get { return _appPath; }
            set { _appPath = value; }
        }
        public void AddProduct(string genericName, string tradeVenue)
        {
        }

        public void RequestProductDetails(KaiTrade.Interfaces.IProduct prod)
        {
        }

        public KaiTrade.Interfaces.IProduct AddProduct(string mnemonic, string Name, string mySecID, string myExchangeID, string ExDestination, string myCFICode, string myMMY, string myCurrency, double? strikePx, bool doEvent)
        {
            return null;
        }

        public Factory Factory
        {
            get
            {
                return Factory.Instance();
            }
        }
  
        
    }


}
