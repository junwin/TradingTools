//-----------------------------------------------------------------------
// <copyright file="UserTrial.cs" company="KaiTrade LLC">
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
using Newtonsoft.Json;

namespace K2DataObjects
{
    [DataContract]
    [JsonObject(MemberSerialization.OptIn)]
    [global::System.Data.Linq.Mapping.TableAttribute(Name = "dbo.UserTrial")]
    public class UserTrial
    {

        /// <summary>
        /// Get the unique identifer for the user allocated by the system
        /// </summary>
        private string m_UserID;



        /// <summary>
        /// Is the user a trial user?
        /// </summary>
        private bool m_IsTrial;


        /// <summary>
        /// End of a trial
        /// </summary>
        private DateTime m_TrialEnd;


        /// <summary>
        /// Get the unique identifer for the user allocated by the system
        /// </summary>
        [DataMember]
        [JsonProperty]
        [global::System.Data.Linq.Mapping.ColumnAttribute(Name = "[ID]", Storage = "m_UserID", DbType = "NVarChar(36) NOT NULL", CanBeNull = false, IsPrimaryKey = true)]
        public string UserID
        { get { return m_UserID; } set { m_UserID = value; } }



        /// <summary>
        /// Is the user a trial user?
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public bool IsTrial
        { get { return m_IsTrial; } set { m_IsTrial = value; } }

        /// <summary>
        /// End of a trial
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public DateTime TrialEnd
        { get { return m_TrialEnd; } set { m_TrialEnd = value; } }
    }
}
