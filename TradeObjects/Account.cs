//-----------------------------------------------------------------------
// <copyright file="Account.cs" company="KaiTrade LLC">
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
using System.ServiceModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace K2DataObjects
{
    [DataContract]
    [JsonObject(MemberSerialization.OptIn)]
    [global::System.Data.Linq.Mapping.TableAttribute(Name = "dbo.Account")]
    public class Account : KaiTrade.Interfaces.IAccount
    {
        /// <summary>
        /// unique identifier for the order
        /// </summary>
        private string m_Identity;

        private string m_User = "";

        

        /// <summary>
        /// Account code for the account - format depends on the venue
        /// </summary>
        private string m_AccountCode = "";

        /// <summary>
        /// Long(descriptive) name for the account
        /// </summary>
        private string m_LongName = "";

        /// <summary>
        /// Venue code that this account applies to
        /// </summary>
        private string m_VenueCode = "";

        /// <summary>
        /// Firm/FCM code that the account belongs to
        /// </summary>
        private string m_FirmCode = "";

        /// <summary>
        /// Type of account
        /// </summary>
        private KaiTrade.Interfaces.AccountType m_AccountType;

        /// <summary>
        /// initial margine required for this account - depends on the
        /// number and type of products
        /// </summary>
        private decimal m_InitialMargin;

        /// <summary>
        /// maintenance margin required for this account - depends on the
        /// number and type of products
        /// </summary>
        private decimal m_MaintMargin;

        /// <summary>
        /// Net liquidity of the account
        /// </summary>
        private decimal m_NetLiquidity;

        /// <summary>
        /// NetLiq - initial margin
        /// </summary>
        private decimal m_AvailableFunds;

        /// <summary>
        /// NetLiq - manint margin
        /// </summary>
        private decimal m_ExcessFunds;

        public Account()
        {
            m_Identity = System.Guid.NewGuid().ToString();
        }

        #region Account Members

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(36)")]
        public string User
        {
            get { return m_User; }
            set { m_User = value; }
        }

        [DataMember]
        [global::System.Data.Linq.Mapping.ColumnAttribute(Name = "[Code]", Storage = "m_AccountCode", DbType = "NVarChar(256) NOT NULL", CanBeNull = false)]
        [JsonProperty]
        public string AccountCode
        {
            get
            {
                return m_AccountCode;
            }
            set
            {
                m_AccountCode = value;
            }
        }

        /// <summary>
        /// Get set the long name for this account
        /// </summary>
        [DataMember]
        [JsonProperty]
        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "m_LongName", DbType = "NVarChar(256)")]
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

        /// <summary>
        /// Get/Set the firm that the account belongs to
        /// </summary>
        [DataMember]
        [JsonProperty]
        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "m_FirmCode", DbType = "NVarChar(256)")]
        public string FirmCode
        {
            get { return m_FirmCode; }
            set { m_FirmCode = value; }
        }

        /// <summary>
        /// Set up account from an XML data binding
        /// </summary>
        /// <param name="myOrder"></param>
        public void FromXml(string xml)
        {
            //KAI.kaitns.Account myAccount = new KAI.kaitns.Account();
            //myAccount.FromXml(xml);
            //FromXMLDB(myAccount);
        }
        //void FromXMLDB(KAI.kaitns.Account myAccount);

        /// <summary>
        /// write account onto an XML data bining
        /// </summary>
        /// <returns></returns>
        public string AsXml()
        {
            //return ToXMLDB().ToXml();\
            return "";
        }
        //KAI.kaitns.Account ToXMLDB();

        

        [DataMember]
        [JsonProperty]
        [global::System.Data.Linq.Mapping.ColumnAttribute(Name = "[Identity]", Storage = "m_Identity", DbType = "NVarChar(36) NOT NULL", CanBeNull = false, IsPrimaryKey = true)]
        public string ID
        {
            get { return m_Identity; }
            set { m_Identity = value; }
        }

        

        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "m_VenueCode", DbType = "NVarChar(256) NOT NULL", CanBeNull = false)]
        [DataMember]
        [JsonProperty]
        public string VenueCode
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

        /// <summary>
        /// Type of account
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public KaiTrade.Interfaces.AccountType AccountType
        {
            get
            {
                return m_AccountType;
            }
            set
            {
                m_AccountType = value;
            }
        }

        /// <summary>
        /// initial margine required for this account - depends on the
        /// number and type of products
        /// </summary>
        [DataMember]
        [System.Data.Linq.Mapping.Column]
        [JsonProperty]
        public decimal InitialMargin
        {
            get
            {
                return m_InitialMargin;
            }
            set
            {
                m_InitialMargin = value;
            }
        }

        /// <summary>
        /// maintanace margine required for this account - depends on the
        /// number and type of products
        /// </summary>
        [DataMember]
        [System.Data.Linq.Mapping.Column]
        [JsonProperty]
        public decimal MaintMargin
        {
            get
            {
                return m_MaintMargin;
            }
            set
            {
                m_MaintMargin = value;
            }
        }

        /// <summary>
        /// Net liquidity of the account
        /// </summary>
        [DataMember]
        [System.Data.Linq.Mapping.Column]
        [JsonProperty]
        public decimal NetLiquidity
        {
            get
            {
                return m_NetLiquidity;
            }
            set
            {
                m_NetLiquidity = value;
            }
        }

        /// <summary>
        /// NetLiq - initial margin
        /// </summary>
        [DataMember]
        [System.Data.Linq.Mapping.Column]
        [JsonProperty]
        public decimal AvailableFunds
        {
            get
            {
                return m_AvailableFunds;
            }
            set
            {
                m_AvailableFunds = value;
            }
        }

        /// <summary>
        /// NetLiq - manint margin
        /// </summary>
        [DataMember]
        [System.Data.Linq.Mapping.Column]
        [JsonProperty]
        public decimal ExcessFunds
        {
            get
            {
                return m_ExcessFunds;
            }
            set
            {
                m_ExcessFunds = value;
            }
        }

        #endregion
    }
}
