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
using System.Text;



namespace KaiTrade.Interfaces
{
    /// <summary>
    /// Defines tha data for a Rabbit MQ exchange
    /// </summary>
    public interface IMQExchange
    {
        /// <summary>
        /// Name of the exchnage 
        /// </summary>
        string Name
        {get; set;}

        /// <summary>
        /// RMQ Name of the exchange
        /// </summary>
        string Exchange
        {get;set;}

        /// <summary>
        /// Type of exchange - e.g. orders tsignal, subscribe, update  
        /// </summary>
        string Type
        {get; set;}

        /// <summary>
        /// Is the exchange enabled
        /// </summary>
        bool Enabled
        {get;set;}
    }
}
