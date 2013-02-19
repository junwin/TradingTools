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
    public enum OrderTriggerType { stoploss, target, held, multiTarget, trailStop, retracement, none };
    public enum OrderTriggerState { waiting, triggered, cancelled, none };

    /// <summary>
    /// Defines the interface some type of triggered order must impliment, these orders are used like normal
    /// orders but with some extended order type KSTOP, KHELD, KTRAILINGSTOP etc
    /// These extended orders are implemented in external assemblied dynamic ally loaded into KaiTrade
    /// </summary>
    public interface TriggeredOrder
    {
        /// <summary>
        /// The identity of this order trigger
        /// </summary>
        string ID
        { get;  }

        /// <summary>
        /// Extra paramters that can be used with this triggered order
        /// </summary>
        List<KaiTrade.Interfaces.K2Parameter> ExtraParameters
        { get; set; }

        /// <summary>
        /// price that the tigger is called
        /// </summary>
        double TriggerPx
        { get; set; }

        /// <summary>
        /// Offset to a trigger px used when sending orders, where a
        /// set of traded is done over time
        /// </summary>
        double TriggerPxOffset
        { get; set; }

        /// <summary>
        /// Trigger type for the order
        /// </summary>
        OrderTriggerType TriggerType
        { get; set; }

        /// <summary>
        /// Trigger state for the order
        /// </summary>
        OrderTriggerState TriggerState
        { get; set; }

        /// <summary>
        /// Order type to use when order is triggered
        /// </summary>
        string OrderType
        { get; set; }

        /// <summary>
        /// ID of the order we will trigger
        /// </summary>
        string OrderIdentifier
        { get; set; }

        /// <summary>
        /// Signal new prices to the trigger
        /// </summary>
        /// <param name="bidPrice"></param>
        /// <param name="askPrice"></param>
        /// <returns>true - if the trigger fires, false if not fired</returns>
        bool PriceSignal(double bidPrice, double askPrice);

        /// <summary>
        /// Cancel the tiggered order, will cancel any running order and set the order to cancelled
        /// the system will then exclude the order from being triggers
        /// </summary>
        void Cancel();

        /// <summary>
        /// Get set the held qty - i.e. the amount still to be sent to the market
        /// </summary>
        long HeldQty
        { get; set; }
    }
}
