/***************************************************************************
 *
 *      Copyright (c) 2009,2010,2011 KaiTrade LLC (registered in Delaware)
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
    public interface TriggeredOrderManager
    {
        /// <summary>
        /// Create a triggered order based on the order passed in
        /// </summary>
        /// <param name="order">order used to create thge triggered order</param>
        /// <param name="OrderType">incomming order type KSTOP, KTARGET or KHELD</param>
        /// <param name="targetOrderType">order type to be used when submitting the order</param>
        /// <param name="parms">extra parameters to use with the triggered order</param>
        /// <returns></returns>
        KaiTrade.Interfaces.TriggeredOrder Create(KaiTrade.Interfaces.Order order, string orderType, string targetOrderType, List<KaiTrade.Interfaces.K2Parameter> parms);

        /// <summary>
        /// Get a triggered order based on its ID
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        KaiTrade.Interfaces.TriggeredOrder Get(string ID);

        /// <summary>
        /// Add an order to the set of triggered order under management
        /// </summary>
        /// <param name="?"></param>
        void Add(KaiTrade.Interfaces.TriggeredOrder triggeredOrder);

        /// <summary>
        /// Add an order to the set of triggered order under management, use the mnemonic
        /// specified as a prices source for the trigger
        /// </summary>
        /// <param name="mnemonic"></param>
        /// <param name="?"></param>
        void Add(string mnemonic, KaiTrade.Interfaces.TriggeredOrder triggeredOrder );

        /// <summary>
        /// remove the triggered order from the list we monitor
        /// </summary>
        /// <param name="triggeredOrder"></param>
        void Remove(KaiTrade.Interfaces.TriggeredOrder triggeredOrder);
    }
}
