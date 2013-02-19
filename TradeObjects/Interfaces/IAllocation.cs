//-----------------------------------------------------------------------
// <copyright file="IAllocation.cs" company="KaiTrade LLC">
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
    /// Represents an allocation based on a set of accounts
    /// used to split an order - the implimenting class will contain a
    /// set of AccountAllocations
    /// </summary>
    public interface IAllocation
    {
        /// <summary>
        /// unique ID for this allocation
        /// </summary>
        string ID
        { get;}

        /// <summary>
        /// Friendly name
        /// </summary>
        string Name
        { get; set;}

        /// <summary>
        /// Mnemonic currently used
        /// </summary>
        string Mnemonic
        { get; set;}

        /// <summary>
        /// Set the min order size for this allocation
        /// </summary>
        double MinOrderSize
        { get; set;}

        /// <summary>
        /// Get/Set the total qty for the allocation
        /// </summary>
        double TotalQty
        { get; set;}

        /// <summary>
        /// Returns the total ratio allocated - ideally this is 100
        /// </summary>
        /// <returns></returns>
        int GetTotalRatioAllocated();

        /// <summary>
        /// Apply the size to the ratios to get an ammount for
        /// each account line
        /// </summary>
        /// <param name="mySize"></param>
        void RecalcAmounts(double mySize);

        /// <summary>
        /// Set ratios from amounts specified
        /// </summary>
        /// <returns>total of all amounts</returns>
        double SetRatioFromAmounts();

        /// <summary>
        /// Get/Set a list of account allocations used by this
        /// </summary>
        List<KaiTrade.Interfaces.AccountAllocation> Items
        { get; set;}

        /// <summary>
        /// Reverse the order of the account allocations
        /// </summary>
        void ReverseOrder();

        /// <summary>
        /// Create an empty account allocation and add it to the allocation
        /// </summary>
        /// <param name="myAccountCode"></param>
        /// <returns></returns>
        KaiTrade.Interfaces.AccountAllocation CreateAccountAllocation(string myAccountCode);

        /// <summary>
        /// Create an empty account allocation and insert it in the allocations after the index specified
        /// </summary>
        /// <param name="myAccountCode"></param>
        /// <param name="myPos">0 based index where the account alloc will be inserted</param>
        /// <returns></returns>
        KaiTrade.Interfaces.AccountAllocation InsertCreateAccountAllocation(string myAccountCode, int myPos);

        /// <summary>
        /// set up from an XML databinding
        /// </summary>
        /// <param name="myAllocation"></param>
        void FromXMLDB(KAI.kaitns.Allocation myAllocation);

        /// <summary>
        /// set up from an XML databinding - opyionally leaving the ID
        ///
        /// </summary>
        /// <param name="myAllocation"></param>
        /// <param name="replaceID">if true  replace the ID</param>
        void FromXMLDB(KAI.kaitns.Allocation myAllocation, bool replaceID);

        /// <summary>
        /// Return an XMLDB representation
        /// </summary>
        /// <returns></returns>
        KAI.kaitns.Allocation ToXMLDB();

        /// <summary>
        /// Get the account allocation for the ID passed or return null
        /// </summary>
        /// <param name="myID"></param>
        /// <returns></returns>
        KaiTrade.Interfaces.AccountAllocation GetAccountAllocation(string myID);

        /// <summary>
        /// Remove the account allocation specified by the ID passed
        /// </summary>
        /// <param name="myID"></param>
        void Remove(string myID);
    }
}
