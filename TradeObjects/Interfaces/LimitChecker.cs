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
using System.Text;

namespace KaiTrade.Interfaces
{
    /// <summary>
    /// defines the interface that must be implemented by an object used to
    /// check limits when trading.
    /// </summary>
    public interface LimitChecker
    {
        /// <summary>
        /// Does this order break limits
        /// </summary>
        /// <param name="?"></param>
        /// <returns>true => limits broken/exceeded</returns>
        bool BreaksLimits(out string myTextReason, KaiTrade.Interfaces.Order myOrder);

        /// <summary>
        /// If the order price and qty are changed are the limits broken
        /// </summary>
        /// <param name="myOrder">exisitng order</param>
        /// <param name="newQty">new qty</param>
        /// <param name="newPrice">new price</param>
        /// <returns>true => limits broken/exceeded</returns>
        bool BreaksLimits(out string myTextReason, KaiTrade.Interfaces.Order myOrder, double newQty, double newPrice);

        /// <summary>
        /// Set the current position
        /// </summary>
        /// <param name="myLongWrk"></param>
        /// <param name="myShortWrk"></param>
        /// <param name="myLongFilled"></param>
        /// <param name="myShortFilled"></param>
        void SetPosition(double myLongWrk, double myShortWrk, double myLongFilled, double myShortFilled);

        /// <summary>
        /// Get/Set max qty for an individual order
        /// </summary>
        double MaxOrderQty
        {
            get;
            set;
        }

        /// <summary>
        /// Max qty allowed for all orders in the system
        /// </summary>
        double SystemMaxQtyLimit
        {
            get;
            set;
        }

        /// <summary>
        /// Get/set max consideration (price*vol) for an individual order
        /// </summary>
        double MaxOrderConsideration
        {
            get;
            set;
        }

        /// <summary>
        /// Get/set the max consideration for the system
        /// </summary>
        double SystemMaxConsideration
        {
            get;
            set;
        }
    }
}
