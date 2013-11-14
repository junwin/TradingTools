/***************************************************************************
 *
 *      Copyright (c) 2009,2010,2011,2012 KaiTrade LLC (registered in Delaware)
 *                     All Rights Reserved Worldwide
 *
 * STRICTLY PROPRIETARY and CONFIDENTIAL
 *
 * WARNING:  This file is the confidential property of KaiTrade LLC For
 * use only by those with the express written permission and license from
 * KaiTrade LLC.  Unauthorized reproduction, distribution, use or disclosure
 * of this file or any program (or document) is prohibited.
 *
 ***************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K2DomainSvc
{
    /// <summary>
    /// Provides order domain functions
    /// </summary>
    public class Order
    {
        private log4net.ILog m_Log = log4net.LogManager.GetLogger("K2DomainSvc");

        public void InsertOrder(KaiTrade.Interfaces.IOrder o)
        {
            try
            {
                K2DS.K2OrderDS orderDS = new K2DS.K2OrderDS();
                orderDS.InsertOrder(o as K2DataObjects.Order, true);
            }
            catch (Exception myE)
            {
                m_Log.Error("InsertOrder", myE);
            }
        }

        /// <summary>
        /// Get a trade given the order
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public KaiTrade.Interfaces.ITrade GetTrade(KaiTrade.Interfaces.IOrder o)
        {
            K2DS.K2TradeDS tradeDS = new K2DS.K2TradeDS();
            IEnumerable<K2DataObjects.Trade> trades = tradeDS.GetTrade((o as K2DataObjects.Order));

            foreach(K2DataObjects.Trade t in trades)
            {
                return t;
            }
            return null;
        }
    }
}
