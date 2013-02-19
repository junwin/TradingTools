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
using System.Text;

namespace KaiTrade.Interfaces
{
    /// <summary>
    /// Defines an interface that an object handling raw prices to agregate them into time
    /// or volume slices must implement
    /// </summary>
    public interface PriceAgregator
    {
        string Name
        {
            get;
        }

        /// <summary>
        /// Process a product price into a set of slices in a TSSet
        /// </summary>
        /// <param name="myPx">decimal price</param>
        /// <param name="myVolume">volume at the price - used for constant vol bars</param>
        /// <param name="myTimeStamp">time stamp that the price was reported at the firm</param>
        void ProcessPrice(decimal myPx, decimal myVolume, DateTime myTimeStamp, object myTag);

        /// <summary>
        /// Process a product price into a set of time slices based on the local time price is processed
        /// </summary>
        /// <param name="myPx">decimal price</param>
        void ProcessPrice(decimal myPx);

        /// <summary>
        /// Process a product price into a set of time slices based on the price update passed
        /// </summary>
        /// <param name="myPx">decimal price</param>
        void ProcessPrice(KaiTrade.Interfaces.PXUpdate pxUpdate);

        /// <summary>
        /// Get/Set the TSSet the agregator will work on
        /// </summary>
        KaiTrade.Interfaces.TSSet TSSet
        {
            get; set;
        }
    }
}
