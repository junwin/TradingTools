//-----------------------------------------------------------------------
// <copyright file="TradeVenue.cs" company="KaiTrade LLC">
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
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Newtonsoft.Json;

namespace K2DataObjects
{
    [DataContract]
    [KnownType(typeof(K2DataObjects.MQExchange))]
    [KnownType(typeof(K2DataObjects.MQRoutingKey))]
    [JsonObject(MemberSerialization.OptIn)]
    [global::System.Data.Linq.Mapping.TableAttribute(Name = "dbo.Venue")]
    public class TradeVenue : KaiTrade.Interfaces.IVenue
    {
        /// <summary>
        /// Venue code
        /// </summary>
        private string m_Code = "";

        /// <summary>
        /// Target venue - used when this venue routes messages
        /// </summary>
        private string m_TargetVenue = "";

        /// <summary>
        /// If specified the venue used to get data (realtime and historic) for
        /// intruments in this venue - used when a venue only provides order routing
        /// and a separate venue is used to get prices
        /// </summary>
        private string m_DataFeedVenue = "";

        /// <summary>
        /// driver code that process messages
        /// </summary>
        private string m_DriverCode = "";

        /// <summary>
        /// Name of venue for display purposes
        /// </summary>
        private string m_Name = "";

        /// <summary>
        /// Account number used for orders
        /// </summary>
        private string m_AccountNumber = "";

        /// <summary>
        /// A bag of extra fields that will be added to new order single
        /// message
        /// </summary>
        private string m_NOSBag = "";

        /// <summary>
        /// A bag of extra fields that will be added to cancel order message
        /// </summary>
        private string m_CancelBag = "";

        /// <summary>
        /// A bag of extra fields that will be added to replace order message
        /// </summary>
        private string m_ReplaceBag = "";

        /// <summary>
        /// FIX begin string
        /// </summary>
        private string m_BeginString = "";
        /// <summary>
        /// FIX Target comp ID
        /// </summary>
        private string m_TID = "";

        /// <summary>
        /// FIX Sender Comp ID
        /// </summary>
        private string m_SID = "";

        /// <summary>
        /// default currency code for the venue
        /// </summary>
        private string m_DefaultCurrencyCode = "";

        /// <summary>
        /// get/set the default security exchange for this venue
        /// </summary>
        private string m_DefaultSecurityExchange = "";

        /// <summary>
        /// get/set the default CFICode(product type)
        /// </summary>
        private string m_DefaultCFICode = "ESXXXX";

        /// <summary>
        /// If set then a product id (srcid) is used as a symbol - for venues that just use a symbol code
        /// </summary>
        private bool m_UseSymbol = false;

        /// <summary>
        /// Get or Set the list of exchanges associated with the venue
        /// </summary>
        private List<KaiTrade.Interfaces.IMQExchange> m_Exchange;
         

        /// <summary>
        /// Get or Set the list of routing keys associated with the venue
        /// </summary>
        private List<KaiTrade.Interfaces.IMQRoutingKey> m_RoutingKey;
 


        // Create a logger for use in this class
        public log4net.ILog m_Log;

        public TradeVenue()
        {
            m_Log = log4net.LogManager.GetLogger("Kaitrade");
            m_Exchange = new List<KaiTrade.Interfaces.IMQExchange>();
            m_RoutingKey = new List<KaiTrade.Interfaces.IMQRoutingKey>();
        }

        #region Venue Members

        /// <summary>
        /// Account number used for orders
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(256)")]
        public string AccountNumber
        {
            get
            {
                return m_AccountNumber;
            }
            set
            {
                m_AccountNumber = value;
            }
        }

        //    [System.Data.Linq.Mapping.Column]
        [DataMember]
        [JsonProperty]
        public List<KaiTrade.Interfaces.IMQExchange>  MessageExchange
        {
            get
            {
                return m_Exchange;
            }
            set
            {
                m_Exchange = value;
            }
        }

        //[System.Data.Linq.Mapping.Column]
        [DataMember]
        [JsonProperty]
        public List<KaiTrade.Interfaces.IMQRoutingKey> RoutingKey
        {
            get
            {
                return m_RoutingKey;
            }
            set
            {
                m_RoutingKey = value;
            }
        }

        /// <summary>
        /// FIX begin string
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(256)")]
        public string BeginString
        {
            get
            {
                return m_BeginString;
            }
            set
            {
                m_BeginString = value;
            }
        }

        /// <summary>
        /// A bag of extra fields that will be added to cancel order message
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(2048)")]
        public string CancelBag
        {
            get
            {
                return m_CancelBag;
            }
            set
            {
                m_CancelBag = value;
            }
        }

        
        /// <summary>
        /// Venue code - should be unique
        /// </summary>
        [DataMember]
        [JsonProperty]
        [global::System.Data.Linq.Mapping.ColumnAttribute(Name = "[Code]", Storage = "m_Code", DbType = "NVarChar(256) NOT NULL", CanBeNull = false, IsPrimaryKey = true)]
        public string Code
        {
            get
            {
                return m_Code;
            }
            set
            {
                m_Code = value;
            }
        }

        /// <summary>
        /// Target venue - when a trade venue simply routes messages
        /// this is the actual venue they want it to trade on
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(256)")]
        public string TargetVenue
        {
            get
            {
                return m_TargetVenue;
            }
            set
            {
                m_TargetVenue = value;
            }
        }

        /// <summary>
        /// If specified the venue used to get data (realtime and historic) for
        /// intruments in this venue - used when a venue only provides order routing
        /// and a separate venue is used to get prices
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(256)")]
        public string DataFeedVenue
        {
            get
            {
                return m_DataFeedVenue;
            }
            set
            {
                m_DataFeedVenue = value;
            }
        }

        /// <summary>
        /// driver code that process messages for this venue
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(256)")]
        public string DriverCode
        {
            get
            {
                return m_DriverCode;
            }
            set
            {
                m_DriverCode = value;
            }
        }

        /// <summary>
        /// A bag of extra fields that will be added to new order single  message
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(2048)")]
        public string NOSBag
        {
            get
            {
                return m_NOSBag;
            }
            set
            {
                m_NOSBag = value;
            }
        }

        /// <summary>
        /// Name of venue for display purposes
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(256)")]
        public string Name
        {
            get
            {
                return m_Name;
            }
            set
            {
                m_Name = value;
            }
        }

        /// <summary>
        /// A bag of extra fields that will be added to cancel replace order message
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(2048)")]
        public string ReplaceBag
        {
            get
            {
                return m_ReplaceBag;
            }
            set
            {
                m_ReplaceBag = value;
            }
        }

        /// <summary>
        /// FIX sender comp id - for fix driver
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(256)")]
        public string SID
        {
            get
            {
                return m_SID;
            }
            set
            {
                m_SID = value;
            }
        }

        /// <summary>
        /// FIX sender target id - for fix driver
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(256)")]
        public string TID
        {
            get
            {
                return m_TID;
            }
            set
            {
                m_TID = value;
            }
        }

        /// <summary>
        /// get/set the default currency code
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(3)")]
        public string DefaultCurrencyCode
        {
            get
            {
                return m_DefaultCurrencyCode;
            }
            set
            {
                m_DefaultCurrencyCode = value;
            }
        }
        /// <summary>
        /// get/set the default security exchange for this venue
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(256)")]
        public string DefaultSecurityExchange
        {
            get
            {
                return m_DefaultSecurityExchange;
            }
            set
            {
                m_DefaultSecurityExchange = value;
            }
        }

        /// <summary>
        /// get/set the default CFICode(product type)
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(8)")]
        public string DefaultCFICode
        {
            get
            {
                return m_DefaultCFICode;
            }
            set
            {
                m_DefaultCFICode = value;
            }
        }

        /// <summary>
        /// If set then a product id (srcid) is used as a symbol - for venues that just use a symbol code
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public bool UseSymbol
        {
            get
            {
                return m_UseSymbol;
            }
            set
            {
                m_UseSymbol = value;
            }
        }

        /// <summary>
        /// Set the venue up from a databinding
        /// </summary>
        /// <param name="myVenue"></param>
        public void FromXml(string myVenue)
        { }

        /// <summary>
        /// Write to an xml databinding
        /// </summary>
        public string AsXML()
        { return ""; }

        

       

        #endregion
    }
}
