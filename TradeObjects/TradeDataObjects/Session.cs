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

namespace K2DataObjects
{
    [DataContract]
    [global::System.Data.Linq.Mapping.TableAttribute(Name = "dbo.Session")]
    public class Session : KaiTrade.Interfaces.ISession
    {
        string m_Identity;
        string m_UserID;
        string m_CorrelationID;

        public Session()
        {
            m_Identity = System.Guid.NewGuid().ToString();
        }

        [DataMember]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(255)")]
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
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(36)")]
        public string UserID
        {
            get
            {
                return m_UserID;
            }
            set
            {
                m_UserID = value;
            }
        }

        [DataMember]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(255)")]
        public string CorrelationID
        {
            get
            {
                return m_CorrelationID;
            }
            set
            {
                m_CorrelationID = value;
            }
        }
    }
}
