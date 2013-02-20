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
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Newtonsoft.Json;

namespace K2DataObjects
{
    [DataContract]
    [KnownType(typeof(K2DataObjects.TradeSignal))]
    [KnownType(typeof(K2DataObjects.Parameter))]
    [JsonObject(MemberSerialization.OptIn)]
    [global::System.Data.Linq.Mapping.TableAttribute(Name = "dbo.PriceBar")]
    public class TSPrice : KaiTrade.Interfaces.TSPrice
    {
        // Defined date values
        private string m_Mnemonic;
        KaiTrade.Interfaces.TSItemType m_ItemType = KaiTrade.Interfaces.TSItemType.none;
        private string m_RequestID = "";
        private decimal m_Avg = 0;
        private decimal m_HLC3 = 0;
        private decimal m_High = 0;
        private decimal m_Low = 0;
        private decimal m_Open = 0;
        private decimal m_Close = 0;
        private long m_Index = -1;
        private decimal m_Mid = 0;
        private long m_TimeStamp = DateTime.Now.Ticks;
        private decimal m_Range = 0;
        private decimal m_TrueRange = 0;
        private decimal m_TrueHigh = 0;
        private decimal m_TrueLow = 0;
        private decimal m_AskVolume = 0;
        private decimal m_BidVolume = 0;
        private decimal m_Volume = 0;

        /// <summary>
        /// Create a logger for use in this Driver
        /// </summary>
        public log4net.ILog m_Log;

        public TSPrice()
        {
            // Set up logging - will participate in the standard toolkit log
            m_Log = log4net.LogManager.GetLogger("KaiTrade");
        }


        /// <summary>
        /// Defines what type of item this is e.g. timed bar, constant volume bar
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public KaiTrade.Interfaces.TSItemType ItemType
        {
            get { return m_ItemType; }
            set { m_ItemType = value; }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(36)")]
        public string RequestID
        {
            get { return m_RequestID; }
            set { m_RequestID = value; }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(255)")]
        public string Mnemonic
        {
            get
            {
                return m_Mnemonic;
            }
            set
            {
                m_Mnemonic = value;
            }
        }


        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public decimal Volume
        {
            get
            {
                return m_Volume;
            }
            set
            {
                m_Volume = value;
            }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public decimal AskVolume
        {
            get
            {
                return m_AskVolume;
            }
            set
            {
                m_AskVolume = value;
            }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public decimal BidVolume
        {
            get
            {
                return m_BidVolume;
            }
            set
            {
                m_BidVolume = value;
            }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public decimal Avg
        {
            get
            {
                return m_Avg;
            }
            set
            {
                m_Avg = value;
            }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public decimal Close
        {
            get
            {
                return m_Close;
            }
            set
            {
                m_Close = value;
            }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public decimal HLC3
        {
            get
            {
                return m_HLC3;
            }
            set
            {
                m_HLC3 = value;
            }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public decimal High
        {
            get
            {
                return m_High;
            }
            set
            {
                m_High = value;
            }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public long Index
        {
            get
            {
                return m_Index;
            }
            set
            {
                m_Index = value;
            }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public decimal Low
        {
            get
            {
                return m_Low;
            }
            set
            {
                m_Low = value;
            }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public decimal Mid
        {
            get
            {
                return m_Mid;
            }
            set
            {
                m_Mid = value;
            }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public decimal Open
        {
            get
            {
                return m_Open;
            }
            set
            {
                m_Open = value;
            }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public decimal Range
        {
            get
            {
                return m_Range;
            }
            set
            {
                m_Range = value;
            }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public long TimeStamp
        {
            get
            {
                return m_TimeStamp;
            }
            set
            {
                m_TimeStamp = value;
            }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public decimal TrueHigh
        {
            get
            {
                return m_TrueHigh;
            }
            set
            {
                m_TrueHigh = value;
            }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public decimal TrueLow
        {
            get
            {
                return m_TrueLow;
            }
            set
            {
                m_TrueLow = value;
            }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public decimal TrueRange
        {
            get
            {
                return m_TrueRange;
            }
            set
            {
                m_TrueRange = value;
            }
        }

    }
}
