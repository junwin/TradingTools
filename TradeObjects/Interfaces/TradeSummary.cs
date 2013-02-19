/***************************************************************************
 *
 *      Copyright (c) 2009,2010 KaiTrade LLC (registered in Delaware)
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

namespace KaiTrade.Interfaces
{
    /// <summary>
    /// Defines an object that summarizes the trades for a given order at some
    /// point in time
    /// </summary>
    public interface TradeSummary
    {
        /// <summary>
        /// setup the trade summary from an Order
        /// </summary>
        /// <param name="?"></param>
        void FromOrder(KaiTrade.Interfaces.Order order);

        /// <summary>
        /// KaiTrade order Identity
        /// </summary>
        string OrderIdentity
        {
            get;
            set;
        }
        /// <summary>
        /// Average price over all fills for a given order
        /// </summary>
        double AvgPx
        {
            get;
            set;
        }

        /// <summary>
        /// position +Ve is long -ve is short
        /// </summary>
        double Position
        {
            get;
            set;
        }
    }
}
