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
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace KaiTrade.Interfaces
{
    /// <summary>
    /// used to show the atate of the source of pxupdates
    /// </summary>
    public enum PXSourceState { stopped, running, error, endOfData , none };

    /// <summary>
    /// Determines the type of depth operation on an update
    /// </summary>
     public enum PXDepthOperation { insert, delete, update, none };

    /// <summary>
    /// Type of update
    /// </summary>
     public enum PXUpdateType { bidask, bid, ask, trade, image, none };

     public delegate void PXUpdated(PXUpdate pxUpdate);
     public delegate void PXSourceStateChanged(PXSourceState state);

    public class PXFields
    {
        public const int SEQNUMBER = 0;
        public const int TIMETICKS = 1;
        public const int OFFERPRICE = 2;
        public const int OFFERSIZE = 3;
        public const int BIDSIZE = 4;
        public const int BIDPRICE = 5;
        public const int TRADEPRICE = 6;
        public const int TRADEVOLUME = 7;
        public const int OFFERPRICEDELTA = 8;
        public const int OFFERSIZEDELTA = 9;
        public const int BIDSIZEDELTA = 10;
        public const int BIDPRICEDELTA = 11;
        public const int TRADEPRICEDELTA = 12;
        public const int TRADEVOLUMEDELTA = 13;
        public const int DAYHIGH = 14;
        public const int DAYLOW = 15;

        public const int MAXPXFIELDS = 17;
    }

    /// <summary>
    /// This interface represents a price update - note that this is often applied to the L1PX interface
    /// and that its possible to get updates containing the same values for some of the fields
    /// where some price source soes not explicitly produce just the changed price fields
    /// </summary>
    public interface PXUpdate
    {
        /// <summary>
        /// Provider of the update - normally the driver ID
        /// </summary>
        string Originator
        {
            get;
            set;
        }

        /// <summary>
        /// Sequence number of the update - this is unique within a particular susbcription
        /// </summary>
        long Sequence
        {
            get;
            set;
        }

        /// <summary>
        /// Time in ticks - local time update received
        /// </summary>
        long Ticks
        {
            get;
            set;
        }

        /// <summary>
        /// Defines the type of price update
        /// </summary>
        PXUpdateType UpdateType
        { get; set; }

        /// <summary>
        /// Time in ticks of the update from the server - this is not always available
        /// </summary>
        long? ServerTicks
        {
            get;
            set;
        }

        /// <summary>
        /// Product mnemonic
        /// </summary>
        string Mnemonic
        {
            get;
            set;
        }

        /// <summary>
        /// Market (MarketMaker) offering a depth entry
        /// </summary>
        string DepthMarket
        {
            get;
            set;
        }
         /// <summary>
         /// position of the depth  0 ... +N, not that the use of bid or offer price
         /// implies the side
         /// </summary>
        int DepthPosition
        {
            get;
            set;
        }

        /// <summary>
        /// Get set the depth operation - i.e insert, replace delete
        /// </summary>
        PXDepthOperation DepthOperation
        {
            get;
            set;
        }

        /// <summary>
        /// Tag/data that can be added by a driver
        /// </summary>
        string DriverTag
        {
            get;
            set;
        }

        decimal? OfferPrice
        {
            get;
            set;
        }

        decimal? OfferSize
        {
            get;
            set;
        }

        decimal? BidSize
        {
            get;
            set;
        }

        decimal? BidPrice
        {
            get;
            set;
        }

        decimal? TradePrice
        {
            get;
            set;
        }

        decimal? TradeVolume
        {
            get;
            set;
        }

        decimal? DayHigh
        {
            get;
            set;
        }

        decimal? DayLow
        {
            get;
            set;
        }

        /// <summary>
        /// Current delta
        /// </summary>
        decimal? OfferPriceDelta
        {
            get;
            set;
        }

        /// <summary>
        /// Curreent delta
        /// </summary>
        decimal? OfferSizeDelta
        {
            get;
            set;
        }

        decimal? BidSizeDelta
        {
            get;
            set;
        }

        decimal? BidPriceDelta
        {
            get;
            set;
        }
        decimal? TradePriceDelta
        {
            get;
            set;
        }
        decimal? TradeVolumeDelta
        {
            get;
            set;
        }

        void CalculateDeltas(PXUpdate prevUpdate);

        /// <summary>
        /// Sets the current update using an existing update
        /// </summary>
        /// <param name="update"></param>
        void From(KaiTrade.Interfaces.PXUpdate update);

        void From(string myMnemonic, KaiTrade.Interfaces.L1PX myL1PX);

        void To(KaiTrade.Interfaces.L1PX myL1PX);

        void From(string myData, char myDelimiter);

        string To( char myDelimiter);
    }
}
