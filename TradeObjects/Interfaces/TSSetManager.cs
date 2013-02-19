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
    /// Used to manage time series datasets
    /// </summary>
    public interface TSSetManager
    {
        /// <summary>
        /// Create a TS Query Group
        /// </summary>
        /// <returns></returns>
        KaiTrade.Interfaces.TSQueryGroup CreateTSQueryGroup();

        /// <summary>
        /// Create and register an empty time series data set
        /// </summary>
        /// <returns></returns>
        KaiTrade.Interfaces.TSSet CreateTSSet();

        /// <summary>
        /// Get a time series set based on its ID
        /// </summary>
        /// <param name="myID"></param>
        /// <returns></returns>
        KaiTrade.Interfaces.TSSet GetTSSet(string myID);

        /// <summary>
        /// Get a time series set based on its Alias
        /// </summary>
        /// <param name="myID"></param>
        /// <returns></returns>
        KaiTrade.Interfaces.TSSet GetTSSetWithAlias(string myID);

        /// <summary>
        /// Get a list of allias in the maanger
        /// </summary>
        /// <returns></returns>
        List<string> GetAliasList();
    }
}
