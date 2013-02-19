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
    /// This represents a running spread trade, this is usually created by a SpreadTrade object
    /// the spread trade object can ahve many associated running spread trades.
    /// </summary>
    public interface RunningSpreadTrade
    {
        /// <summary>
        /// return the unique ID of this running spread trade
        /// </summary>
        string Identity
        { get ;  }

        /// <summary>
        /// return the count of the current instance - this is used to create
        /// a name that refelects each individual instance of the
        /// object - the Identity could be used but its very long for a grid
        /// Note that is never persisted
        /// </summary>
        int InstanceCount
        { get; }

        /// <summary>
        /// Delegate used to get events from a spread alg
        /// </summary>
        KaiTrade.Interfaces.RunningSpreadTradeUpdate RunningSpreadTradeUpdate
        {
            get;
            set;
        }

        /// <summary>
        /// Get the status of the pairs trade
        /// </summary>
        KaiTrade.Interfaces.SpreadTradeStatus Status
        {
            get;
        }

        /// <summary>
        /// Maximum size exposed to the market
        /// </summary>
        long MaxSlice
        {
            get;
            set;
        }

        /// <summary>
        /// Get the remaining qty to trade
        /// </summary>
        long LeavesQty
        {
            get;
            set;
        }

        long HangQuantity
        {
            get;
            set;
        }

        /// <summary>
        /// Get the remaining qty to trade
        /// </summary>
        long PotentialQty
        {
            get;
            set;
        }

        string Side
        {
            get;
            set;
        }

        /// <summary>
        /// Size/Qty of the pair to buy or sell
        /// </summary>
        long Size
        {
            get;
            set;
        }
        /// <summary>
        /// The level value to be applied to prices in this spread trade
        /// this an decimal value added or subtracted to price when the
        /// spread is bought
        /// </summary>
        double BuyLevel
        {
            get;
            set;
        }

        /// <summary>
        /// The level value to be applied to prices in this spread trade
        /// this an decimal value added or subtracted to price when the
        /// spread is sold
        /// </summary>
        double SellLevel
        {
            get;
            set;
        }

        /// <summary>
        /// Total qty to buy
        /// </summary>
        double BuyQty
        {
            get;
            set;
        }

        /// <summary>
        /// Total qty to sell
        /// </summary>
        double SellQty
        {
            get;
            set;
        }

        bool UsePriceTracker
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set the stategy that will execute the pairs orders
        /// </summary>
        KaiTrade.Interfaces.Strategy Strategy
        {
            get;
            set;
        }

        /// <summary>
        /// Activate the pair trade
        /// </summary>
        /// <param name="mySide"></param>
        void Enter(string mySide, double myLevel, double myQty);

        /// <summary>
        /// Exit a pair/spread trade
        /// </summary>
        /// <param name="myCancel"></param>
        /// <param name="myFlatten"></param>
        void Exit(bool myCancel, bool myFlatten);

        /// <summary>
        /// Tick the spread trade - this allows it to do work on a timed basis
        /// such as processing slices
        /// </summary>
        void Tick();

        /// <summary>
        /// Get the strategy group currently in use.
        /// </summary>
        KaiTrade.Interfaces.StrategyGroup StrategyGroup
        { get; }
    }
}
