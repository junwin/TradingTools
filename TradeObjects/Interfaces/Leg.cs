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
    /// Defines the Leg price update delegate
    /// </summary>
    /// <param name="myPX"></param>
    public delegate void LegPxUpdate(KaiTrade.Interfaces.Leg myLeg, KaiTrade.Interfaces.L1PX myPX);

    /// <summary>
    /// Defines a leg of some tradeable product, note that a leg only occurs in one product
    /// </summary>
    public interface Leg
    {
        /// <summary>
        /// get/set handles for a legpx update
        /// </summary>
        LegPxUpdate PxUpdate
        {
            get;
            set;
        }

        /// <summary>
        /// get the identity for the leg
        /// </summary>
        string Identity
        { get;}

        /// <summary>
        /// Start to subscribe to prices for the leg product
        /// </summary>
        void SubscribePX();

        /// <summary>
        /// Get/Set the leg Mnemonic
        /// </summary>
        string Mnemonic
        {
            get;
            set;
        }

        /// <summary>
        /// Get the product associated with this leg
        /// </summary>
        TradableProduct Product
        {
            get;
        }

        /// <summary>
        /// get/set the position index of this leg in the products list of legs
        /// used to optimise performance
        /// </summary>
        int PositionIndex
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set the parent product associated with this leg
        /// </summary>
        TradableProduct Parent
        {
            get;
            set;
        }

        /// <summary>
        /// get/set the account used for this leg
        /// </summary>
        string AccountCode
        {
            get;
            set;
        }

        /// <summary>
        /// Type of spread
        /// </summary>
        string Type
        {
            get;
            set;
        }

        /// <summary>
        /// Side of the leg
        /// </summary>
        string Side
        {
            get;
            set;
        }

        /// <summary>
        /// If the side is Short Sell this needs to be used
        /// to specify where the products are held
        /// </summary>
        string ShortSaleLocate
        {
            get;
            set;
        }

        /// <summary>
        /// Order type of the leg
        /// </summary>
        string OrdType
        {
            get;
            set;
        }

        /// <summary>
        /// Is this leg quoted in the market
        /// </summary>
        bool Quoted
        {
            get;
            set;
        }
        /// <summary>
        /// Price for the leg
        /// </summary>
        decimal? Price
        {
            get;
            set;
        }

        /// <summary>
        /// Offset to applied to price - used for spreads
        /// </summary>
        decimal PriceTickOffset
        {
            get;
            set;
        }
        /// <summary>
        /// Spread Offset to applied to price as an absolute value- used for spreads
        /// </summary>
        decimal SpreadOffset
        {
            get;
        }

        /// <summary>
        /// Leg Tick size - this should come from the product - but we *must* have one here
        /// </summary>
        decimal TickSize
        { get; set;}

        /// <summary>
        /// return the number of decimal places
        /// </summary>
        int NumberDecimalPlaces
        {
            get;
            set;
        }
        /// <summary>
        /// Factor to dampen our reaction to price changes 0 => no dampening
        /// </summary>
        decimal PriceDampeningFactor
        {
            get;
            set;
        }

        /// <summary>
        /// Quantity for the leg
        /// </summary>
        decimal? Quantity
        {
            get;
            set;
        }

        /// <summary>
        /// Leg multiplier -used to match say Barrel versus Gallons
        /// </summary>
        decimal Multiplier
        {
            get;
            set;
        }

        /// <summary>
        /// Ratio for the leg to the sum of Legs in a product
        /// calculated based on Qtys
        /// </summary>
        decimal Ratio
        {
            get;
            set;
        }

        /// <summary>
        /// get a L1PX interface for the leg
        /// </summary>
        KaiTrade.Interfaces.L1PX L1PX
        { get;}

        /// <summary>
        /// Set up a leg from an XML data binding
        /// </summary>
        /// <param name="myLeg"></param>
        /// <param name="myLoadProduct">If true load and underlying legs specified</param>
        void FromXMLDB(KAI.kaitns.Leg myLeg, bool myLoadProduct);

        /// <summary>
        /// write a leg onto an XML data bining
        /// </summary>
        /// <returns></returns>
        KAI.kaitns.Leg ToXMLDB();
    }
}
