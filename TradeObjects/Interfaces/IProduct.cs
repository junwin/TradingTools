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
    /// defines a product that can be traded - this includes
    /// synthetic products composed on N legs
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
        /// Set some property in the product based on its name
        /// </summary>
        /// <param name="myPropName">name of the propterty/attribute</param>
        /// <param name="myPropValue"></param>
        void SetProperty(string myPropName, string myPropValue);

        /// <summary>
        /// get/set the name of the price calcualtion to use on legs - if needed
        /// </summary>
        string SyntheticPriceCalcName
        { get; set;}

        
        /// <summary>
        /// Set whether to calculate to leg prices
        /// </summary>
        /// <param name="bSubscribe">true => calculate prices false => do not calculate prices</param>
        void CalculateLegPrices(bool bCalculate);

        /// <summary>
        /// Reset the ratios of all legs for this product
        /// only used on multiLeg products
        /// </summary>
        void ResetLegRatios();

        /// <summary>
        /// Get the Leg associated with a mnemonic - or return null
        /// </summary>
        /// <param name="myMonika">monika name used to find the leg</param>
        /// <returns> a leg or null if leg does not exist</returns>
        Leg GetLeg(string myMnemonic);

        /// <summary>
        /// Add a leg to the product leg collection
        /// </summary>
        /// <param name="myLeg"></param>
        void AddLeg(Leg myLeg);

        /// <summary>
        /// Add a leg into the leg collection at a
        /// specific position. If a leg already exists at that
        /// position it will be overwritten
        /// </summary>
        /// <param name="myLeg"></param>
        /// <param name="myPosition">0 based postion of the leg in the legs collection</param>
        void AddLeg(Leg myLeg, int myPosition);

        /// <summary>
        /// Get the price implied in the target leg based on an input price for another product
        /// </summary>
        /// <param name="myBaseMnemonic"></param>
        /// <param name="myBasePrice"></param>
        /// <param name="myTargetMnemonic"></param>
        /// <returns></returns>
        double GetImpliedLegPrice(string myBaseMnemonic, double myBasePrice, string myTargetMnemonic);
        /// <summary>
        /// Get the price implied in the target leg based on an input price for another product
        /// </summary>
        /// <param name="myBaseLegIndex"></param>
        /// <param name="myBasePrice"></param>
        /// <param name="myTargetLegIndex"></param>
        /// <returns></returns>
        double GetImpliedLegPrice(string myBaseLegIndex, double myBasePrice, int myTargetLegIndex);
        /// <summary>
        /// Get the price implied in the target leg based on an input price for another product
        /// </summary>
        /// <param name="myBaseLegIndex"></param>
        /// <param name="myBasePrice"></param>
        /// <param name="myTargetLegIndex"></param>
        /// <returns></returns>
        double GetImpliedLegPrice(int myBaseLegIndex, double myBasePrice, int myTargetLegIndex);

        /// <summary>
        /// Get the implied price for the leg and side specified based on the other legs
        /// </summary>
        /// <param name="myBaseLegIndex"></param>
        /// <param name="myStrategySide">Is the spread being bought or sold</param>
        /// <returns></returns>
        double GetImpliedLegPrice(int myBaseLegIndex, string myStrategySide);
        double GetImpliedLegPrice(KaiTrade.Interfaces.Leg myLeg, string myStrategySide);
        double GetImpliedLegSize(int myBaseLegIndex, string myStrategySide);
        double GetImpliedLegSize(KaiTrade.Interfaces.Leg myLeg, string myStrategySide);

        /// <summary>
        /// Return the price and Size for the side andleg index specified
        /// </summary>
        /// <param name="myStrategySide"></param>
        /// <param name="myIndex"></param>
        /// <param name="mySz"></param>
        /// <param name="myPx"></param>
        void GetPairPxSz(string myStrategySide, int myIndex, out double mySz, out double myPx);
        void GetPairPxSz(string myStrategySide, int myIndex, double myLevel, out double mySz, out double myPx);

        /// <summary>
        /// Get a leg based on its mnemonic and side
        /// </summary>
        /// <param name="myLegMnemonic"></param>
        /// <param name="mySide"></param>
        /// <returns>leg or null if not found</returns>
        KaiTrade.Interfaces.Leg GetLeg(string myLegMnemonic, string mySide);

        /// <summary>
        /// remove the leg passed from our list of legs
        /// </summary>
        /// <param name="myLeg"></param>
        void RemoveLeg(Leg myLeg);

        /// <summary>
        /// Create a leg and add it to the leg collection
        /// </summary>
        /// <returns></returns>
        KaiTrade.Interfaces.Leg CreateLeg();

        /// <summary>
        /// Determines if the product is active, false implies the product is not actively traded
        /// </summary>
        bool Active
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
        /// Generate a product mnemonic for this product
        /// </summary>
        /// <returns></returns>
        string GetMnemonic();

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
        /// This is assigned by the trade venue when a product request returns a
        /// sequence of products - it is not persited. The use of this property is for the
        /// driver - do not change elsewhere
        /// </summary>
        long TradeVenueSequence
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
        /// Exchange listing the product - LSE, CME etc
        /// </summary>
        string Exchange
        {
            get;
            set;
        }

        /// <summary>
        /// Execution destination as defined by institution when order is entered - for example SMART for IB
        /// contrast with SecurityExchange where the product is listed
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
        /// Round a price to the correct number of places
        /// </summary>
        /// <param name="myPrice"></param>
        /// <returns></returns>
        double RoundPrice(double myPrice);

        /// <summary>
        /// Price format of the product eg decimal, int, 1/32, 1/64
        /// </summary>
        PriceFormat PriceFormat
        { get; set; }

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
        /// Load from a databinding
        /// </summary>
        /// <param name="myDef"></param>
        /// <param name="myLoadUnderlying"> if true load any underlying products</param>
        void From(KAI.kaitns.Product myDef, bool myLoadUnderlying);

        /// <summary>
        /// Store on a databinding
        /// </summary>
        /// <returns></returns>
        KAI.kaitns.Product ToDataBinding();

        /// <summary>
        /// Set fields from a quickfix message
        /// </summary>
        /// <param name="myMsg"></param>
        void Set(QuickFix.Message myMsg);

        /// <summary>
        /// write product information on a quickfix message
        /// </summary>
        /// <param name="myMsg"></param>
        void From(QuickFix.Message myMsg);

        /// <summary>
        /// Get/Set the price publisher associated with the product
        /// </summary>
        KaiTrade.Interfaces.Publisher Publisher
        {
            get;
            set;
        }

        /// <summary>
        /// Apply a price update to the product
        /// </summary>
        /// <param name="update"></param>
        void ApplyUpdate(KaiTrade.Interfaces.PXUpdate update);

        /// <summary>
        /// Add handler for PXUpdates - local in process use only.
        /// </summary>
        ApplyPXUpdate ApplyPxUpdate
        { get; set; }

        /// <summary>
        /// get set the defult number of depth levels for this product
        /// </summary>
        int DepthLevelCount
        {
            get;
            set;
        }

        /// <summary>
        /// Get an Level 1 price interface for this product
        /// </summary>
        KaiTrade.Interfaces.L1PX L1PX
        { get;}

        /// <summary>
        /// DOM used for price depth
        /// </summary>
        K2DOM DOM
        { get; set; }
    }

    /// <summary>
    /// Used as WCF data contract - this is used to avoid the quickfix dependancies on the main product
    /// </summary>
    public interface ProductData
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
        void From(KaiTrade.Interfaces.TradableProduct p);

        /// <summary>
        /// Set up the properties of a tradeble product from this
        /// </summary>
        /// <param name="p"></param>
        void To(KaiTrade.Interfaces.TradableProduct p);

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
