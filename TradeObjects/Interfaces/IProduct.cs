//-----------------------------------------------------------------------
// <copyright file="IProduct.cs" company="KaiTrade LLC">
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
    public enum PriceFormat { pxdecimal, px32, px64, pxint, none };

    /// <summary>
    /// Delegate for handling PXUpdates
    /// </summary>
    /// <param name="update"></param>
    public delegate void ApplyPXUpdate(KaiTrade.Interfaces.IPXUpdate update);

 
    /// <summary>
    /// Used as WCF data contract - this is used to avoid the quickfix dependancies on the main product
    /// </summary>
    public interface IProduct 
    {
        /// <summary>
        /// Return my identity
        /// </summary>
        string Identity
        {
            get;
            set;
        }

        /// <summary>
        /// set this up from a tradeable product
        /// </summary>
        /// <param name="p"></param>
        void From(KaiTrade.Interfaces.IProduct p);

        /// <summary>
        /// Set up the properties of a tradeble product from this
        /// </summary>
        /// <param name="p"></param>
        void To(KaiTrade.Interfaces.IProduct p);

        /// <summary>
        /// get/set the name of the price calcualtion to use on legs - if needed
        /// </summary>
        string SyntheticPriceCalcName
        { get; set; }

        /// <summary>
        /// get/set a user defined tag
        /// </summary>
        string Tag
        {
            get;
            set;
        }

        /// <summary>
        /// get/set product long name
        /// </summary>
        string LongName
        {
            get;
            set;
        }

        /// <summary>
        /// get/set product Mnemonic
        /// </summary>
        string Mnemonic
        {
            get;
            set;
        }

        /// <summary>
        /// get/set Generic name for the product, this is driver specific
        /// for example in CQG EP refers to the current eMini contract
        ///
        /// </summary>
        string GenericName
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set the driver ID used for this product - an empty string
        /// implies no specific adapter
        /// </summary>
        string DriverID
        {
            get;
            set;
        }

        /// <summary>
        /// get/set the trade venue
        /// </summary>
        string TradeVenue
        {
            get;
            set;
        }

        /// <summary>
        /// The broker service - is used by adapters to determine a service within
        /// a particular venue
        /// </summary>
        string BrokerService
        { get; set; }

        /// <summary>
        /// Exchange listing the product
        /// </summary>
        string Exchange
        {
            get;
            set;
        }

        /// <summary>
        /// Execution destination as defined by institution when order is entered
        /// </summary>
        string ExDestination
        {
            get;
            set;
        }

        /// <summary>
        /// Broker assigned symbol for the product
        /// </summary>
        string Symbol
        {
            get;
            set;
        }

        /// <summary>
        /// Commodity that this product (contract) belongs to
        /// </summary>
        string Commodity
        {
            get;
            set;
        }
        /// <summary>
        /// Product strike price - used for options
        /// </summary>
        decimal? StrikePrice
        {
            get;
            set;
        }

        /// <summary>
        /// Contract date used for futures and options
        /// </summary>
        string MMY
        {
            get;
            set;
        }

        /// <summary>
        /// Futures and Options maturity date YYYYMMDD - see MMY 
        /// </summary>
        string MaturityDate
        { get; set; }

        /// <summary>
        /// CFI Code (see Fixprotocol spec) used to indicate the type of product(Stock, Future, option etc)
        /// </summary>
        string CFICode
        {
            get;
            set;
        }

        /// <summary>
        /// Alternative ID for the security, ISIN, Reuter code, CUSIP etc see IDSource for type of symbol
        /// </summary>
        string SecurityID
        {
            get;
            set;
        }

        /// <summary>
        /// Type of Security ID - see fix spec for valid values
        /// </summary>
        string IDSource
        {
            get;
            set;
        }

        /// <summary>
        /// Currency that the product is traded in
        /// </summary>
        string Currency
        {
            get;
            set;
        }

        /// <summary>
        /// Product TickSize
        /// </summary>
        decimal? TickSize
        {
            get;
            set;
        }

        /// <summary>
        /// Product TickValue
        /// </summary>
        decimal? TickValue
        {
            get;
            set;
        }
        /// <summary>
        /// return the number of decimal places
        /// </summary>
        int NumberDecimalPlaces
        {
            get;
            set;
        }

        /// <summary>
        /// A multiplier applied to qty (Bid/Offer..) received from the venue
        /// for example for IB this should be 100 for stocks
        /// </summary>
        int PriceFeedQuantityMultiplier
        {
            get;
            set;
        }

        /// <summary>
        /// Product Contract Size
        /// </summary>
        decimal? ContractSize
        {
            get;
            set;
        }

        /// <summary>
        /// Product quantity increment size/ change size
        /// </summary>
        int QtyIncrement
        {
            get;
            set;
        }

        /// <summary>
        /// get set the defult number of depth levels for this product
        /// </summary>
        int DepthLevelCount
        {
            get;
            set;
        }
    }
}
