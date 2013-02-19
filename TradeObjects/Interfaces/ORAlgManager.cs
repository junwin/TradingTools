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
    /// The interface that an object that manages a set of Order routing
    /// algorithms must implement
    /// </summary>
    public interface ORAlgManager
    {
        /// <summary>
        /// Get a named OR Strategy Algorithm
        /// </summary>
        /// <param name="myName">name of strategy</param>
        /// <returns></returns>
        KaiTrade.Interfaces.ORStrategyAlgorithm GetORAlgAlgorithm(string myName);

        /// <summary>
        /// Add an OR Strategy Algorithm
        /// </summary>
        /// <param name="myName"></param>
        /// <param name="myAlg"></param>
        void AddORAlgAlgorithm(string myName, KaiTrade.Interfaces.ORStrategyAlgorithm myAlg);
    }
}
