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
    /// Level one price interface - this always shows an image of prices
    /// for some products. It may be updated using PXUpdate
    /// </summary>
    public interface L1PX
    {
        decimal? OfferPrice
        {
            get;
            //set;
        }
        decimal? OfferSize
        {
            get;
            //set;
        }
        decimal? BidSize
        {
            get;
            //set;
        }
        decimal? BidPrice
        {
            get;
            //set;
        }

        /// <summary>
        /// The last trade price for some product
        /// </summary>
        decimal? TradePrice
        {
            get;
            //set;
        }

        /// <summary>
        /// The last trade volume for some product
        /// </summary>
        decimal? TradeVolume
        {
            get;
            //set;
        }

        decimal? OfferPriceDelta
        {
            get;
            //set;
        }
        decimal? OfferSizeDelta
        {
            get;
            //set;
        }
        decimal? BidSizeDelta
        {
            get;
            //set;
        }
        decimal? BidPriceDelta
        {
            get;
            //set;
        }

        /// <summary>
        /// Change in trade price
        /// </summary>
        decimal? TradePriceDelta
        {
            get;
            //set;
        }
        decimal? TradeVolumeDelta
        {
            get;
            //set;
        }

        decimal? DayHigh
        {
            get;
            //set;
        }
        decimal? DayLow
        {
            get;
            //set;
        }
        decimal? Open
        {
            get;
            //set;
        }

        int TickDirection
        {
            get;
            //set;
        }

        /// <summary>
        /// Apply the update to the corresponding prices
        /// </summary>
        /// <param name="update"></param>
        void ApplyUpdate(KaiTrade.Interfaces.PXUpdate update);

        /// <summary>
        /// Get the price values as an updates
        /// </summary>
        /// <returns></returns>
        KaiTrade.Interfaces.PXUpdate  AsUpdate();

        /// <summary>
        /// API Time of last update(from driver)
        /// </summary>
        DateTime APIUpdateTime
        {
            get;
            set;
        }

        /// <summary>
        /// QuoteID of a update by a quote - this is only valid in response to
        /// a quote request
        /// </summary>
        string QuoteID
        {
            get;
        }

        /// <summary>
        /// Get/set the length of time in milli seconds that this quote (or update) is valid for
        /// </summary>
        long ValidityPeriod
        {
            get;
        }

        /// <summary>
        /// Return a delimited string of current values
        /// time stamp, mnemonic bidsz, bidpx,offerpx, offersz, tradesz, trade px
        /// </summary>
        /// <param name="myDelimiter">delimiter used between pxvalues</param>
        /// <returns></returns>
        string AsCSVString(string myDelimiter);
    }
}
