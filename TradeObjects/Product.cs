//-----------------------------------------------------------------------
// <copyright file="Product.cs" company="KaiTrade LLC">
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
    [JsonObject(MemberSerialization.OptIn)]
    [global::System.Data.Linq.Mapping.TableAttribute(Name = "dbo.Product")]
    public class Product : KaiTrade.Interfaces.IProduct
    {
        private string m_CFICode = "";
        //NOT USED? private bool m_ValidCFICode = false;

        private string m_Currency = "";
        //NOT USED? private bool m_ValidCurrency = false;

        private string m_DriverID = "";
        private string m_Exchange = "";
        //NOT USED? private bool m_ValidExchange = false;

        private string m_SecurityID = "";
        //NOT USED? private bool m_ValidSecurityID = false;

        private string m_IDSource = "";
        //NOT USED? private bool m_ValidIDSource = false;

        private string m_Identity = "";
        private string m_LongName = "";
        private string m_Commodity = "";

        private string m_MMY = "";
        private string m_MaturityDate = "";
        //NOT USED? private bool m_ValidMMY = false;

        private string m_Mnemonic = "";
        private string m_GenericName = "";

        private int m_QtyIncrement = 1;

        private decimal? m_StrikePrice = 0;
        //NOT USED? private bool m_ValidStrikePrice = false;

        private string m_Symbol = "";
        //NOT USED? private bool m_ValidSymbol = false;

        private string m_ExDestination = "";
        //NOT USED? private bool m_ValidExDestination = false;

        /// <summary>
        /// Name of the algo/calc used for synthetic prices based on legs
        /// </summary>
        private string m_SyntheticPriceCalcName;

        private int m_NumberDecimalPlaces = 0;
        private int m_PriceFeedQuantityMultiplier = 0;

        private int m_DepthLevelCount = 0;

        private string m_Tag = "";
        private decimal? m_TickSize = 1;
        private decimal? m_TickValue = 0;
        private decimal? m_ContractSize = 1;
        private string m_TradeVenue = "";

        private string m_BrokerService = "";
        private bool m_Active = true;

        

        public log4net.ILog m_Log = log4net.LogManager.GetLogger("Kaitrade");

        #region TradableProduct Members

        public Product()
        {
            m_Identity = System.Guid.NewGuid().ToString();
        }

        /// <summary>
        /// set this up from a tradeable product
        /// </summary>
        /// <param name="p"></param>
        public void From(KaiTrade.Interfaces.IProduct p)
        {
            try
            {
                Mnemonic = p.Mnemonic;
                Active = p.Active;
                CFICode = p.CFICode;
                Exchange = p.Exchange;
                IDSource = p.IDSource;
                DriverID = p.DriverID;
                LongName = p.LongName;
                Commodity = p.Commodity;
                MMY = p.MMY;
                MaturityDate = p.MaturityDate;
                GenericName = p.GenericName;
                SecurityID = p.SecurityID;
                StrikePrice = p.StrikePrice;
                Symbol = p.Symbol;
                Tag = p.Tag;
                TradeVenue = p.TradeVenue;
                BrokerService = p.BrokerService;
                ExDestination = p.ExDestination;
                Currency = p.Currency;
                ContractSize = p.ContractSize;
                TickSize = p.TickSize;
                NumberDecimalPlaces = p.NumberDecimalPlaces;
                NumberDecimalPlaces = 0;
                PriceFeedQuantityMultiplier = p.PriceFeedQuantityMultiplier;
                if (PriceFeedQuantityMultiplier == 0)
                {
                    PriceFeedQuantityMultiplier = 1;
                }
                TickValue = p.TickValue;
                SyntheticPriceCalcName = p.SyntheticPriceCalcName;
                Identity = p.Identity;
            }
            catch (Exception myE)
            {
                m_Log.Error("GetMnemonic", myE);
            }
        }
        /// <summary>
        /// Set up the properties of a tradeble product from this
        /// </summary>
        /// <param name="p"></param>
        public void To(KaiTrade.Interfaces.IProduct p)
        {
            try
            {
                p.Mnemonic = Mnemonic;
                p.Active = Active;
                p.CFICode = CFICode;
                p.Exchange = Exchange;
                p.IDSource = IDSource;
                p.DriverID = DriverID;
                p.LongName = LongName;
                p.Commodity = Commodity;
                p.MMY = MMY;
                p.MaturityDate = MaturityDate;
                p.GenericName = GenericName;
                p.SecurityID = SecurityID;
                p.StrikePrice = StrikePrice;
                p.Symbol = Symbol;
                p.Tag = Tag;
                p.TradeVenue = TradeVenue;
                p.BrokerService = BrokerService;
                p.ExDestination = ExDestination;
                p.Currency = Currency;
                p.ContractSize = ContractSize;
                p.TickSize = TickSize;
                p.NumberDecimalPlaces = NumberDecimalPlaces;
                NumberDecimalPlaces = 0;
                if (PriceFeedQuantityMultiplier == 0)
                {
                    PriceFeedQuantityMultiplier = 1;
                }
                p.PriceFeedQuantityMultiplier = PriceFeedQuantityMultiplier;

                p.TickValue = TickValue;
                p.SyntheticPriceCalcName = SyntheticPriceCalcName;
                p.Identity = Identity;
            }
            catch (Exception myE)
            {
                m_Log.Error("GetMnemonic", myE);
            }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public bool Active
        {
            get { return m_Active; }
            set { m_Active = value; }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(16)")]
        public string CFICode
        {
            get
            {
                return m_CFICode;
            }
            set
            {
                m_CFICode = value;
               //NOT USED? m_ValidCFICode = true;
            }
        }
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(3)")]
        public string Currency
        {
            get
            {
                return m_Currency;
            }
            set
            {
                m_Currency = value;
               //NOT USED? m_ValidCurrency = true;
            }
        }
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(255)")]
        public string DriverID
        {
            get
            {
                return m_DriverID;
            }
            set
            {
                m_DriverID = value;
            }
        }
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(255)")]
        public string Exchange
        {
            get
            {
                return m_Exchange;
            }
            set
            {
                m_Exchange = value;
               //NOT USED? m_ValidExchange = true;
            }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(255)")]
        public string IDSource
        {
            get
            {
                return m_IDSource;
            }
            set
            {
                m_IDSource = value;
               //NOT USED? m_ValidIDSource = true;
            }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(36)")]
        public string Identity
        {
            get { return m_Identity; }
            set { m_Identity = value; }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(512)")]
        public string LongName
        {
            get
            {
                return m_LongName;
            }
            set
            {
                m_LongName = value;
            }
        }
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(255)")]
        public string Commodity
        {
            get
            {
                return m_Commodity;
            }
            set
            {
                m_Commodity = value;
            }
        }
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(32)")]
        public string MMY
        {
            get
            {
                return m_MMY;
            }
            set
            {
                m_MMY = value;
               //NOT USED? m_ValidMMY = true;
            }
        }
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(32)")]
        public string MaturityDate
        {
            get
            {
                return m_MaturityDate;
            }
            set
            {
                m_MaturityDate = value;
               //NOT USED? m_ValidMMY = true;
            }
        }
        
        /// <summary>
        /// get/set Generic name for the product, this is driver specific
        /// for example in CQG EP refers to the current eMini contract
        ///
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(255)")]
        public string GenericName
        {
            get
            {
                return m_GenericName;
            }
            set
            {
                m_GenericName = value;
            }
        }
        [DataMember]
        [JsonProperty]
        [global::System.Data.Linq.Mapping.ColumnAttribute(Name = "[Mnemonic]", Storage = "m_Mnemonic", DbType = "NVarChar(255) NOT NULL", CanBeNull = false, IsPrimaryKey = true)]
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
        public int QtyIncrement
        {
            get
            {
                return m_QtyIncrement;
            }
            set
            {
                m_QtyIncrement = value;
            }
        }
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(255)")]
        public string SecurityID
        {
            get
            {
                return m_SecurityID;
            }
            set
            {
                m_SecurityID = value;
               //NOT USED? m_ValidSecurityID = true;
            }
        }
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public decimal? StrikePrice
        {
            get
            {
                return m_StrikePrice;
            }
            set
            {
                m_StrikePrice = value;
               //NOT USED? m_ValidStrikePrice = true;
            }
        }
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(255)")]
        public string Symbol
        {
            get
            {
                return m_Symbol;
            }
            set
            {
                m_Symbol = value;
               //NOT USED? m_ValidSymbol = true;
            }
        }
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(512)")]
        public string Tag
        {
            get
            {
                return m_Tag;
            }
            set
            {
                m_Tag = value;
            }
        }
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public decimal? TickSize
        {
            get
            {
                return m_TickSize;
            }
            set
            {
                m_TickSize = value;
            }
        }
        /// <summary>
        /// Product TickValue
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public decimal? TickValue
        {
            get
            {
                return m_TickValue;
            }
            set
            {
                m_TickValue = value;
            }
        }

        /// <summary>
        /// return the number of decimal places
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public int NumberDecimalPlaces
        {
            get { return m_NumberDecimalPlaces; }
            set { m_NumberDecimalPlaces = value; }
        }
        /// <summary>
        /// A multiplier applied to qty (Bid/Offer..) received from the venue
        /// for example for IB this should be 100 for stocks
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public int PriceFeedQuantityMultiplier
        {
            get { return m_PriceFeedQuantityMultiplier; }
            set { m_PriceFeedQuantityMultiplier = value; }
        }
        /// <summary>
        /// Product Contract Size
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public decimal? ContractSize
        {
            get
            {
                return m_ContractSize;
            }
            set
            {
                m_ContractSize = value;
            }
        }
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(255)")]
        public string TradeVenue
        {
            get
            {
                return m_TradeVenue;
            }
            set
            {
                m_TradeVenue = value;
            }
        }
        /// <summary>
        /// The broker service - is used by adapters to determine a service within
        /// a particular venue
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(255)")]
        public string BrokerService
        { get { return m_BrokerService; } set { m_BrokerService = value; } }

        #endregion

        #region TradableProduct Members

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(255)")]
        public string ExDestination
        {
            get
            {
                return m_ExDestination;
            }
            set
            {
                m_ExDestination = value;
               //NOT USED? m_ValidExDestination = true;
            }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public int DepthLevelCount
        {
            get { return m_DepthLevelCount; }
            set
            {
                m_DepthLevelCount = value;
            }
        }

        #endregion

        /// <summary>
        /// get/set the name of the price calculation to use on legs - if needed
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(255)")]
        public string SyntheticPriceCalcName
        {
            get { return m_SyntheticPriceCalcName ?? ""; }
            set
            {
                m_SyntheticPriceCalcName = value ?? "";
            }
        }
    }
}
