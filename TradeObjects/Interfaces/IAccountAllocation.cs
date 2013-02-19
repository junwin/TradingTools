//-----------------------------------------------------------------------
// <copyright file="IAccountAllocation.cs" company="KaiTrade LLC">
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
    /// Represents an individual account allocation, used when
    /// splitting an order over N accounts
    /// </summary>
    public interface IAccountAllocation
    {
        /// <summary>
        /// unique id for the account allocation
        /// </summary>
        string ID
        { get; set; }

        /// <summary>
        /// Get/Set the parent allocation
        /// </summary>
        KaiTrade.Interfaces.IAllocation Parent
        { get; set;}

        /// <summary>
        /// ID of the parent allocation (same as the parent ref)
        /// </summary>
        string ParentID
        { get; set; }

        /// <summary>
        /// Get set the venue code for the allocation
        /// </summary>
        string VenueCode
        { get; set;}

        /// <summary>
        /// Account code of this allocation
        /// </summary>
        string AccountCode
        { get; set;}

        /// <summary>
        /// ratio of total order to allocate 0..1
        /// </summary>
        double Ratio
        { get; set;}

        /// <summary>
        /// Ammount to allocate - note this overides percent
        /// </summary>
        double Amount
        { get; set;}

        /// <summary>
        /// Max size of an individual order, this is used with the amount to
        /// to place a set of orders to completly buy the allocation amount
        /// it should be less than the amount
        /// </summary>
        double TranchSize
        { get; set;}

        /// <summary>
        /// Is this line enabled
        /// </summary>
        bool Enabled
        { get; set;}

        /// <summary>
        /// Get the cummulative quantity of any orders
        /// </summary>
        decimal CumQty
        { get; }

        /// <summary>
        /// Returns the qty working in the market for any orders
        /// </summary>
        decimal LeavesQty
        { get; }

        

        
    }
}
