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
    public interface OCOManager
    {
        /// <summary>
        /// Add an order to a group of orders that comprise a set of OCO Orders
        /// </summary>
        /// <param name="?"></param>
        /// <param name="OCOGroupName"></param>
        void AddOrderOCO(KaiTrade.Interfaces.Order order, string OCOGroupName);

        /// <summary>
        /// Get the list of orders that comprise an OCO scheme by thier oco group name
        /// </summary>
        /// <param name="OCOGroupName"></param>
        /// <returns></returns>
        List<KaiTrade.Interfaces.Order> GetOCOOrders(string OCOGroupName);

        /// <summary>
        /// Called when one of the orders in an OCO group has traded - no action if the order is not traded
        /// </summary>
        /// <param name="order"></param>
        void OnOrderTraded(KaiTrade.Interfaces.Order order);

        /// <summary>
        /// This checks that we are not legged, if more than one order
        /// in the group is filled we consider that the order is legged
        /// </summary>
        /// <param name="OCOGroupName"></param>
        /// <returns></returns>
        bool IsLegged(string OCOGroupName);
    }
}
