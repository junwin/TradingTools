//-----------------------------------------------------------------------
// <copyright file="ClearingHouse.cs" company="KaiTrade LLC">
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
    [global::System.Data.Linq.Mapping.TableAttribute(Name = "dbo.ClearingHouse")]
    public class ClearingHouse : KaiTrade.Interfaces.IClearingHouse
    {
        private string m_Identity;
        private string m_Name;
        private string m_Code;

        public ClearingHouse()
        {
            m_Identity = System.Guid.NewGuid().ToString();
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

        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "m_Name", DbType = "NVarChar(256) NOT NULL", CanBeNull = false)]
        [DataMember]
        [JsonProperty]
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

        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "m_Code", DbType = "NVarChar(256) NOT NULL", CanBeNull = false)]
        [DataMember]
        [JsonProperty]
        public string Code
        {
            get
            {
                return m_Code;
            }
            set
            {
                m_Code = value;
            }
        }
    }
}
