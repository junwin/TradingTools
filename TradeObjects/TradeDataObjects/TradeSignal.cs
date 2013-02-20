//-----------------------------------------------------------------------
// <copyright file="TradeSignal.cs" company="KaiTrade LLC">
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
    [global::System.Data.Linq.Mapping.TableAttribute(Name = "dbo.TradeSignal")]
    public class TradeSignal : KaiTrade.Interfaces.ITradeSignal
    {
        /// <summary>
        /// Type of signal
        /// </summary>
        private KaiTrade.Interfaces.TradeSignalType m_SignalType;
        private string m_Identity;
        private string m_StrategyID = "";
        private string m_Name;
        private string m_Origin="";
        private string m_OrdType;
        private string m_Mnemonic = "";

        private KaiTrade.Interfaces.TradeSignalStatus m_Status = KaiTrade.Interfaces.TradeSignalStatus.undefined;

        /// <summary>
        /// Time we created signal
        /// </summary>
        private DateTime m_TimeCreated;

        /// <summary>
        /// Time that the signal is valid in milli seconds
        /// </summary>
        private long m_TimeValid = long.MaxValue;

        /// <summary>
        /// Is the signal set
        /// </summary>
        private bool m_Set;

        /// <summary>
        /// Side of order associated with this signal
        /// </summary>
        private string m_Side;

        /// <summary>
        /// Order quantity associated with the signal
        /// </summary>
        private double m_OrdQty=0;

        /// <summary>
        /// price associated with the signal
        /// </summary>
        private double m_Price=0;

        /// <summary>
        /// stop price associated with the signal
        /// </summary>
        private double m_StopPrice=0;

        /// <summary>
        /// profit price associated with the signal
        /// </summary>
        private double m_ProfitPrice = 0;

        private string m_Text="";

        /// <summary>
        /// User id ( if specified) - note this is the identity not the user's signonID
        /// </summary>
        private string user = "";

       
        

        public TradeSignal()
        {
            m_Identity = System.Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Type of signal
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public KaiTrade.Interfaces.TradeSignalType SignalType
        {
            get { return m_SignalType; }
            set { m_SignalType = value; }
        }

        /// <summary>
        /// unique identifier
        /// </summary>
        [DataMember]
        [JsonProperty]
        [global::System.Data.Linq.Mapping.ColumnAttribute(Name = "[Identity]", Storage = "m_Identity", DbType = "NVarChar(36) NOT NULL", CanBeNull = false, IsPrimaryKey = true)]
        public string Identity
        {
            get { return m_Identity; }
            set { m_Identity = value; }
        }

        [DataMember]
        [JsonProperty]
        [global::System.Data.Linq.Mapping.ColumnAttribute(Name = "[User]", Storage = "user", DbType = "NVarChar(36)")]
        public string User
        {
            get { return user; }
            set { user = value; }
        }

        /// <summary>
        /// Name of signal
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(255)")]
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public KaiTrade.Interfaces.TradeSignalStatus Status
        {
            get { return m_Status; }
            set { m_Status = value; }
        }

        /// <summary>
        /// Origin of signal - for example a CQG system trade in a trading system
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(255)")]
        public string Origin
        {
            get { return m_Origin; }
            set { m_Origin = value; }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public DateTime TimeCreated
        {
            get { return m_TimeCreated; }
            set { m_TimeCreated = value; }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public long TimeValid
        {
            get { return m_TimeValid; }
            set { m_TimeValid = value; }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(255)")]
        public string StrategyID
        {
            get { return m_StrategyID; }
            set { m_StrategyID = value; }
        }

        /// <summary>
        /// Is the signal set
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public bool Set
        {
            get { return m_Set; }
            set { m_Set = value; }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(255)")]
        public string Mnemonic
        {
            get { return m_Mnemonic; }
            set { m_Mnemonic = value; }
        }

        /// <summary>
        /// Type of order associated with this signal
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(255)")]
        public string OrdType
        {
            get { return m_OrdType; }
            set { m_OrdType = value; }
        }

        /// <summary>
        /// Side of order associated with this signal
        /// </summary>
        [DataMember]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(255)")]
        public string Side
        {
            get { return m_Side; }
            set { m_Side = value; }
        }

        /// <summary>
        /// Order quantity associated with the signal
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public double OrdQty
        {
            get { return m_OrdQty; }
            set { m_OrdQty = value; }
        }

        /// <summary>
        /// price associated with the signal
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public double Price
        {
            get { return m_Price; }
            set { m_Price = value; }
        }

        /// <summary>
        /// stop price associated with the signal
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public double StopPrice
        {
            get { return m_StopPrice; }
            set { m_StopPrice = value; }
        }

        /// <summary>
        /// profit price associated with the signal
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public double ProfitPrice
        {
            get { return m_ProfitPrice; }
            set { m_ProfitPrice = value; }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(512)")]
        public string Text
        {
            get { return m_Text; }
            set { m_Text = value; }
        }

        public override string ToString()
        {
            string myInfo = "";
            try
            {
                myInfo += string.Format("Origin ={0} Name= {1} Type= {2} Set= {3} ", this.Origin, this.Name, this.SignalType.ToString(), this.Set);
                myInfo += string.Format("Side= {0} OrdType= {1} Qty= {2} ", this.Side, this.OrdType, this.OrdQty);
                myInfo += string.Format("Price= {0} StopPrice= {1} ProfitPrice= {2}", this.Price, this.StopPrice, this.ProfitPrice);
                myInfo += string.Format("Text= {0} ", this.Text);
                myInfo += string.Format("DateCreated= {0} ", this.TimeCreated);
            }
            catch
            {
            }
            return myInfo;
        }
    }
}
