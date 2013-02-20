//-----------------------------------------------------------------------
// <copyright file="Position.cs" company="KaiTrade LLC">
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
using System.Text;

using System.ServiceModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace K2DataObjects
{
    [DataContract]
    [JsonObject(MemberSerialization.OptIn)]
    public class PositionSummary : KaiTrade.Interfaces.IPositionSummary
    {
        public PositionSummary()
        {

        }

        public PositionSummary(string mnemomic, string accountCode, string correlationID)
        {
            this.Mnemonic = mnemomic;
            this.AccountCode = accountCode;
            this.CorrelationID = correlationID;
            this.ServerTime = DateTime.Now;
        }

        /// <summary>
        /// Product Mnemonic
        /// </summary>
        private string m_Mnemonic;

        [DataMember]
        [JsonProperty]
        public string Mnemonic
        {
            get { return m_Mnemonic; }
            set { m_Mnemonic = value; }
        }

        private string m_AccountCode;

        [DataMember]
        [JsonProperty]
        public string AccountCode
        {
            get { return m_AccountCode; }
            set { m_AccountCode = value; }
        }

        private string m_CorrelationID;

        [DataMember]
        [JsonProperty]
        public string CorrelationID
        {
            get { return m_CorrelationID; }
            set { m_CorrelationID = value; }
        }

        private long m_ProductPosition;

        [DataMember]
        [JsonProperty]
        public long ProductPosition
        {
            get { return m_ProductPosition; }
            set { m_ProductPosition = value; }
        }


        private long? m_AccountPositon;

        [DataMember]
        [JsonProperty]
        public long? AccountPositon
        {
            get { return m_AccountPositon; }
            set { m_AccountPositon = value; }
        }


        private long? m_CorrelationIDPositon;

        [DataMember]
        [JsonProperty]
        public long? CorrelationIDPositon
        {
            get { return m_CorrelationIDPositon; }
            set { m_CorrelationIDPositon = value; }
        }


        /// <summary>
        /// Time of last update
        /// </summary>
        private DateTime m_ServerTime;

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public DateTime ServerTime
        {
            get { return m_ServerTime; }
            set { m_ServerTime = value; }
        }


        public override string ToString()
        {
            string myRet = "";
            try
            {
                myRet += string.Format("Account {0}, Mnemonic {1}, ProductPosn {2} AccountPosn {3} CorrIdPosn {4}", this.AccountCode, this.Mnemonic, this.ProductPosition, this.AccountPositon, this.CorrelationIDPositon);


            }
            catch
            {
            }

            return myRet;
        }

    }


    [DataContract]
    [JsonObject(MemberSerialization.OptIn)]
    [global::System.Data.Linq.Mapping.TableAttribute(Name = "dbo.Account")]
    public class Position : KaiTrade.Interfaces.IPosition
    {

        private string m_Identity;

        [DataMember]
        [JsonProperty]
        [global::System.Data.Linq.Mapping.ColumnAttribute(Name = "[Identity]", Storage = "m_ID", DbType = "NVarChar(36) NOT NULL", CanBeNull = false, IsPrimaryKey = true)]
        public string Identity
        {
            get { return m_Identity; }
            set { m_Identity = value; }
        } 

        /// <summary>
        /// ID of parent position
        /// </summary>
        private string m_Parent;

        [DataMember]
        [JsonProperty]
        [global::System.Data.Linq.Mapping.ColumnAttribute(Name = "[Parent]", Storage = "m_Parent", DbType = "NVarChar(36) NOT NULL", CanBeNull = true, IsPrimaryKey = false)]
        public string Parent
        {
            get { return m_Parent; }
            set { m_Parent = value; }
        }


        /// <summary>
        /// Account associated with the position
        /// </summary>
        private string m_CorrelationID;

        [DataMember]
        [global::System.Data.Linq.Mapping.ColumnAttribute(Name = "[CorrelationID]", Storage = "m_CorrelationID", DbType = "NVarChar(256) NOT NULL", CanBeNull = true)]
        [JsonProperty]
        public string CorrelationID
        {
            get { return m_CorrelationID; }
            set { m_CorrelationID = value; }
        }

        private string m_AccountCode;

        [DataMember]
        [global::System.Data.Linq.Mapping.ColumnAttribute(Name = "[AccountCode]", Storage = "m_AccountCode", DbType = "NVarChar(256) NOT NULL", CanBeNull = false)]
        [JsonProperty]
        public string AccountCode
        {
            get { return m_AccountCode; }
            set { m_AccountCode = value; }
        }

        /// <summary>
        /// Average price
        /// </summary>
        private decimal? m_AvgPrice;

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public decimal? AvgPrice
        {
            get { return m_AvgPrice; }
            set { m_AvgPrice = value; }
        }

        /// <summary>
        /// Product Mnemonic
        /// </summary>
        private string m_Mnemonic;

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(255)")]
        public string Mnemonic
        {
            get { return m_Mnemonic; }
            set { m_Mnemonic = value; }
        }

        /// <summary>
        /// Profit and Loss
        /// </summary>
        private decimal? m_PnL;

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public decimal? PnL
        {
            get { return m_PnL; }
            set { m_PnL = value; }
        }

        /// <summary>
        /// Quantity
        /// </summary>
        private long m_Quantity;

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public long Quantity
        {
            get { return m_Quantity; }
            set { m_Quantity = value; }
        }

        /// <summary>
        /// Side
        /// </summary>
        private string m_Side;

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(32)")]
        public string Side
        {
            get { return m_Side; }
            set { m_Side = value; }
        }

        /// <summary>
        /// Time of last update
        /// </summary>
        private DateTime m_UpdateTime;

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public DateTime UpdateTime
        {
            get { return m_UpdateTime; }
            set { m_UpdateTime = value; }
        }

        /// <summary>
        /// Open trade equity
        /// </summary>
        private decimal? m_OTE;

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public decimal? OTE
        {
            get { return m_OTE; }
            set { m_OTE = value; }
        }

        /// <summary>
        /// Market value of options
        /// </summary>
        private decimal? m_MVO;

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public decimal? MVO
        {
            get { return m_MVO; }
            set { m_MVO = value; }
        }



        public override string ToString()
        {
            string myRet = "";
            try
            {
                myRet += string.Format("Account {0}, Mnemonic {1}, Side {2} AvgPx {3}, Qty {4}", this.AccountCode, this.Mnemonic, this.Side, this.AvgPrice, this.Quantity);
                myRet += string.Format("PnL {0}, Time {1}, OTE  {2}, MVO {3}", this.PnL, this.UpdateTime, this.OTE, this.MVO);
                
            }
            catch
            {
            }

            return myRet;
        }
       
    }
}
