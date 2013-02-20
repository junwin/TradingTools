//-----------------------------------------------------------------------
// <copyright file="Firm.cs" company="KaiTrade LLC">
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
    /// Models some financial instritution - for example an FCM or broker
    /// </summary>
    [DataContract]
    [JsonObject(MemberSerialization.OptIn)]
    [global::System.Data.Linq.Mapping.TableAttribute(Name = "dbo.Firm")]
    public class Firm : KaiTrade.Interfaces.IFirm
    {
        private string m_ID;
        private string m_FirmCode;
        private string m_FirmName;
        private KaiTrade.Interfaces.FirmType m_FirmType;
        private bool m_External;

        /// <summary>
        /// Get set the unique identifier
        /// </summary>
        [DataMember]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(36)")]
        public string ID
        {
            get { return m_ID; }
            set { m_ID = value; }
        }

        /// <summary>
        /// Get set the unique identifier
        /// </summary>
        [DataMember]
        [JsonProperty]
        [global::System.Data.Linq.Mapping.ColumnAttribute(Name = "[FirmCode]", Storage = "m_FirmCode", DbType = "NVarChar(256) NOT NULL", CanBeNull = false, IsPrimaryKey = true)]
        public string FirmCode
        {
            get { return m_FirmCode; }
            set { m_FirmCode = value; }
        }

        /// <summary>
        /// Get set the unique identifier
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(256)")]
        public string FirmName
        {
            get { return m_FirmName; }
            set { m_FirmName = value; }
        }

        /// <summary>
        /// Set the type of firm
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public KaiTrade.Interfaces.FirmType FirmType
        {
            get { return m_FirmType; }
            set { m_FirmType = value; }
        }

        /// <summary>
        /// defines if the firm is external - for example firm we give up a trade to
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public bool External
        {
            get { return m_External; }
            set { m_External = value; }
        }
    }
}
