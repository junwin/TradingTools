/***************************************************************************
 *
 *      Copyright (c) 2009,2010,2011 KaiTrade LLC (registered in Delaware)
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
using System.Linq;
using System.Text;

namespace KaiTrade.Interfaces
{
    /// <summary>
    /// Defines the interface for a  item's P&L
    /// </summary>
    public interface PNLItem
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
