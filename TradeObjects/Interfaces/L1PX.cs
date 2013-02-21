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
        void ApplyUpdate(KaiTrade.Interfaces.IPXUpdate update);

        /// <summary>
        /// Get the price values as an updates
        /// </summary>
        /// <returns></returns>
        KaiTrade.Interfaces.IPXUpdate  AsUpdate();

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
