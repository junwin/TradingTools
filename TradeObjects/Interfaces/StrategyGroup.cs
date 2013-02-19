/***************************************************************************
 *
 *      Copyright (c) 2009, 2010 KaiTrade LLC (registered in Delaware)
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
    /// Models a group of strategies
    /// </summary>
    public interface StrategyGroup
    {
        /// <summary>
        /// Add a strategy to the group
        /// </summary>
        /// <param name="myStrategy"></param>
        void AddStrategy(KaiTrade.Interfaces.Strategy myStrategy);

        /// <summary>
        /// Get the last added strategy
        /// </summary>
        /// <returns></returns>
        KaiTrade.Interfaces.Strategy GetLastStrategy();

        /// <summary>
        /// Get a list of all the groups strategies
        /// </summary>
        List<KaiTrade.Interfaces.Strategy> Strategies
        { get; }

        /// <summary>
        /// return the working qty of the group of strategies(long working - short working) +ve => long
        /// note that this can be a total  for different products and legs
        /// </summary>
        double GetWorkingQty();

        /// <summary>
        /// return the position of the group of strategies(long filled - short filled) +ve => long
        /// note that this can be a total  for different products and legs
        /// </summary>
        double GetPosition();

        /// <summary>
        ///
        /// </summary>
        /// <param name="myTradeQty"> ammount traded long fills + short fills</param>
        /// <param name="myPotentialTraded">potential amount that could trade</param>
        /// <returns></returns>
        void GetCurrentTradedQty(out double myTradeQty, out double myPotentialTraded);

        /// <summary>
        /// Get the current traded qty both Long and short
        /// </summary>
        /// <param name="myLongTradeQty"></param>
        /// <param name="myLongPotentialTraded"></param>
        /// <param name="myShortTradeQty"></param>
        /// <param name="myShortPotentialTraded"></param>
        void GetCurrentTradedQty(out double myLongTradeQty, out double myLongPotentialTraded, out double myShortTradeQty, out double myShortPotentialTraded);

        /// <summary>
        /// Set the enabled flag for the strategies in this group
        /// </summary>
        void SetEnabled(bool myEnabled);

        /// <summary>
        /// Cancel any outstanding orders associated with this strategy group
        /// </summary>
        void Cancel();

        /// <summary>
        /// Flatten positions and cancel working posn in each strategy in the group
        /// </summary>
        void Flatten();

        /// <summary>
        /// Exit the all strategies in the group - action depends on implimenting class
        /// </summary>
        void Exit();
    }
}
