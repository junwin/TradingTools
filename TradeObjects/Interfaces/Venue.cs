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
    /// This defines a trade venue that provides execution through some API
    /// </summary>
    public interface Venue
    {
        /// <summary>
        ///  long name for the venue - for display purposes
        /// </summary>
        string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Code used to ID the venue
        /// </summary>
        string Code
        {
            get;
            set;
        }

        /// <summary>
        /// Target venue - when a trade venue simply routes messages
        /// this is the actual venue they want it to trade on
        /// </summary>
        string TargetVenue
        {
            get;
            set;
        }

        /// <summary>
        /// If specified the venue used to get data (realtime and historic) for
        /// intruments in this venue - used when a venue only provides order routing
        /// and a separate venue is used to get prices
        /// </summary>
        string DataFeedVenue
        {
            get;
            set;
        }

        /// <summary>
        /// Driver code that gets messages for this venue
        /// </summary>
        string DriverCode
        {
            get;
            set;
        }

        /// <summary>
        /// FIX Begin string - used for FIX Driver
        /// </summary>
        string BeginString
        {
            get;
            set;
        }

        /// <summary>
        /// FIX Sender compID - used for FIX Driver
        /// </summary>
        string SID
        {
            get;
            set;
        }

        /// <summary>
        /// FIX Target comp ID - used for FIX Driver
        /// </summary>
        string TID
        {
            get;
            set;
        }

        /// <summary>
        /// Account number used for orders on this venue
        /// </summary>
        string AccountNumber
        {
            get;
            set;
        }

        /// <summary>
        /// A bag of delimited fields that will be added to new order single
        /// message
        /// </summary>
        string NOSBag
        {
            get;
            set;
        }

        /// <summary>
        /// A bag of delimited  fields that will be added to cancel order message
        /// </summary>
        string CancelBag
        {
            get;
            set;
        }
        /// <summary>
        /// A bag of  delimited  fields that will be added to replace order message
        /// </summary>
        string ReplaceBag
        {
            get;
            set;
        }

        /// <summary>
        /// get/set the default currency code
        /// </summary>
        string DefaultCurrencyCode
        {
            get;
            set;
        }

        /// <summary>
        /// get/set the default security exchange for this venue
        /// </summary>
        string DefaultSecurityExchange
        {
            get;
            set;
        }

        /// <summary>
        /// get/set the default CFICode(product type)
        /// </summary>
        string DefaultCFICode
        {
            get;
            set;
        }

        /// <summary>
        /// If set then a product id (srcid) is used as a symbol - for venues that just use a symbol code
        /// </summary>
        bool UseSymbol
        {
            get;
            set;
        }

        /// <summary>
        /// Get or Set the list of message exchanges associated with the venue
        /// </summary>
        List<KaiTrade.Interfaces.IMQExchange> MessageExchange
        { get; set; }

        /// <summary>
        /// Get or Set the list of routing keys associated with the venue
        /// </summary>
        List<KaiTrade.Interfaces.IMQRoutingKey> RoutingKey
        { get; set; }

        /// <summary>
        /// Set the venue up from a databinding
        /// </summary>
        /// <param name="myVenue"></param>
        void FromXml(string myVenue);

        /// <summary>
        /// Write to an xml databinding
        /// </summary>
        string AsXML();
        /*
        /// <summary>
        /// Set the venue up from a databinding
        /// </summary>
        /// <param name="myVenue"></param>
        void FromXMLDB(KAI.kaitns.Venue myVenue);

        /// <summary>
        /// Write to an xml databinding
        /// </summary>
        KAI.kaitns.Venue ToXMLDB();
         */
    }

    /// <summary>
    /// This defines a trade venue data object that provides execution through some API
    /// </summary>
    public interface XXVenueData
    {
        /// <summary>
        ///  long name for the venue - for display purposes
        /// </summary>
        string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Code used to ID the venue
        /// </summary>
        string Code
        {
            get;
            set;
        }

        /// <summary>
        /// Target venue - when a trade venue simply routes messages
        /// this is the actual venue they want it to trade on
        /// </summary>
        string TargetVenue
        {
            get;
            set;
        }

        /// <summary>
        /// If specified the venue used to get data (realtime and historic) for
        /// intruments in this venue - used when a venue only provides order routing
        /// and a separate venue is used to get prices
        /// </summary>
        string DataFeedVenue
        {
            get;
            set;
        }

        /// <summary>
        /// Driver code that gets messages for this venue
        /// </summary>
        string DriverCode
        {
            get;
            set;
        }

        /// <summary>
        /// FIX Begin string - used for FIX Driver
        /// </summary>
        string BeginString
        {
            get;
            set;
        }

        /// <summary>
        /// FIX Sender compID - used for FIX Driver
        /// </summary>
        string SID
        {
            get;
            set;
        }

        /// <summary>
        /// FIX Target comp ID - used for FIX Driver
        /// </summary>
        string TID
        {
            get;
            set;
        }

        /// <summary>
        /// Account number used for orders on this venue
        /// </summary>
        string AccountNumber
        {
            get;
            set;
        }

        /// <summary>
        /// A bag of delimited fields that will be added to new order single
        /// message
        /// </summary>
        string NOSBag
        {
            get;
            set;
        }

        /// <summary>
        /// A bag of delimited  fields that will be added to cancel order message
        /// </summary>
        string CancelBag
        {
            get;
            set;
        }
        /// <summary>
        /// A bag of  delimited  fields that will be added to replace order message
        /// </summary>
        string ReplaceBag
        {
            get;
            set;
        }

        /// <summary>
        /// get/set the default currency code
        /// </summary>
        string DefaultCurrencyCode
        {
            get;
            set;
        }

        /// <summary>
        /// get/set the default security exchange for this venue
        /// </summary>
        string DefaultSecurityExchange
        {
            get;
            set;
        }

        /// <summary>
        /// get/set the default CFICode(product type)
        /// </summary>
        string DefaultCFICode
        {
            get;
            set;
        }

        /// <summary>
        /// If set then a product id (srcid) is used as a symbol - for venues that just use a symbol code
        /// </summary>
        bool UseSymbol
        {
            get;
            set;
        }
    }
}
