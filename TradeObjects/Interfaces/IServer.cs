//-----------------------------------------------------------------------
// <copyright file="IServer.cs" company="KaiTrade LLC">
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
namespace KaiTrade.Interfaces
{
    public enum ServerRole { algo, execution, generalPurpose, none }
    /// <summary>
    /// Interface for server components
    /// </summary>
    public interface IServer
    {
        /// <summary>
        /// Unique ID for the server
        /// </summary>
        string ID
        { get; set; }

        /// <summary>
        /// server name
        /// </summary>
        string Name
        { get; set; }

        /// <summary>
        /// Instance number of server if multiple instances on the host machine
        /// </summary>
        long InstanceNumber
        { get; set; }

        /// <summary>
        /// Machine name of pysical server
        /// </summary>
        string MachineName
        { get; set; }

        /// <summary>
        /// Server enabled for use
        /// </summary>
        bool Enabled
        { get; set; }

        /// <summary>
        /// Get the role of the server, for example algo server, execution server, general purpose and so on
        /// </summary>
        ServerRole ServerRole
        { get; set; }

        /// <summary>
        /// Get/Set running state of the server
        /// </summary>
        bool Running
        { get; set; }

        /// <summary>
        /// Time server started in ticks (i.e. DateTime.Now.Ticks)
        /// </summary>
        long startTimeTicks
        { get; set; }
    }
}
