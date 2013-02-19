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
using System.Text;

namespace KaiTrade.Interfaces
{
   

    /// <summary>
    /// Defines a fill receved from a broker or exchange - the underlying data here is an 
    /// execution report or drop copy
    /// </summary>
    public interface Fill 
    {
        /// <summary>
        ///  The fill Identity from the ExecID on a fix message
        /// </summary>
        string Identity
        {
            get;
            set;
        }

        /// <summary>
        /// Long Sequence assigned by Datastore - can be used to records
        /// processed fills
        /// </summary>
        long Sequence
        { get; set; }

        /// <summary>
        /// The order id - this is the KaiTrade order identity
        /// </summary>
        string OrderID
        {
            get;
            set;
        }

        /// <summary>
        /// ClOrdID of the order at the time of the fill
        /// </summary>
        string ClOrdID
        { get; set; }

        /// <summary>
        /// original ClOrdID of the order at the time of the fill
        /// </summary>
        string OrigClOrdID
        { get; set; }

        /// <summary>
        /// Describes purpose of the Exec report
        /// </summary>
        string ExecType
        { get; set; }

        /// <summary>
        /// If trade cancel or tarde correct refers to the previous fill/execution report
        /// </summary>
        string ExecRefID
        { get; set; }

        /// <summary>
        /// Account that the order traded under
        /// </summary>
        string Account
        { get; set; }

        /// <summary>
        /// Mnemonic - better to use the product ID where possible
        /// </summary>
        string Mnemonic
        { get; set; }


        /// <summary>
        /// Unique product ID
        /// </summary>
        string ProductID
        { get; set; }


        /// <summary>
        /// system time stamp - time last processed or data service action
        /// </summary>
        DateTime SystemTime
        { get; set; }

        /// <summary>
        /// The quantity filled on the exec report - can be 0
        /// </summary>
        double FillQty
        {
            get;
            set;
        }

        /// <summary>
        /// Last trade price
        /// </summary>
        double LastPx
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
        /// Cumulative qty - some fills do not have a value
        /// </summary>
        double CumQty
        {
            get;
            set;
        }

        /// <summary>
        /// Qty left in the market  - some fills do not have a value
        /// </summary>
        double LeavesQty
        {
            get;
            set;
        }

        /// <summary>
        /// String version of the order status
        /// </summary>
        string OrderStatus
        {
            get;
            set;
        }

        /// <summary>
        /// Ticks price - specific to a particular broker
        /// </summary>
        long Ticks
        { get; set; }

        /// <summary>
        /// The exec report for the fill
        /// </summary>
        string ExecReport
        {
            get;

            set;
        }

        /// <summary>
        /// Set this fill up from a fix execution report
        /// </summary>
        /// <param name="execReport"></param>
        void SetUpFromFixExecReport(string execReport);
    }
}
