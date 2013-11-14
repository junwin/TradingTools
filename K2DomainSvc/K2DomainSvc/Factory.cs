using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K2DomainSvc
{
    public class Factory
    {
        /// <summary>
        /// Singleton OrderManager
        /// </summary>
        private static volatile Factory s_instance;

        /// <summary>
        /// used to lock the class during instantiation
        /// </summary>
        private static object s_Token = new object();

        public static Factory Instance()
        {
            // Uses "Lazy initialization" and double-checked locking
            if (s_instance == null)
            {
                lock (s_Token)
                {
                    if (s_instance == null)
                    {
                        s_instance = new Factory();
                    }
                }
            }
            return s_instance;
        }

        protected Factory()
        {

        }


        public Order GetOrderSvc()
        {
            return new Order();
        }

        public Product GetProductSvc()
        {
            return new Product();
        }

        public Trade GetTradeSvc()
        {
            return new Trade();
        }

        public User GetUserSvc()
        {
            return new User();
        }
    }
}
