using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using K2ServiceInterface;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;

namespace DriverBase
{
    public class AppFacade : IFacade
    {
        private string _appPath = "";
        private IPriceHandler _priceHandler = null;

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

            _appPath = Application.StartupPath;
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

        public IPriceHandler PriceHandler
        {
            get { return _priceHandler; }
            set { _priceHandler = value; }
        }



        public string GetUserProfileProperty(string section, string propertyName)
        {
            return "";
        }

        public void ProcessPositionUpdate(KaiTrade.Interfaces.IPosition position)
        {
        }
       
        
    }


}
