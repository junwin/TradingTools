//-----------------------------------------------------------------------
// <copyright file="Order.cs" company="KaiTrade LLC">
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
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Linq;
using Newtonsoft.Json;

namespace K2DataObjects
{
    [DataContract]
    [KnownType(typeof(K2DataObjects.Parameter))]
    [global::System.Data.Linq.Mapping.TableAttribute(Name = "dbo.Order")]
    [JsonObject(MemberSerialization.OptIn)]
    public class Order : KaiTrade.Interfaces.IOrder
    {
        private string m_Identity;
        private string m_ParentIdentity;
        private string m_UserID = "";
        private string m_SessionID = "";
        private string m_TriggerOrderID = "";
        private int m_AutoTradeProcessCount = 0;
        private string m_Mnemonic = "";

        /// <summary>
        /// THis is the system id for a particular product
        /// </summary>
        private string m_ProductIdentity;

       

        private string m_Account = "";

        /// <summary>
        /// This is the identity of the account - rather than the text name/code
        /// </summary>
        private string m_AccountIdentity = "";

        
        private string m_HandlInst = "";
        private string m_ClOrdID = "";
        private string m_ClientAssignedID="";
        private string m_OrigClOrdID = "";
        private string m_OrderID = "";
        private string m_Side = "";
        private string m_ShortSaleLocate = "";
        private string m_LocationID;
        private string m_LocateReqd;
        private string m_OrdType = "";
        private string m_ExtendedOrdType = "";
        private string[] m_ExtendedOrdTypeParameters = null;
        private double m_OrderQty = 0;
        private double m_MaxFloor = 0;
        private double m_Price = 0;
        private double m_StopPx = 0;
        private string m_TimeInForce = "";
        private string m_ExpireDate = "";
        private string m_OrdStatus = "";
        private double m_LeavesQty = 0;
        private double m_CumQty = 0;
        private double m_LastPx = 0;
        private double m_LastQty = 0;
        private double m_AvgPx = 0;
        private string m_Text = "";
        private string m_TransactTime = "";
        private long m_Expiration = 0;

        private KaiTrade.Interfaces.LastOrderCommand _lastOrderCommand = KaiTrade.Interfaces.LastOrderCommand.none;

        //private List<KaiTrade.Interfaces.Fill> m_FillsList = new List<KaiTrade.Interfaces.Fill>();

        private string m_Description;
        //private List<KaiTrade.Interfaces.Field> m_CancelBag;
        //private List<KaiTrade.Interfaces.Field> m_NOSBag;
        //private List<KaiTrade.Interfaces.Field> m_ReplaceBag;

        private string m_OCAGroupName = "";
        private bool m_AETrade = false;
        //NOT USED? private double m_QuantityDelta = 0.0;
        private double m_QuantityLimit = 0.0;
        private string m_StrategyName = "";
        private string m_VenueCode = "";
        private string m_Tag = "";
        private string m_CorrelationID = "";

        private string m_AlgoName = "";
        private List<KaiTrade.Interfaces.IParameter> m_K2Parameters;
        private DateTime m_SystemDate;

        public Order()
        {
            m_K2Parameters = new List<KaiTrade.Interfaces.IParameter>();
        }

        public void From(KaiTrade.Interfaces.IOrder o)
        {
            Identity = o.Identity;
            ParentIdentity = o.ParentIdentity;
            User = o.User;
            if (o.SessionID != null)
            {
                SessionID = o.SessionID;
            }
            if (o.CorrelationID != null)
            {
                CorrelationID = o.CorrelationID;
            }
            TriggerOrderID = o.TriggerOrderID;
            AutoTradeProcessCount = o.AutoTradeProcessCount;

            //ProductID = o.Pr
            Mnemonic = o.Mnemonic;

            if (o.Account != null)
            {
                Account = o.Account;
            }

            if (o.HandlInst != null)
            {
                HandlInst = o.HandlInst.ToString();
            }

            ClOrdID = o.ClOrdID;

            if (o.OrigClOrdID != null)
            {
                OrigClOrdID = o.OrigClOrdID;
            }

            if (o.ClientAssignedID != null)
            {
                ClientAssignedID = o.ClientAssignedID;
            }

            if (o.OrderID != null)
            {
                OrderID = o.OrderID;
            }

            // note swap the side back to long description
            Side = o.Side;

            ShortSaleLocate = o.ShortSaleLocate;

            if (o.LocationID != null)
            {
                LocationID = o.LocationID;
            }

            if (o.LocateReqd != null)
            {
                LocateReqd = o.LocateReqd.ToString();
            }

            OrdType = o.OrdType;

            ExtendedOrdType = o.ExtendedOrdType;

            ExtendedOrdTypeParameters = o.ExtendedOrdTypeParameters;

            OrderQty = (long)o.OrderQty;
            
            MaxFloor = (long)o.MaxFloor;
            
            Price = o.Price;
           
            StopPx = o.StopPx;
            
            if (o.TimeInForce != null)
            {
                TimeInForce = o.TimeInForce.ToString();
            }
            if (o.ExpireDate != null)
            {
                ExpireDate = o.ExpireDate;
            }
            if (o.OrdStatus != null)
            {
                OrdStatus = o.OrdStatus;
            }
            LeavesQty = o.LeavesQty;
            
            CumQty = o.CumQty;
            
            LastPx = o.LastPx;
            
            LastQty = o.LastQty;
            
            AvgPx = o.AvgPx;
            

            Text = o.Text;
            if (o.TransactTime != null)
            {
                TransactTime = o.TransactTime.ToString();
            }
            Expiration = o.Expiration;

            //private List<KaiTrade.Interfaces.Fill> m_FillsList = new List<KaiTrade.Interfaces.Fill>();

            Description = o.Description;
            //private List<KaiTrade.Interfaces.Field> m_CancelBag;
            //private List<KaiTrade.Interfaces.Field> m_NOSBag;
            //private List<KaiTrade.Interfaces.Field> m_ReplaceBag;

            OCAGroupName = o.OCAGroupName;
            m_AETrade = o.IsAutoTrade;

            QuantityLimit = o.QuantityLimit;
            StrategyName = o.StrategyName;
            TradeVenue = o.TradeVenue;
            Tag = o.Tag;
        }

        //Get/Set user identity (a guid) that the order belongs to
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(36)")]
        public string User
        {
            get { return m_UserID; }
            set { m_UserID = value; }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(36)")]
        public string SessionID
        {
            get { return m_SessionID; }
            set { m_SessionID = value; }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(256)")]
        public string CorrelationID
        {
            get { return m_CorrelationID; }
            set { m_CorrelationID = value; }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(256)")]
        public string Account
        {
            get
            {
                return m_Account;
            }
            set
            {
                m_Account = value;
            }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(36)")]
        public string AccountIdentity
        {
            get { return m_AccountIdentity; }
            set { m_AccountIdentity = value; }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public int AutoTradeProcessCount
        {
            get
            {
                return m_AutoTradeProcessCount;
            }
            set
            {
                m_AutoTradeProcessCount = value;
            }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public double AvgPx
        {
            get
            {
                return m_AvgPx;
            }
            set
            {
                m_AvgPx = value;
            }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(512)")]
        public string ClOrdID
        {
            get
            {
                return m_ClOrdID;
            }
            set
            {
                m_ClOrdID = value;
            }
        }

        /// <summary>
        /// Gets or Sets an identity assigned by the user (trader, app) to identify
        /// an  order
        /// </summary>
        /// <value>The user assigned  order id.</value>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(256)")]
        public string ClientAssignedID
        {
            get
            {
                return m_ClientAssignedID;
            }
            set
            {
                m_ClientAssignedID = value;
            }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public double CumQty
        {
            get
            {
                return m_CumQty;
            }
            set
            {
                m_CumQty = value;
            }
        }
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(512)")]
        public string Description
        {
            get
            {
                return m_Description;
            }
            set
            {
                m_Description = value;
            }
        }
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public long Expiration
        {
            get
            {
                return m_Expiration;
            }
            set
            {
                m_Expiration = value;
            }
        }
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public string ExpireDate
        {
            get
            {
                return m_ExpireDate;
            }
            set
            {
                m_ExpireDate = value;
            }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(255)")]
        public string ExtendedOrdType
        {
            get
            {
                return m_ExtendedOrdType;
            }
            set
            {
                m_ExtendedOrdType = value;
            }
        }

        [DataMember]
        [JsonProperty]
        public string[] ExtendedOrdTypeParameters
        {
            get
            {
                return m_ExtendedOrdTypeParameters;
            }
            set
            {
                m_ExtendedOrdTypeParameters = value;
            }
        }
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(32)")]
        public string HandlInst
        {
            get
            {
                return m_HandlInst;
            }
            set
            {
                m_HandlInst = value;
            }
        }

        [DataMember]
        [JsonProperty]
        [global::System.Data.Linq.Mapping.ColumnAttribute(Name = "[Identity]", Storage = "m_Identity", DbType = "NVarChar(36) NOT NULL", CanBeNull = false, IsPrimaryKey = true)]
        public string Identity
        {
            get
            {
                return m_Identity;
            }
            set
            {
                m_Identity = value;
            }
        }
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public bool IsAutoTrade
        {
            get
            {
                return m_AETrade;
            }
            set
            {
                m_AETrade = value;
            }
        }
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public double LastPx
        {
            get
            {
                return m_LastPx;
            }
            set
            {
                m_LastPx = value;
            }
        }
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public double LastQty
        {
            get
            {
                return m_LastQty;
            }
            set
            {
                m_LastQty = value;
            }
        }
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public double LeavesQty
        {
            get
            {
                return m_LeavesQty;
            }
            set
            {
                m_LeavesQty = value;
            }
        }
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(255)")]
        public string LocateReqd
        {
            get
            {
                return m_LocateReqd;
            }
            set
            {
                m_LocateReqd = value;
            }
        }
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(255)")]
        public string LocationID
        {
            get
            {
                return m_LocationID;
            }
            set
            {
                m_LocationID = value;
            }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public long MaxFloor
        {
            get
            {
                return (long)m_MaxFloor;
            }
            set
            {
                m_MaxFloor = value;
            }
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
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(36)")]
        public string ProductIdentity
        {
            get { return m_ProductIdentity; }
            set { m_ProductIdentity = value; }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(256)")]
        public string OCAGroupName
        {
            get
            {
                return m_OCAGroupName;
            }
            set
            {
                m_OCAGroupName = value;
            }
        }
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(32)")]
        public string OrdStatus
        {
            get
            {
                return m_OrdStatus;
            }
            set
            {
                m_OrdStatus = value;
            }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public KaiTrade.Interfaces.LastOrderCommand LastOrderCommand
        {
            get
            {
                return _lastOrderCommand;
            }
            set
            {
                _lastOrderCommand = value;
            }
        }


        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(32)")]
        public string OrdType
        {
            get
            {
                return m_OrdType;
            }
            set
            {
                m_OrdType = value;
            }
        }
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(256)")]
        public string OrderID
        {
            get
            {
                return m_OrderID;
            }
            set
            {
                m_OrderID = value;
            }
        }
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public long OrderQty
        {
            get
            {
                return (long)m_OrderQty;
            }
            set
            {
                m_OrderQty = value;
            }
        }
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(256)")]
        public string OrigClOrdID
        {
            get
            {
                return m_OrigClOrdID;
            }
            set
            {
                m_OrigClOrdID = value;
            }
        }
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(36)")]
        public string ParentIdentity
        {
            get
            {
                return m_ParentIdentity;
            }
            set
            {
                m_ParentIdentity = value;
            }
        }
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public double Price
        {
            get
            {
                return m_Price;
            }
            set
            {
                m_Price = value;
            }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public double QuantityLimit
        {
            get
            {
                return m_QuantityLimit;
            }
            set
            {
                m_QuantityLimit = value;
            }
        }
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(256)")]
        public string ShortSaleLocate
        {
            get
            {
                return m_ShortSaleLocate;
            }
            set
            {
                m_ShortSaleLocate = value;
            }
        }
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(32)")]
        public string Side
        {
            get
            {
                return m_Side;
            }
            set
            {
                m_Side = value;
            }
        }
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public double StopPx
        {
            get
            {
                return m_StopPx;
            }
            set
            {
                m_StopPx = value;
            }
        }
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(256)")]
        public string StrategyName
        {
            get
            {
                return m_StrategyName;
            }
            set
            {
                m_StrategyName = value;
            }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(256)")]
        public string AlgoName
        {
            get
            {
                return m_AlgoName;
            }
            set
            {
                m_AlgoName = value;
            }
        }

        /// <summary>
        /// Get/Set the list of strategy parameters for the order - based on FIX Protocal ADTL
        /// </summary>
        [DataMember]
        [JsonProperty]
        public List<KaiTrade.Interfaces.IParameter> K2Parameters
        {
            get
            {
                return m_K2Parameters;
            }
            set
            {
                m_K2Parameters = value;
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
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(512)")]
        public string Text
        {
            get
            {
                return m_Text;
            }
            set
            {
                m_Text = value;
            }
        }
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(26)")]
        public string TimeInForce
        {
            get
            {
                return m_TimeInForce;
            }
            set
            {
                m_TimeInForce = value;
            }
        }
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(256)")]
        public string TradeVenue
        {
            get
            {
                return m_VenueCode;
            }
            set
            {
                m_VenueCode = value;
            }
        }
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(26)")]
        public string TransactTime
        {
            get
            {
                return m_TransactTime;
            }
            set
            {
                m_TransactTime = value;
            }
        }
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(36)")]
        public string TriggerOrderID
        {
            get
            {
                return m_TriggerOrderID;
            }
            set
            {
                m_TriggerOrderID = value;
            }
        }

        /// <summary>
        /// Date time processed/stored in DB
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public DateTime SystemDate
        {
            get
            {
                return m_SystemDate;
            }
            set
            {
                m_SystemDate = value;
            }
        }
    }
}
