
/***************************************************************************
 *
 *      Copyright (c) 2009,2010,2011,2012 KaiTrade LLC (registered in Delaware)
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

    public delegate void OnProductPositionChanged(object sender, IPositionSummary productPosition);
    public delegate void OnAccountSummaryChanged(object sender, IPositionSummary accountSummary);

    /// <summary>
    /// Defines the interface for our position publisher
    /// </summary>
    public interface PositionPublisher
    {
        
        OnProductPositionChanged ProductPositionChanged
        { get; set; }

        OnAccountSummaryChanged AccountSummaryChanged
        { get; set; }

        /// <summary>
        /// Apply a position update
        /// </summary>
        /// <param name="newPosition"></param>
        void ApplyPosition(KaiTrade.Interfaces.IPosition newPosition);

        /// <summary>
        /// Get the account position of the product specified - will throw an exceptions
        /// if the product is not found
        /// </summary>
        /// <param name="product">product that you want position for</param>
        /// <returns>position</returns>
        long  GetProductPosition(string product);

        /// <summary>
        /// Get the account position of the account and product specified - will throw an exceptions
        /// if the product or account is not found
        /// </summary>
        /// <param name="product">product that you want position for</param>
        /// <param name="account">account that you want position for</param>
        /// <returns>position</returns>
        long GetAccountProductPosition(string account, string product);
       
    }
}
