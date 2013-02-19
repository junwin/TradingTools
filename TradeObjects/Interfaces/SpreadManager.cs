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
    /// Interface of an object that manages a set of spread trades
    /// </summary>
    public interface SpreadManager
    {
        /// <summary>
        /// Create a spread trade and register it in the manager
        /// </summary>
        /// <returns></returns>
        KaiTrade.Interfaces.SpreadTrade CreateSpreadTrade();

        /// <summary>
        /// Get a spread trade based on its ID
        /// </summary>
        /// <param name="myID"></param>
        /// <returns></returns>
        KaiTrade.Interfaces.SpreadTrade GetSpreadTrade(string myID);

        /// <summary>
        /// Get a spread trade based on its Name
        /// </summary>
        /// <param name="myName"> spreads name</param>
        /// <returns></returns>
        KaiTrade.Interfaces.SpreadTrade GetSpreadTradeWithName(string myName);

        /// <summary>
        /// Get a list of all the pair/spread ids
        /// </summary>
        /// <returns></returns>
        List<string> GetIDs();

        /// <summary>
        /// Load the manager from a file of data bindings
        /// </summary>
        /// <param name="myFilePath"></param>
        void FromFile(string myFilePath);

        /// <summary>
        /// Store the manager from a file of data bindings
        /// </summary>
        /// <param name="myFilePath"></param>
        void ToFile(string myFilePath);

        /// <summary>
        /// Apply a timer tick to all the pairs
        /// </summary>
        void Tick();
    }
}
