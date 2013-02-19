//-----------------------------------------------------------------------
// <copyright file="ITradeSystemManager.cs" company="KaiTrade LLC">
// Copyright (c) 2013, KaiTrade LLC.
//// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
//// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>// <author>John Unwin</author>
// <website>https://github.com/junwin/K2RTD.git</website>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace KaiTrade.Interfaces
{
    public interface ITradeSystemManager
    {
        /// <summary>
        /// Get a trade system using its ID
        /// </summary>
        /// <param name="tradeSystemID"></param>
        /// <returns></returns>
        KaiTrade.Interfaces.TradeSystem GetTradeSystem(string tradeSystemID);

        /// <summary>
        /// Get a list of trade systems ID's for the tradeSys
        /// </summary>
        /// <param name="tradeSystemName">name of trade system</param>
        /// <returns>list of matchng ID's - can be more than 1 since only the ID is unique</returns>
        List<string> GetTradeSystemID(string tradeSystemName);

        /// <summary>
        /// Add or replace a trading system - will store this in the tradesystem manager
        /// </summary>
        /// <param name="tradeSystem"></param>
        void AddReplaceTradeSystem(KaiTrade.Interfaces.TradeSystem tradeSystem);

        /// <summary>
        /// Get a list of trade systems supported by some venue
        /// </summary>
        /// <param name="venue">Venue code</param>
        /// <returns></returns>
        List<KaiTrade.Interfaces.TradeSystem> GetAvailableTradeSystems(string venue);
        

        /// <summary>
        /// Will request any trade systems that the driver supports - note that this
        /// is asyncronous the driver will add any trading systems using the Facade
        /// </summary>
        void RequestTradeSystems();

        /// <summary>
        /// Get the parameters supported by some trading system as ATDL
        /// </summary>
        /// <param name="tradeSystemID"></param>
        /// <returns></returns>
        string GetParametersAsFIXAtdl(string tradeSystemID);

        /// <summary>
        /// Add a mapping between some confition (used as a signal and a strategy
        /// </summary>
        /// <param name="conditionName">name of condition</param>
        /// <param name="strategyName">name of mapped strategy</param>
        /// <param name="isEntry">is the condition used as an entry or exit</param>
        void AddConditionStrategy(string conditionName, string strategyName, bool isEntry);

        /// <summary>
        /// Get the strategy and signal type for a condition used to 
        /// create a trade signal
        /// </summary>
        /// <param name="conditionName">name of condition</param>
        /// <param name="strategyName">name of strategy</param>
        /// <param name="isEntry">is the condition treated as entry or exit</param>
        void GetConditionStrategyEntry(string conditionName, out string strategyName, out bool isEntry);

        

        
    }
}
