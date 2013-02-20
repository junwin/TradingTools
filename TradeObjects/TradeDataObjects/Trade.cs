//-----------------------------------------------------------------------
// <copyright file="Trade.cs" company="KaiTrade LLC">
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
    /// <summary>
    /// This class models a junction between orders and trades
    /// </summary>
    [DataContract]
    [JsonObject(MemberSerialization.OptIn)]
    [global::System.Data.Linq.Mapping.TableAttribute(Name = "dbo.OrderTrade")]
    public class OrderTrade
    {
        private string _OrderIdentity;
        private string _TradeIdentity;
        private int _Identity;

        public OrderTrade()
        {
        }

        public OrderTrade(string orderIdentity, string tradeIdentity)
        {
            _OrderIdentity = orderIdentity;
            _TradeIdentity = tradeIdentity;
        }

        [global::System.Data.Linq.Mapping.Column(Name = "ID", Storage = "_Identity", DbType = "int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
        public int Identity
        {   get {return _Identity;}
            set { _Identity = value;}
        }

        [global::System.Data.Linq.Mapping.ColumnAttribute(Name = "[OrderID]", Storage = "_OrderIdentity", DbType = "NVarChar(36) NOT NULL", CanBeNull = false)]
        public string OrderIdentity
        {
            get { return _OrderIdentity; }
            set { _OrderIdentity = value; }
        }

        [global::System.Data.Linq.Mapping.ColumnAttribute(Name = "[TradeID]", Storage = "_TradeIdentity", DbType = "NVarChar(36) NOT NULL", CanBeNull = false)]
        public string TradeIdentity
        {
            get { return _TradeIdentity; }
            set { _TradeIdentity = value; }
        }

    }

    [DataContract]
    [JsonObject(MemberSerialization.OptIn)]
    [global::System.Data.Linq.Mapping.TableAttribute(Name = "dbo.Trade")]
    public partial class Trade : KaiTrade.Interfaces.ITrade
    {
        private string _Identity;

        private string _VenueCode;

        private string _ProductID;

       

        private string _Mnemonic;

        private string _Account;

        private string _ClOrdID;
        private string _ClOrdID2;

        private string _OrigClOrdID;

        private string _OrderID;

        private string _Side;

        private string _OrdType;

        private decimal? _Quantity;

        private decimal? _Price;

        private decimal? _StopPx;

        private string _TradeStatus;

        private decimal? _LeavesQty;

        private decimal? _CumQty;

        private decimal? _LastPx;

        private decimal? _LastQty;

        private decimal? _AvgPx;

        private string _Text;

        private string _TransactTime;
        private string _TradeDate;

        private string _Description;

        private string _Tag;

        private string _CorrelationID;

        private string _ExecutionID;

        private string _TimeInForce;

        /// <summary>
        /// Determines if a trade opens or closes a position
        /// </summary>
        private string m_PositionEffect = "";

        private string m_TradeID;
        private string m_MatchID;
        private string m_MessageEventSource;
        private string m_TradingSessionID;
        private string m_TradeInputSource;
        private string m_HandlInst;
        private string m_VenueType;
        private string m_ExecutingBrokerCode;
        private string m_ClearingBrokerCode;
        private string m_Trader;
        private DateTime m_ClearingDate;
        private DateTime m_BusinessDate;
        private string m_SessionID;
        private string m_SessionSubID;

        private DateTime m_SystemDate;

        public Trade()
        {
            _Identity = System.Guid.NewGuid().ToString();
        }

        [global::System.Data.Linq.Mapping.ColumnAttribute(Name = "[Identity]", Storage = "_Identity", DbType = "NVarChar(36)")]
        public string Identity
        {
            get
            {
                return this._Identity;
            }
            set
            {
                if ((this._Identity != value))
                {
                    this._Identity = value;
                }
            }
        }

        [DataMember]
        [JsonProperty]
        [global::System.Data.Linq.Mapping.ColumnAttribute(Name = "[TradeID]", Storage = "m_TradeID", DbType = "NVarChar(256) NOT NULL", CanBeNull = false, IsPrimaryKey = true)]
        public string TradeID
        { get { return m_TradeID; } set { m_TradeID = value; } }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(256)")]
        public string MatchID
        { get { return m_MatchID; } set { m_MatchID = value; } }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(256)")]
        public string MessageEventSource
        { get { return m_MessageEventSource; } set { m_MessageEventSource = value; } }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(256)")]
        public string TradingSessionID
        { get { return m_TradingSessionID; } set { m_TradingSessionID = value; } }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(256)")]
        public string TradeInputSource
        { get { return m_TradeInputSource; } set { m_TradeInputSource = value; } }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(256)")]
        public string HandlInst
        { get { return m_HandlInst; } set { m_HandlInst = value; } }

        [DataMember]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(256)")]
        public string VenueType
        { get { return m_VenueType; } set { m_VenueType = value; } }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(255)")]
        public string ExecutingBrokerCode
        { get { return m_ExecutingBrokerCode; } set { m_ExecutingBrokerCode = value; } }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(256)")]
        public string ClearingBrokerCode
        { get { return m_ClearingBrokerCode; } set { m_ClearingBrokerCode = value; } }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(256)")]
        public string Trader
        { get { return m_Trader; } set { m_Trader = value; } }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(26)")]
        public DateTime ClearingDate
        { get { return m_ClearingDate; } set { m_ClearingDate = value; } }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(26)")]
        public DateTime BusinessDate
        { get { return m_BusinessDate; } set { m_BusinessDate = value; } }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(256)")]
        public string SessionID
        { get { return m_SessionID; } set { m_SessionID = value; } }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(256)")]
        public string SessionSubID
        { get { return m_SessionSubID; } set { m_SessionSubID = value; } }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(256)")]
        public string VenueCode
        {
            get
            {
                return this._VenueCode;
            }
            set
            {
                if ((this._VenueCode != value))
                {
                    this._VenueCode = value;
                }
            }
        }

        [DataMember]
        [JsonProperty]
        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_ProductID", DbType = "NVarChar(36)")]
        public string ProductID
        {
            get
            {
                return this._ProductID;
            }
            set
            {
                if ((this._ProductID != value))
                {
                    this._ProductID = value;
                }
            }
        }

       

        [DataMember]
        [JsonProperty]
        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_Mnemonic", DbType = "NVarChar(256)")]
        public string Mnemonic
        {
            get
            {
                return this._Mnemonic;
            }
            set
            {
                if ((this._Mnemonic != value))
                {
                    this._Mnemonic = value;
                }
            }
        }

        [DataMember]
        [JsonProperty]
        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_Account", DbType = "NVarChar(256)")]
        public string Account
        {
            get
            {
                return this._Account;
            }
            set
            {
                if ((this._Account != value))
                {
                    this._Account = value;
                }
            }
        }

        [DataMember]
        [JsonProperty]
        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_ClOrdID", DbType = "NVarChar(256)")]
        public string ClOrdID
        {
            get
            {
                return this._ClOrdID;
            }
            set
            {
                if ((this._ClOrdID != value))
                {
                    this._ClOrdID = value;
                }
            }
        }

        [DataMember]
        [JsonProperty]
        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_ClOrdID2", DbType = "NVarChar(256)")]
        public string ClOrdID2
        {
            get
            {
                return this._ClOrdID2;
            }
            set
            {
                if ((this._ClOrdID2 != value))
                {
                    this._ClOrdID2 = value;
                }
            }
        }


        [DataMember]
        [JsonProperty]
        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_OrigClOrdID", DbType = "NVarChar(256)")]
        public string OrigClOrdID
        {
            get
            {
                return this._OrigClOrdID;
            }
            set
            {
                if ((this._OrigClOrdID != value))
                {
                    this._OrigClOrdID = value;
                }
            }
        }

        [DataMember]
        [JsonProperty]
        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_OrderID", DbType = "NVarChar(256)")]
        public string OrderID
        {
            get
            {
                return this._OrderID;
            }
            set
            {
                if ((this._OrderID != value))
                {
                    this._OrderID = value;
                }
            }
        }

        [DataMember]
        [JsonProperty]
        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_Side", DbType = "NVarChar(32)")]
        public string Side
        {
            get
            {
                return this._Side;
            }
            set
            {
                if ((this._Side != value))
                {
                    this._Side = value;
                }
            }
        }

        [DataMember]
        [JsonProperty]
        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_OrdType", DbType = "NVarChar(32)")]
        public string OrdType
        {
            get
            {
                return this._OrdType;
            }
            set
            {
                if ((this._OrdType != value))
                {
                    this._OrdType = value;
                }
            }
        }

        [DataMember]
        [JsonProperty]
        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_Quantity", DbType = "Float")]
        public decimal? Quantity
        {
            get
            {
                return this._Quantity;
            }
            set
            {
                if ((this._Quantity != value))
                {
                    this._Quantity = value;
                }
            }
        }

        [DataMember]
        [JsonProperty]
        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_Price", DbType = "Float")]
        public decimal? Price
        {
            get
            {
                return this._Price;
            }
            set
            {
                if ((this._Price != value))
                {
                    this._Price = value;
                }
            }
        }

        [DataMember]
        [JsonProperty]
        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_StopPx", DbType = "Float")]
        public decimal? StopPx
        {
            get
            {
                return this._StopPx;
            }
            set
            {
                if ((this._StopPx != value))
                {
                    this._StopPx = value;
                }
            }
        }

        [DataMember]
        [JsonProperty]
        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_TradeStatus", DbType = "NVarChar(32)")]
        public string TradeStatus
        {
            get
            {
                return this._TradeStatus;
            }
            set
            {
                if ((this._TradeStatus != value))
                {
                    this._TradeStatus = value;
                }
            }
        }

        [DataMember]
        [JsonProperty]
        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_LeavesQty", DbType = "Float")]
        public decimal? LeavesQty
        {
            get
            {
                return this._LeavesQty;
            }
            set
            {
                if ((this._LeavesQty != value))
                {
                    this._LeavesQty = value;
                }
            }
        }

        [DataMember]
        [JsonProperty]
        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_CumQty", DbType = "Float")]
        public decimal? CumQty
        {
            get
            {
                return this._CumQty;
            }
            set
            {
                if ((this._CumQty != value))
                {
                    this._CumQty = value;
                }
            }
        }

        [DataMember]
        [JsonProperty]
        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_LastPx", DbType = "Float")]
        public decimal? LastPx
        {
            get
            {
                return this._LastPx;
            }
            set
            {
                if ((this._LastPx != value))
                {
                    this._LastPx = value;
                }
            }
        }

        [DataMember]
        [JsonProperty]
        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_LastQty", DbType = "Float")]
        public decimal? LastQty
        {
            get
            {
                return this._LastQty;
            }
            set
            {
                if ((this._LastQty != value))
                {
                    this._LastQty = value;
                }
            }
        }

        [DataMember]
        [JsonProperty]
        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_AvgPx", DbType = "Float")]
        public decimal? AvgPx
        {
            get
            {
                return this._AvgPx;
            }
            set
            {
                if ((this._AvgPx != value))
                {
                    this._AvgPx = value;
                }
            }
        }

        [DataMember]
        [JsonProperty]
        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_Text", DbType = "NVarChar(512)")]
        public string Text
        {
            get
            {
                return this._Text;
            }
            set
            {
                if ((this._Text != value))
                {
                    this._Text = value;
                }
            }
        }

        [DataMember]
        [JsonProperty]
        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_TransactTime", DbType = "NVarChar(26)")]
        public string TransactTime
        {
            get
            {
                return this._TransactTime;
            }
            set
            {
                if ((this._TransactTime != value))
                {
                    this._TransactTime = value;
                }
            }
        }

        [DataMember]
        [JsonProperty]
        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_TradeDate", DbType = "NVarChar(26)")]
        public string TradeDate
        {
            get
            {
                return this._TradeDate;
            }
            set
            {
                if ((this._TradeDate != value))
                {
                    this._TradeDate = value;
                }
            }
        }

        [DataMember]
        [JsonProperty]
        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_Description", DbType = "NVarChar(256)")]
        public string Description
        {
            get
            {
                return this._Description;
            }
            set
            {
                if ((this._Description != value))
                {
                    this._Description = value;
                }
            }
        }

        [DataMember]
        [JsonProperty]
        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_Tag", DbType = "NVarChar(512)")]
        public string Tag
        {
            get
            {
                return this._Tag;
            }
            set
            {
                if ((this._Tag != value))
                {
                    this._Tag = value;
                }
            }
        }

        [DataMember]
        [JsonProperty]
        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_CorrelationID", DbType = "NVarChar(256)")]
        public string CorrelationID
        {
            get
            {
                return this._CorrelationID;
            }
            set
            {
                if ((this._CorrelationID != value))
                {
                    this._CorrelationID = value;
                }
            }
        }

        [DataMember]
        [JsonProperty]
        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_ExecutionID", DbType = "NVarChar(256)")]
        public string ExecutionID
        {
            get
            {
                return this._ExecutionID;
            }
            set
            {
                if ((this._ExecutionID != value))
                {
                    this._ExecutionID = value;
                }
            }
        }

        [DataMember]
        [JsonProperty]
        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_TimeInForce", DbType = "NVarChar(26)")]
        public string TimeInForce
        {
            get
            {
                return this._TimeInForce;
            }
            set
            {
                if ((this._TimeInForce != value))
                {
                    this._TimeInForce = value;
                }
            }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(1)")]
        public string PositionEffect
        {
            get
            {
                return m_PositionEffect;
            }
            set
            {
                m_PositionEffect = value;
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
