using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DriverBase
{
    public class Facade : IFacade
    {
        private string _appPath = "";
        private Dictionary<string, L1PriceSupport.IL1PX> _l1Prices;
        private Dictionary<string, KaiTrade.Interfaces.IDOM> _DOM;

        public Facade()
        {
            _l1Prices = new Dictionary<string,L1PriceSupport.IL1PX>();
            _DOM = new Dictionary<string, KaiTrade.Interfaces.IDOM>();
        }

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


        public L1PriceSupport.IL1PX GetL1Prices(KaiTrade.Interfaces.IProduct product)
        {
            if(_l1Prices.ContainsKey(product.Mnemonic))
            {
                return _l1Prices[product.Mnemonic];
            }
            return null;

        }
        public void SetL1Prices(KaiTrade.Interfaces.IProduct product, L1PriceSupport.IL1PX L1Price)
        {
            if (_l1Prices.ContainsKey(product.Mnemonic))
            {
                _l1Prices[product.Mnemonic] = L1Price;
            }
            else
            {
                _l1Prices.Add(product.Mnemonic, L1Price);
            }
        }

        public KaiTrade.Interfaces.IDOM GetDOM(KaiTrade.Interfaces.IProduct product)
        {
            if (_DOM.ContainsKey(product.Mnemonic))
            {
                return _DOM[product.Mnemonic];
            }
            return null;
        }
        public void SetDOM(KaiTrade.Interfaces.IProduct product, KaiTrade.Interfaces.IDOM DOM)
        {
            if (_DOM.ContainsKey(product.Mnemonic))
            {
                _DOM[product.Mnemonic] = DOM;
            }
            else
            {
                _DOM.Add(product.Mnemonic, DOM);
            }
        }
  
        
    }


}
