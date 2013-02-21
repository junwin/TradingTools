//-----------------------------------------------------------------------
// <copyright file="IPNLItem.cs" company="KaiTrade LLC">
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
    /// Defines the interface for a  item's P&L
    /// </summary>
    public interface IPNLItem
    {
        /// <summary>
        /// Get/Set the product mnemonic for this portfolio item
        /// </summary>
        string Mnemonic
        { get; set; }

        /// <summary>
        /// Account Identifier for this line of
        /// </summary>
        string AccountID
        { get; set; }

        /// <summary>
        /// Position of this product line as a decimal, +ve values are long
        /// and -ve values are short
        /// </summary>
        decimal Position
        { get; set; }

        /// <summary>
        /// market price of a unit of the product
        /// </summary>
        decimal MktPrice
        { get; set; }

        /// <summary>
        /// Avergage price per unit of postion - based on the fills
        /// </summary>
        decimal AvgPrice
        { get; set; }

        /// <summary>
        /// realiazed PNL - based on the difference of the average price and the current market price
        /// </summary>
        decimal RealizedPNL
        { get; set; }

        /// <summary>
        /// unrealized P&L - position based on opened and closed trades
        /// </summary>
        decimal UnRealizedPNL
        { get; set; }
    }
}
