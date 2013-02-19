//-----------------------------------------------------------------------
// <copyright file="IMQRoutingkey.cs" company="KaiTrade LLC">
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
using System.Linq;




namespace KaiTrade.Interfaces
{
    /// <summary>
    /// Defines a routing key used to send message fix, fixml,  MQ, RMQ etc
    /// </summary>
    public interface IMQRoutingKey
    {
        /// <summary>
        /// Name of the exchnage 
        /// </summary>
        string Name
        { get; set; }

        /// <summary>
        /// MQ Name of the exchange
        /// </summary>
        string Exchange
        { get; set; }

        /// <summary>
        /// Queue name - this is optional, if specified you need to 
        /// create the queue or use an existing queue, some apps
        /// will use this to set a userID or CorrelationId
        /// </summary>
        string QueueName
        { get; set; }

        /// <summary>
        /// Type of exchange - e.g. orders tsignal, subscribe, update  
        /// </summary>
        string Type
        { get; set; }

        /// <summary>
        /// The key used for routing
        /// </summary>
        string Key
        { get; set; }

        /// <summary>
        /// Is the exchange enabled
        /// </summary>
        bool Enabled
        { get; set; }
    }
}
