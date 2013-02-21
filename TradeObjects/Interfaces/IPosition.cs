
//-----------------------------------------------------------------------
// <copyright file="IPosition.cs" company="KaiTrade LLC">
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
    /// summary of the position qty
    /// </summary>
    public interface IPositionSummary
    {
        /// <summary>
        /// Product Mnemonic
        /// </summary>
        string Mnemonic
        { get; set; }

        /// <summary>
        /// Account associated with the position
        /// </summary>
        string AccountCode
        { get; set; }

        /// <summary>
        /// Identifier used to track a trade system use of orders, strategeies and algos against some
        /// ID - this is the CorrelationID the position record applied to - this may represent
        /// all or some of the position for the correlation id
        /// </summary>
        string CorrelationID
        { get; set; }

        long ProductPosition
        { get; set; }

        long? AccountPositon
        { get; set; }

        long? CorrelationIDPositon
        { get; set; }

        DateTime ServerTime
        { get; set; }


    }
    /// <summary>
    /// Represents some position update(or total position)
    /// </summary>
    public interface IPosition
    {
        /// <summary>
        /// Identity
        /// </summary>
        string Identity
        { get; set; }

        /// <summary>
        /// ID of parent position
        /// </summary>
        string Parent
        { get; set; }


        /// <summary>
        /// Account associated with the position
        /// </summary>
        string AccountCode
        { get; set; }

        /// <summary>
        /// Identifier used to track a trade system use of orders, strategeies and algos against some
        /// ID - this is the CorrelationID the position record applied to - this may represent
        /// all or some of the position for the correlation id
        /// </summary>
        string CorrelationID
        { get; set; }

        
        /// <summary>
        /// Product Mnemonic
        /// </summary>
        string Mnemonic
        { get; set; }



        /// <summary>
        /// Average price
        /// </summary>
        decimal? AvgPrice
        { get; set; }

        /// <summary>
        /// Profit and Loss
        /// </summary>
        decimal? PnL
        { get; set; }

        /// <summary>
        /// Quantity - the current position (this is NOT a delta)
        /// </summary>
        long Quantity
        { get; set; }

        /// <summary>
        /// Side
        /// </summary>
        string Side
        { get; set; }

        /// <summary>
        /// Time of last update
        /// </summary>
        DateTime UpdateTime
        { get; set; }

        /// <summary>
        /// Open trade equity
        /// </summary>
        decimal? OTE
        { get; set; }

        /// <summary>
        /// Market value of options
        /// </summary>
        decimal? MVO
        { get; set; }

        
    }
}
