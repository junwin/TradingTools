﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using K2ServiceInterface;

namespace DriverBase
{
    public class AppFacade : IFacade
    {
        private string _appPath = "";
        private Dictionary<string, IL1PX> _l1Prices;
        private Dictionary<string, KaiTrade.Interfaces.IDOM> _DOM;
        /// <summary>
        /// Singleton OrderManager
        /// </summary>
        private static volatile AppFacade s_instance;

        /// <summary>
        /// used to lock the class during instantiation
        /// </summary>
        private static object s_Token = new object();

        /// <summary>
        /// Logger
        /// </summary>
        public log4net.ILog m_Log = log4net.LogManager.GetLogger("Kaitrade");

        public static AppFacade Instance()
        {
            // Uses "Lazy initialization" and double-checked locking
            if (s_instance == null)
            {
                lock (s_Token)
                {
                    if (s_instance == null)
                    {
                        s_instance = new AppFacade();
                    }
                }
            }
            return s_instance;
        }

        protected AppFacade()
        {
            _l1Prices = new Dictionary<string,IL1PX>();
            _DOM = new Dictionary<string, KaiTrade.Interfaces.IDOM>();
        }

        public string AppPath
        {
            get { return _appPath; }
            set { _appPath = value; }
        }

        public IProductManager GetProductManager()
        {
            return ProductManager.Instance();
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

        public IFactory Factory
        {
            get
            {
                return AppFactory.Instance();
            }
        }


        public IL1PX GetL1Prices(KaiTrade.Interfaces.IProduct product)
        {
            if(_l1Prices.ContainsKey(product.Mnemonic))
            {
                return _l1Prices[product.Mnemonic];
            }
            return null;

        }
        public void SetL1Prices(KaiTrade.Interfaces.IProduct product, IL1PX L1Price)
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