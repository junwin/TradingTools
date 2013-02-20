//-----------------------------------------------------------------------
// <copyright file="TradeVenueDestination.cs" company="KaiTrade LLC">
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
    [global::System.Data.Linq.Mapping.TableAttribute(Name = "dbo.TradeVenueDest")]
    [JsonObject(MemberSerialization.OptIn)]
    public class TradeVenueDestination : KaiTrade.Interfaces.IVenueTradeDestination
    {
        string m_ExDestination = "";
        string m_ExchangeCode = "";
        string m_CFICode = "";
        string m_VenueCode = "";

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
            }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(255)")]
        public string ExchangeCode
        {
            get
            {
                return m_ExchangeCode;
            }
            set
            {
                m_ExchangeCode = value;
            }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(16)")]
        public string PrimaryCFICode
        {
            get
            {
                return m_CFICode;
            }
            set
            {
                m_CFICode = value;
            }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(255)")]
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
    }
}
