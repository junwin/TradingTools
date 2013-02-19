//-----------------------------------------------------------------------
// <copyright file="ISession.cs" company="KaiTrade LLC">
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
    /// Interface of an object that represents a sessino between a user of
    /// the system and a remote instance of KaiTrade
    /// </summary>
    public interface ISession
    {
        /// <summary>
        /// Get the unique identifer for the sesssion
        /// </summary>
        string Identity
        { get; set; }

        /// <summary>
        /// Get/Set the user associated with the session
        /// </summary>
        string UserID
        { get;set;}

        /// <summary>
        /// Set the session correlation ID
        /// </summary>
        string CorrelationID
        { get; set; }
    }
}
