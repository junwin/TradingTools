//-----------------------------------------------------------------------
// <copyright file="IUser.cs" company="KaiTrade LLC">
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
    /// <summary>
    /// This is used to manage a group of TS Data sets, an application can have many TSQueryGroups, each
    /// group will have 1 or more TS datasets. The group allows the user to access these at a higher level
    /// for example to get the data, load/store and so on
    /// </summary>
    public interface TSQueryGroup
    {
        /// <summary>
        /// Set up a TS Query group from XML
        /// </summary>
        /// <param name="myXML"></param>
        void FromXml(string myXML);

        

        /// <summary>
        /// Store a query group as XML
        /// </summary>
        /// <returns></returns>
        string GetXML();

       

        /// <summary>
        /// Get/Set the group name
        /// </summary>
        string Name
        { get; set;}

        /// <summary>
        /// ID of the strategy that this set is related to (if any)
        /// </summary>
        string StrategyName
        { get; set; }

        /// <summary>
        /// Get the status of a group of set - this will show
        /// the most severe error state of any of the groups sets
        /// open implies all are open
        /// </summary>
        KaiTrade.Interfaces.Status Status
        { get;}

        /// <summary>
        /// will set the mnemonic that used for all members of a query set
        /// </summary>
        string Mnemonic
        {
            get;
            set;
        }

        /// <summary>
        /// This will override the intraday interval used in individual queries
        /// </summary>
        /// <param name="myInterval"></param>
        void SetInterval(int myInterval);

        /// <summary>
        /// Add a subscriber to the query group - any sets that have a publisher interface
        /// will be subscribed to
        /// </summary>
        /// <param name="mySubscriber"></param>
        void addSubscriber(KaiTrade.Interfaces.ISubscriber mySubscriber);

        /// <summary>
        /// Unsubscribe to the  to the query group - any sets that have a publisher interface
        /// will be unsubscribed
        /// </summary>
        /// <param name="mySubscriber"></param>
        void UnSubscribe(KaiTrade.Interfaces.ISubscriber mySubscriber);

        

        /// <summary>
        /// get/set list of TS Sets
        /// </summary>
        List<KaiTrade.Interfaces.ITSSet> Items
        { get; set;}
    }
}
