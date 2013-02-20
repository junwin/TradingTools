//-----------------------------------------------------------------------
// <copyright file="Server.cs" company="KaiTrade LLC">
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

namespace K2DataObjects
{
    [DataContract]
    [global::System.Data.Linq.Mapping.TableAttribute(Name = "dbo.Server")]
    public class Server :KaiTrade.Interfaces.IServer
    {
        private bool m_Enabled;
        private string m_ID;
        private long m_InstanceNumber;
        private string m_MachineName;
        private string m_Name;
        private KaiTrade.Interfaces.ServerRole m_ServerRole = KaiTrade.Interfaces.ServerRole.none;
        private long m_StartTimeTicks;
        private bool m_Running = false;

        [DataMember]
        [System.Data.Linq.Mapping.Column]
        public bool Enabled
        {
            get { return m_Enabled; }
            set { m_Enabled = value; }
        }

        [DataMember]
        [global::System.Data.Linq.Mapping.ColumnAttribute(Name = "[ID]", Storage = "m_ID", DbType = "NVarChar(36) NOT NULL", CanBeNull = false, IsPrimaryKey = true)]
        public string ID
        {
            get { return m_ID; }
            set { m_ID = value; }
        }

        [DataMember]
        [System.Data.Linq.Mapping.Column]
        public long InstanceNumber
        {
            get { return m_InstanceNumber; }
            set { m_InstanceNumber = value; }
        }

        [DataMember]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(255)")]
        public string MachineName
        {
            get { return m_MachineName; }
            set { m_MachineName = value; }
        }

        [DataMember]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(255)")]
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        /// <summary>
        /// Get the role of the server, for example algo server, execution server, general purpose and so on
        /// </summary>
        [DataMember]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(255)")]
        public KaiTrade.Interfaces.ServerRole ServerRole
        {
            get { return m_ServerRole; }
            set { m_ServerRole = value; }
        }

        /// <summary>
        /// Get/Set running state of the server
        /// </summary>
        [DataMember]
        [System.Data.Linq.Mapping.Column]
        public bool Running
        {
            get { return m_Running; }
            set { m_Running = value; }
        }

        /// <summary>
        /// Time server started in ticks (i.e. DateTime.Now.Ticks)
        /// </summary>
        [DataMember]
        [System.Data.Linq.Mapping.Column]
        public long startTimeTicks
        {
            get { return m_StartTimeTicks; }
            set { m_StartTimeTicks = value; }
        }
    }
}
