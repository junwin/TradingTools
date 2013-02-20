//-----------------------------------------------------------------------
// <copyright file="IMQExchange.cs" company="KaiTrade LLC">
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
    [global::System.Data.Linq.Mapping.TableAttribute(Name = "dbo.MQExchange")]
    public class MQExchange : KaiTrade.Interfaces.IMQExchange
    {

        /// <summary>
        /// Name of the exchnage 
        /// </summary>
        private string m_Name;

        /// <summary>
        /// RMQ Name of the exchange
        /// </summary>
        private string m_Exchange;
        
        /// <summary>
        /// Type of exchange - e.g. orders tsignal, subscribe, update  
        /// </summary>
        private string m_Type;
        
        /// <summary>
        /// Is the exchange enabled
        /// </summary>
        private bool m_Enabled;



        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public bool Enabled
        {
            get
            {
                return m_Enabled;
            }
            set
            {
                m_Enabled = value;
            }
        }


        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(256)")]
        public string Exchange
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

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(255)")]
        public string Type
        {
            get
            {
                return m_Type;
            }
            set
            {
                m_Type = value;
            }
        }
         
    }
}
