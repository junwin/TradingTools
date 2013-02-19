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
    /// The valid states of a pair
    /// </summary>
    public enum SpreadTradeStatus { loaded, stopped, running, paused, complete, error };

    public delegate void SpreadTradeUpdate(SpreadTrade mySpreadTrade);

    public delegate void RunningSpreadTradeUpdate(string myID, SpreadTradeStatus myNewStatus);

    /// <summary>
    /// This interface represents an object that provides spread trading function
    /// In general a spread trade consists of parameters to control the spread/pairs trade
    /// and a strategy used to execute the spread orders.
    /// A spread trade will have 0..N running spread trades, these actually represent an
    /// instance of a 1 or more trades running together - the spread trade object itself acts
    /// as a sort of templte to run manage them running spread trades
    ///
    /// </summary>
    public interface SpreadTrade
    {
        /// <summary>
        /// Get the unique ID for this spread trade
        /// </summary>
        string ID
        {
            get;
        }

        /// <summary>
        /// User friendly name
        /// </summary>
        string Name
        { get; set; }

        /// <summary>
        /// Mnemonic used for the trade - should be a multi leg product
        /// </summary>
        string Mnemonic
        { get; set; }

        /// <summary>
        /// Price calculation used to calculate the multileg spread price
        /// ratio, pairs ect
        /// </summary>
        string PriceConvention
        { get; set; }

        /// <summary>
        /// The level value to be applied to prices in this spread trade
        /// this an decimal value added or subtracted to price when the
        /// spread is bought
        /// </summary>
        double BuyLevel
        { get; set; }

        /// <summary>
        /// The level value to be applied to prices in this spread trade
        /// this an decimal value added or subtracted to price when the
        /// spread is sold
        /// </summary>
        double SellLevel
        { get; set; }

        /// <summary>
        /// Is the qty represented as a number of shares or a monetary amount to but
        /// </summary>
        string QuantityConvention
        { get; set; }

        /// <summary>
        /// Are we buying or selling the spread
        /// </summary>
        string Side
        { get; set; }

        /// <summary>
        /// Total qty to buy
        /// </summary>
        double BuyQty
        { get; set; }

        /// <summary>
        /// Total qty to sell
        /// </summary>
        double SellQty
        { get; set; }

        /// <summary>
        /// Maximum size exposed to the market
        /// </summary>
        long MaxSlice
        { get; set; }

        /// <summary>
        /// Get the remaining qty to trade
        /// </summary>
        long LeavesQty
        { get; set; }

        /// <summary>
        /// Get the potential  qty  i.e. whats left in the market to trade
        /// </summary>
        long PotentialQty
        { get; set; }

        /// <summary>
        /// Size/Qty of the pair to buy or sell
        /// </summary>
        long Size
        { get; set; }

        /// <summary>
        /// Maximum imbalence before the pair will pause trading
        /// </summary>
        long HangAllowence
        { get; set; }

        /// <summary>
        /// Get the curretly hung qty for the spread
        /// </summary>
        long HangQuantity
        { get; set; }

        /// <summary>
        /// Alg used to maintain the prices of quoted orders relative to the market PX
        /// </summary>
        string PriceTrackerAlg
        { get; set; }

        /// <summary>
        /// Get/Set if we will use a price tracker alg
        /// </summary>
        bool UsePriceTracker
        { get; set; }

        /// <summary>
        /// Number of payups that will be made to trade a side
        /// </summary>
        int PayUpCount
        { get; set; }

        /// <summary>
        /// Interval between payups in ms
        /// </summary>
        int PayUpInterval
        { get; set; }

        /// <summary>
        /// Amount as a price that will be added/deducted in an attempt to trade a leg
        /// </summary>
        double PayUpAmount
        { get; set; }

        /// <summary>
        /// Time before a leg that has not traded is converted to a leg - to force it to trade
        /// 0 => no conversion
        /// </summary>
        int TimeToMarket
        { get; set; }

        /// <summary>
        /// Get/Set the stategy that will execute the pairs orders
        /// </summary>
        KaiTrade.Interfaces.Strategy Strategy
        { get; set; }

        /// <summary>
        /// Get the run sequence number - this is incremented each time the pair
        /// is executed in a given session (i.e. user logon) - it is not unique
        /// </summary>
        int RunSequenceNumber
        { get; }

        /// <summary>
        /// Get/Set the product associated with the pair/spread
        /// </summary>
        KaiTrade.Interfaces.TradableProduct Product
        { get; set; }

        /// <summary>
        /// Get the group of strategies associated with the pair
        /// </summary>
        KaiTrade.Interfaces.StrategyGroup StrategyGroup
        {
            get;
        }

        /// <summary>
        /// Get a list of all the running strategies for the pair trade
        /// </summary>
        /// <returns></returns>
        List<KaiTrade.Interfaces.RunningSpreadTrade> GetRunningSpreadTrades();

        /// <summary>
        /// Get the status of the pairs trade
        /// </summary>
        SpreadTradeStatus Status
        {
            get;
        }

        /// <summary>
        /// Enter/Activate the pair trade
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
        /// Update the parameters in each running spread based on the current setting of the spreadtrade
        /// </summary>
        void UpdateRunningTrades();

        /// <summary>
        /// Get the maximum qty that can be traded based on
        /// the hang allowence and the available qty in the
        /// market for the spread
        /// </summary>
        /// <param name="myDesiredQty"></param>
        /// <param name="mySide">side of the trade we wish to place</param>
        /// <returns></returns>
        long GetMaxTradeQty(long myDesiredQty, string mySide);

        /// <summary>
        /// Set up a spread trade from an XML data binding
        /// </summary>
        /// <param name="myOrder"></param>
        void FromXMLDB(KAI.kaitns.SpreadTrade mySpreadTrade);

        /// <summary>
        /// write spread trade onto an XML data bining
        /// </summary>
        /// <returns></returns>
        KAI.kaitns.SpreadTrade ToXMLDB();

        /// <summary>
        /// Tick the spread trade - this allows it to do work on a timed basis
        /// such as processing slices
        /// </summary>
        void Tick();

        /// <summary>
        /// Number of products that are quoted - i.e. will be sumbimitted into the market
        /// </summary>
        /// <returns></returns>
        int QuotedCount();

        /// <summary>
        /// Delegate used to get events from a spread alg
        /// </summary>
        SpreadTradeUpdate SpreadTradeUpdate
        {
            get;
            set;
        }
    }
}
