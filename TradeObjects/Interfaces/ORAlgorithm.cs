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
    /// Delegate to indicate the algo has completed
    /// </summary>
    /// <param name="myMatcher"></param>
    public delegate void ORAlgCompleted(KaiTrade.Interfaces.ORStrategyAlgorithm myAlg, bool isSuccessfull);

    /// <summary>
    /// This interface defines the methods that an algorithm used with a strategy
    /// must impliment. A strategy will use different algorithms depending on the
    /// type of strategy
    /// </summary>
    public interface ORStrategyAlgorithm
    {
        /// <summary>
        /// Reset the alg to its starting state
        /// </summary>
        void Reset();

        /// <summary>
        /// Called on Set updates - used to allow clients to be evented when the alg completes
        /// </summary>
        ORAlgCompleted ORAlgCompleted
        {
            get;
            set;
        }

        /// <summary>
        /// Start the algo running this may result in orders being placed
        /// </summary>
        void Run(KaiTrade.Interfaces.Strategy myStrategy);

        /// <summary>
        /// Enter the strategy - action depends on implimenting class
        /// </summary>
        void Enter(KaiTrade.Interfaces.Strategy myStrategy);

        /// <summary>
        /// Exit the strategy - action depends on implimenting class
        /// </summary>
        void Exit(KaiTrade.Interfaces.Strategy myStrategy);

        /// <summary>
        /// Process a Trade signal
        /// </summary>
        /// <param name="myStrategy">strategy running the algo</param>
        /// <param name="isExitSignal">is the signal an exit-depends on the list of exists for the strategy</param>
        /// <param name="mySignal">a trade signal</param>
        void HandleTradeSignal(KaiTrade.Interfaces.Strategy myStrategy, bool isExitSignal, KaiTrade.Interfaces.TradeSignal mySignal);

        /// <summary>
        /// Get the name of this alg
        /// </summary>
        string Name
        { get;}

        /// <summary>
        /// Get/Set the strategy runing this algo
        /// </summary>
        KaiTrade.Interfaces.Strategy Strategy
        { get; set;}

        /// <summary>
        /// Submit the strategy based on the default parameters in the strategy
        /// </summary>
        /// <param name="myStrategy"></param>
        string Submit(Strategy myStrategy);

        /// <summary>
        /// Sumbit the order - note that the order passed is only used to hold order parameters
        /// </summary>
        /// <param name="myStrategy"></param>
        /// <param name="myOrder"></param>
        string Submit(Strategy myStrategy, Order myOrder);

        /// <summary>
        /// Place order for the strategy
        /// </summary>
        /// <param name="myStrategy"></param>
        /// <param name="myQty"></param>
        /// <param name="myQtyLimit"></param>
        /// <param name="myPrice"></param>
        /// <param name="myStopPx"></param>
        /// <param name="mySide"></param>
        /// <param name="myOrderType"></param>
        /// <param name="myTimeType"></param>
        /// <param name="myDateTime"></param>
        /// <param name="myAccount"></param>
        /// <returns></returns>
        string Submit(Strategy myStrategy, double myQty, double myQtyLimit, double myPrice, double myStopPx, string mySide, string myOrderType, string myTimeType, string myDateTime, string myAccount, string extendedOrderType,  List<KaiTrade.Interfaces.K2Parameter> parms);

        /// <summary>
        /// Apply a modification to a strategy
        /// </summary>
        /// <param name="myStrategy"></param>
        /// <param name="newQty"></param>
        /// <param name="newPrice"></param>
        /// <returns>number of affected orders</returns>
        int Replace(Strategy myStrategy, double? newQty, double? newPrice, double? newStopPx);

        /// <summary>
        /// Cancel a running strategy
        /// </summary>
        /// <param name="myStrategy"></param>
        /// /// <returns>number of affected orders</returns>
        int Cancel(Strategy myStrategy);
        int CancelBuy(Strategy myStrategy);
        int CancelSell(Strategy myStrategy);

        /// <summary>
        /// Flatten/Trade out any positions in the strategy
        /// </summary>
        /// <param name="myStrategy"></param>
        /// <returns></returns>
        int Flatten(Strategy myStrategy);

        /// <summary>
        /// Flatten/Trade out any positions in the strategy - set the ordertag
        /// </summary>
        /// <param name="myStrategy"></param>
        /// <param name="orderTag">tag to be used on orders</param>
        /// <returns></returns>
        int Flatten(Strategy myStrategy, string orderTag);

        /// <summary>
        /// Handle an Exec report for an order belonging to a strategy
        /// the primitive gandling of the exec report on the order itself will have been done
        /// exec reports always relate to an actual order in the market
        /// </summary>
        /// <param name="myExec">fix execution report</param>
        /// <param name="myOrd">order the execution report applies to </param>
        void HandleExecReport(QuickFix.Message myExec, KaiTrade.Interfaces.Strategy myStrategy, KaiTrade.Interfaces.Order myOrd);

        /// <summary>
        /// Handle an reject report for an order belonging to a strategy
        /// the primitive gandling of the exec report on the order itself will have been done
        /// reject reports always relate to an actual order in the market
        /// </summary>
        /// <param name="myExec">fix execution report</param>
        /// <param name="myOrd">order the execution report applies to </param>
        void HandleReject(QuickFix.Message myReject, KaiTrade.Interfaces.Strategy myStrategy, KaiTrade.Interfaces.Order myOrd);

        /// <summary>
        /// Apply a change in position
        /// </summary>
        /// <param name="myStrategy"></param>
        /// <param name="myExec"></param>
        void HandlePositionChange(Strategy myStrategy);

        /// <summary>
        /// Refresh the position of the Strategy, will iterate the orders to
        /// total position
        /// </summary>
        void RefreshPosition(KaiTrade.Interfaces.Strategy myStrategy);

        /// <summary>
        /// get the PnL for the strategy - calculated when accessed
        /// not you need prices for this.
        /// </summary>
        double GetPNL(KaiTrade.Interfaces.Strategy myStrategy);

        /// <summary>
        /// Starting position - used to offset the actual position
        /// </summary>
        double StartPosition
        { get; set; }

        /// <summary>
        /// Get a setting value by name - used to get alg parameters
        /// </summary>
        /// <param name="myName"></param>
        /// <returns></returns>
        string GetParameterValue(string myName);

        /// <summary>
        /// Set a alg setting by name, used to set parameters used by the alg. The names
        /// need to match those published in ATDL
        /// </summary>
        /// <param name="myName"></param>
        /// <param name="myValue"></param>
        /// <param name="runIdentifier">this identifies a particular run - if empty will just apply in general</param>
        void SetParameterValue(string myName, string myValue, string runIdentifier);

        /// <summary>
        /// Set a paramter in the alg
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="runIdentifier"></param>
        void SetParameterValue(KaiTrade.Interfaces.K2Parameter parameter, string runIdentifier);

        /// <summary>
        /// Get a list of avalaible parameters for the alg
        /// </summary>
        /// <returns></returns>
        List<string> GetParameterNames();

        /// <summary>
        /// Clear the algo parameters
        /// </summary>
        void ClearParameters();

        /// <summary>
        /// Get the Atdl that corresponds to this algo - older algos may not support this.
        /// </summary>
        /// <returns></returns>
        string GetAtdl();

        /// <summary>
        /// number of complete runs - e.g. a fully sequence buy-sell etc is done
        /// </summary>
        int Compeleted
        { get;}

        /// <summary>
        /// how many time we have run
        /// </summary>
        int RunCount
        { get;}

        /// <summary>
        /// Process a set of orders - typically on a timed basis
        /// </summary>
        /// <param name="?"></param>
        void ProcessOrderGroup(KaiTrade.Interfaces.OrderGroup myGroup);

        /// <summary>
        /// Identifier used to track a trade system use of orders, strategeies and algos against some
        /// ID
        /// </summary>
        string CorrelationID
        {
            get;
            set;
        }
    }
}
