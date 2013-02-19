//-----------------------------------------------------------------------
// <copyright file="IDriverData.cs" company="KaiTrade LLC">
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

namespace KaiTrade.Interfaces
{
    /// <summary>
    /// Defines the data that describes some driver
    /// </summary>
    public interface IDriverData
    {
        /// <summary>
        /// Unique identity for this plugin
        /// </summary>
        string Identity
        { get; set; }

        /// <summary>
        /// Path/Url for plugin
        /// </summary>
        string Path
        { get; set; }

        /// <summary>
        /// Is the plugin enabled
        /// </summary>
        bool Enabled
        { get; set; }

        /// <summary>
        /// Name of the plugin
        /// </summary>
        string Name
        { get; set; }

        /// <summary>
        /// PlugIn Vendor
        /// </summary>
        string Vendor
        { get; set; }

      
        /// <summary>
        /// String based config data
        /// </summary>
        string Config
        { get; set; }

        /// <summary>
        /// User signon - for driver to access broker
        /// </summary>
        string UserID
        { get; set; }

        /// <summary>
        /// Password for driver to accesss server
        /// </summary>
        string Password
        { get; set; }

        /// <summary>
        /// Server address ip or Name
        /// </summary>
        string ServerName
        { get; set; }

        /// <summary>
        /// Server port
        /// </summary>
        string ServerPort
        { get; set; }

        //IMQExchange MQExch
        //IMQRoutingKey

        /// <summary>
        /// Driver code
        /// </summary>
        string Code
        { get; set; }

        /// <summary>
        /// Code of driver that this driver routes traffic (if this is some type o
        /// proxy
        /// </summary>
        string RouteCode
        { get; set; }

        /// <summary>
        /// Path to config file
        /// </summary>
        string ConfigPath
        { get; set; }

        /// <summary>
        /// Driver does not start automatically
        /// </summary>
        bool ManualStart
        { get; set; }

        /// <summary>
        /// Used on live market
        /// </summary>
        bool LiveMarket
        { get; set; }

        /// <summary>
        /// Hide any drive UI 
        /// </summary>
        bool HideDriverUI
        { get; set; }

        /// <summary>
        /// Use asyncronous price handling
        /// </summary>
        bool AsyncPrices
        { get; set; }

        /// <summary>
        /// Use queued replace and cancels
        /// </summary>
        bool QueueReplaceRequests
        { get; set; }



    }
}
