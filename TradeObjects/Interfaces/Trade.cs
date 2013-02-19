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
using System.Text;

namespace KaiTrade.Interfaces
{
    /// <summary>
    /// Represents a trade record - based on an Execution report
    /// </summary>
    public interface Trade
    {
        string Identity
        {
            get;
            set;
        }

        string TradeID
        { get; set; }

        string MatchID
        { get; set; }

        string MessageEventSource
        { get; set; }

        string TradingSessionID
        { get; set; }

        string TradeInputSource
        { get; set; }

        string HandlInst
        { get; set; }

        string VenueType
        { get; set; }
        string ExecutingBrokerCode
        { get; set; }

        string ClearingBrokerCode
        { get; set; }

        string Trader
        { get; set; }

        DateTime ClearingDate
        { get; set; }

        DateTime BusinessDate
        { get; set; }

        string SessionID
        { get; set; }

        string SessionSubID
        { get; set; }

        /// <summary>
        /// Identifier used to track a trade system use of orders, strategeies and algos against some
        /// ID
        /// </summary>
        string CorrelationID
        {
            get;
            set;
        }

        string VenueCode
        {
            get;
            set;
        }

        string ProductID
        {
            get;
            set;
        }

        
        string Mnemonic
        {
            get;
            set;
        }

        string Account
        {
            get;
            set;
        }

        string ClOrdID
        {
            get;
            set;
        }

        string ClOrdID2
        {
            get;
            set;
        }

        string OrigClOrdID
        {
            get;
            set;
        }

        string OrderID
        {
            get;
            set;
        }

        string Side
        {
            get;
            set;
        }

        string OrdType
        {
            get;
            set;
        }

        decimal? Quantity
        {
            get;
            set;
        }

       decimal?  Price
       {
            get;
            set;
        }

        decimal? StopPx
        {
            get;
            set;
        }

        string TradeStatus
        {
            get;
            set;
        }

        decimal? LeavesQty
        {
            get;
            set;
        }

        decimal? CumQty
        {
            get;
            set;
        }

        decimal? LastPx
        {
            get;
            set;
        }

        decimal? LastQty
        {
            get;
            set;
        }

        decimal? AvgPx
        {
            get;
            set;
        }

        string Text
        {
            get;
            set;
        }

        string TransactTime
        {
            get;
            set;
        }

        string TradeDate
        {
            get;
            set;
        }

        string Description
        {
            get;
            set;
        }

        string Tag
        {
            get;
            set;
        }

        string ExecutionID
        {
            get;
            set;
        }

        string TimeInForce
        {
            get;
            set;
        }

        /// <summary>
        /// Date time processed/stored in DB
        /// </summary>
        DateTime SystemDate
        {
            get;
            set;
        }
    }
}
