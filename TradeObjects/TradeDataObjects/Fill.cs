//-----------------------------------------------------------------------
// <copyright file="Fill.cs" company="KaiTrade LLC">
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
    /// prodives a data store for a Fill
    /// </summary>
    [DataContract]
    [global::System.Data.Linq.Mapping.TableAttribute(Name = "dbo.Fill")]
    [JsonObject(MemberSerialization.OptIn)]
    public class Fill : KaiTrade.Interfaces.IFill 
    {
        private string m_ExecID = "";
        private string m_ExecReport;
        private double m_LastQty = 0;
        private double m_LeavesQty = 0;
        private double m_CumQty = 0;
        private double m_LastPx = 0;
        private double m_AvgPx = 0;
        private string m_OrderStatus = "";


        private string m_ClOrdID;
        private string m_OrigClOrdID;
        private string m_ExecType;
        private string m_ExecRefID;

        /// <summary>
        /// Account that the order traded under
        /// </summary>
        private string m_Account;

        /// <summary>
        /// Mnemonic - better to use the product ID where possible
        /// </summary>
        private string m_Mnemonic;

        /// <summary>
        /// Unique product ID
        /// </summary>
        private string m_ProductID;


        /// <summary>
        /// system time stamp - time last processed or data service action
        /// </summary>
        private DateTime m_SystemTime;


        private long m_Sequence;


        /// <summary>
        /// Time in ticks that the fill was processed
        /// </summary>
        private long m_Ticks;

        /// <summary>
        /// Kai/Exchange/Broker order that the fill relates to
        /// </summary>
        private string m_OrderID = "";

        /// <summary>
        /// Kaitrade order id (guid) that the fill belongs to - this can
        /// be empty if the order is not matched
        /// </summary>
        private string m_OrderIdentity = "";

        /// <summary>
        /// Free format text asscociated with the fill
        /// </summary>
        private string _text = "";

        


        /// <summary>
        /// Reject reason if the order is cancelled
        /// </summary>
        private string _OrdRejReason = "";

       

        public log4net.ILog m_Log = log4net.LogManager.GetLogger("Kaitrade");

        #region Fill Members

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(1)")]
        public string OrderStatus
        {
            get { return m_OrderStatus; }
            set { m_OrderStatus = value; }
        }
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public double AvgPx
        {
            get { return m_AvgPx; }
            set { m_AvgPx = value; }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public double LeavesQty
        {
            get { return m_LeavesQty; }
            set { m_LeavesQty = value; }
        }
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public double CumQty
        {
            get { return m_CumQty; }
            set { m_CumQty = value; }
        }

        /// <summary>
        /// get/set time processed by KaiTrade main message handler in ticks
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public long Ticks
        {
            get { return m_Ticks; }
            set { m_Ticks = value; }
        }

        [DataMember]
        [JsonProperty]
        [global::System.Data.Linq.Mapping.Column(Name = "Sequence", Storage = "m_Sequence", DbType = "BIGINT NOT NULL IDENTITY", IsPrimaryKey = false, IsDbGenerated = true)]
        public long Sequence
        {
            get { return m_Sequence; }
            set { m_Sequence = value; }
        }

        /// <summary>
        /// ClOrdID of the order at the time of the fill
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(256)")]
        public string ClOrdID
        {   get
            {
                return m_ClOrdID;
            }
            set
            {
                m_ClOrdID = value;
            } 
        }

        /// <summary>
        /// original ClOrdID of the order at the time of the fill
        /// </summary>
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

        /// <summary>
        /// Describes purpose of the Exec report
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(32)")]
        public string ExecType
        {
            get
            {
                return m_ExecType;
            }
            set
            {
                m_ExecType = value;
            }
        }

        /// <summary>
        /// If trade cancel or tarde correct refers to the previous fill/execution report
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(256)")]
        public string ExecRefID
        {
            get
            {
                return m_ExecRefID;
            }
            set
            {
                m_ExecRefID = value;
            }
        }

        /// <summary>
        /// get/set the quickfix exec report - resets the ID
        /// </summary>
        ///
        [DataMember]
        [System.Data.Linq.Mapping.Column]
        [JsonProperty]
        public string ExecReport
        {
            get
            {
                return m_ExecReport;
            }
            set
            {
                m_ExecReport = value;
                
            }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public double FillQty
        {
            get { return m_LastQty; }
            set { m_LastQty = value; }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public double LastPx
        {
            get { return m_LastPx; }
            set { m_LastPx = value; }
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
        [global::System.Data.Linq.Mapping.ColumnAttribute(Name = "[OrderIdentity]", Storage = "m_OrderIdentity", DbType = "NVarChar(36) NOT NULL", CanBeNull = false, IsPrimaryKey = false)]
        public string OrderIdentity
        {
            get { return m_OrderIdentity; }
            set { m_OrderIdentity = value; }
        }

        [DataMember]
        [JsonProperty]
        [global::System.Data.Linq.Mapping.ColumnAttribute(Name = "[Identity]", Storage = "m_ExecID", DbType = "NVarChar(36) NOT NULL", CanBeNull = false, IsPrimaryKey = true)]
        public string Identity
        {
            get
            {
                return m_ExecID;
            }
            set { m_ExecID = value; }
        }


        /// <summary>
        /// Account that the order traded under
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(256)")]
        public string Account
        { get { return m_Account; } set { m_Account = value; } }

        /// <summary>
        /// Mnemonic - better to use the product ID where possible
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(256)")]
        public string Mnemonic
        { get { return m_Mnemonic; } set { m_Mnemonic = value; } }


        /// <summary>
        /// Unique product ID
        /// </summary>

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(36)")]
        public string ProductID
        { get { return m_ProductID; } set { m_ProductID = value; } }


        /// <summary>
        /// system time stamp - time last processed or data service action
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public DateTime SystemTime
        { get { return m_SystemTime; } set { m_SystemTime = value; } }


        [DataMember]
        [JsonProperty]
        [global::System.Data.Linq.Mapping.ColumnAttribute(Name = "[OrdRejReason]", Storage = "_OrdRejReason", DbType = "NVarChar(64)", CanBeNull = true, IsPrimaryKey = false)]
        public string OrdRejReason
        {
            get { return _OrdRejReason; }
            set { _OrdRejReason = value; }
        }

        [DataMember]
        [JsonProperty]
        [global::System.Data.Linq.Mapping.ColumnAttribute(Name = "[Text]", Storage = "_text", DbType = "NVarChar(256)", CanBeNull = true, IsPrimaryKey = false)]
        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        /*
        public  void SetUpFromFixExecReport(string r)
        {
            try
            {
                m_ExecReport = r;
                QuickFix.ExecID _ExecID = new QuickFix.ExecID();
                QuickFix.LastQty _LastQty = new QuickFix.LastQty();
                QuickFix.LeavesQty _LeavesQty = new QuickFix.LeavesQty();
                QuickFix.CumQty _CumQty = new QuickFix.CumQty();
                QuickFix.LastPx _LastPx = new QuickFix.LastPx();
                QuickFix.AvgPx _AvgPx = new QuickFix.AvgPx();
                QuickFix.ClOrdID _clOrdID = new QuickFix.ClOrdID();
                QuickFix.OrigClOrdID _OrigClOrdID = new QuickFix.OrigClOrdID();
                QuickFix.ExecType _execType = new QuickFix.ExecType();
                QuickFix.ExecRefID _execRefID = new QuickFix.ExecRefID();
                QuickFix.OrdStatus ordStatus = new QuickFix.OrdStatus();
                QuickFix.OrderID orderID = new QuickFix.OrderID();
                QuickFix.Symbol symbol = new QuickFix.Symbol();
                QuickFix.Account account = new QuickFix.Account();

                QuickFix.Message execReport = new QuickFix.Message(r);

                execReport.getField(_clOrdID);
                this.ClOrdID = _clOrdID.getValue();

                if (execReport.isSetField(_OrigClOrdID))
                {
                    execReport.getField(_OrigClOrdID);
                    this.OrigClOrdID = _OrigClOrdID.getValue();
                }
                else
                {
                    this.OrigClOrdID = "";
                }

                if (execReport.isSetField(_execRefID))
                {
                    execReport.getField(_execRefID);
                    this.ExecRefID = _execRefID.getValue();
                }
                else
                {
                    this.ExecRefID = "";
                }

                execReport.getField(_execType);
                this.ExecType = _execType.getValue().ToString();

                execReport.getField(_ExecID);
                this.Identity = _ExecID.getValue();

                if (execReport.isSetField(ordStatus))
                {
                    execReport.getField(ordStatus);
                    this.OrderStatus = ordStatus.ToString();
                }

                if (execReport.isSetField(symbol))
                {
                    execReport.getField(symbol);
                    this.Mnemonic = symbol.ToString();
                }
                else
                {
                    QuickFix.SecurityID securityID = new QuickFix.SecurityID();
                    if (execReport.isSetField(securityID))
                    {
                        execReport.getField(securityID);
                        this.Mnemonic = securityID.ToString();
                    }
                }
                if (execReport.isSetField(account))
                {
                    execReport.getField(account);
                    this.Account = account.ToString();
                }

                if (execReport.isSetField(orderID))
                {
                    execReport.getField(orderID);
                    this.OrderID = orderID.ToString();
                }

                execReport.getField(_AvgPx);
                this.AvgPx = _AvgPx.getValue();

                execReport.getField(_CumQty);
                this.CumQty = _CumQty.getValue();

                execReport.getField(_LeavesQty);
                this.LeavesQty = _LeavesQty.getValue();

                if (execReport.isSetField(_LastQty))
                {
                    execReport.getField(_LastQty);
                    this.FillQty = _LastQty.getValue();
                }
                else
                {
                    this.FillQty = 0;

                }

                if (execReport.isSetField(_LastPx))
                {
                    execReport.getField(_LastPx);
                    this.LastPx = _LastPx.getValue();
                }
                else
                {
                    this.LastPx = 0;
                }

                this.SystemTime = DateTime.UtcNow;
            }
            catch (Exception myE)
            {
            }
        }
         */

        #endregion
    }
}
