//-----------------------------------------------------------------------
// <copyright file="User.cs" company="KaiTrade LLC">
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
    [global::System.Data.Linq.Mapping.TableAttribute(Name = "dbo.User")]
    public class User : KaiTrade.Interfaces.IUser
    {
        private string m_Identity;
        /// <summary>
        /// User identifier
        /// </summary>
        private string m_UserID;
        /// <summary>
        /// User signon name
        /// </summary>
        private string m_UserName;

        
        /// <summary>
        /// Users password
        /// </summary>
        private string m_UserPwd;
        private string m_K2Config;
        private bool m_Enabled;
        private DateTime m_LastSignIn;
        private string m_LastIP;
        private bool m_IsSignedIn=false;
        private string m_Email;

        public User()
        {
            m_Identity = System.Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Is the user enabled
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public bool Enabled
        {
            get
            {
                //return m_Enabled;
                return true;
            }
            set
            {
                m_Enabled = value;
            }
        }
        [DataMember]
        [JsonProperty]
        [global::System.Data.Linq.Mapping.ColumnAttribute(Name = "[ID]", Storage = "m_Identity", DbType = "NVarChar(36) NOT NULL", CanBeNull = false, IsPrimaryKey = true)]
        public string ID
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
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(256) NOT NULL")]
        public string UserName
        {
            get { return m_UserName; }
            set { m_UserName = value; }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(256) NOT NULL")]
        public string UserPwd
        {
            get
            {
                return m_UserPwd;
            }
            set
            {
                m_UserPwd = value;
            }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(256) NOT NULL")]
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

        /// <summary>
        /// Get/Set the users K2 config, determines access, servcies, plugings and drivers
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(4000)")]
        public string K2Config
        {
            get
            {
                return m_K2Config;
            }
            set
            {
                m_K2Config = value;
            }
        }

        /// <summary>
        /// Date time the user last signed in
        /// </summary>
        [DataMember]
        [JsonProperty]
        [global::System.Data.Linq.Mapping.ColumnAttribute(Name = "[LastSignIn]", Storage = "m_LastSignIn", DbType = "DateTime", CanBeNull = true, IsPrimaryKey = false)]
        public DateTime LastSignIn
        {
            get
            {
                return m_LastSignIn;
            }
            set
            {
                m_LastSignIn = value;
            }
        }

        /// <summary>
        /// IP address of last sign in
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(32)")]
        public string LastIP
        {
            get
            {
                return m_LastIP;
            }
            set
            {
                m_LastIP = value;
            }
        }

        /// <summary>
        /// Are they signed in true=> signed in
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public bool IsSignedIn
        {
            get
            {
                return m_IsSignedIn;
            }
            set
            {
                m_IsSignedIn = value;
            }
        }

        /// <summary>
        /// User's email
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(256)")]
        public string Email
        {
            get
            {
                return m_Email;
            }
            set
            {
                m_Email = value;
            }
        }

    }
}
