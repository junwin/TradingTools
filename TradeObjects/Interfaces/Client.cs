//-----------------------------------------------------------------------
// <copyright file="IClient.cs" company="KaiTrade LLC">
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
    /// Provides an interface that is implemented by objects that want to
    /// receive messaged - this normally used to subscribe to updates from the
    /// application facade. See also the Subscriber interface used for prices
    /// </summary>
    public interface IClient
    {
        /// <summary>
        /// Handle a status message sent to the client
        /// </summary>
        /// <param name="myMessage">KTA Message to process</param>
        void OnStatusMessage(Message myMessage);

        /// <summary>
        /// Handle a general message sent to the client
        /// </summary>
        /// <param name="myMessage">KTA Message to process</param>
        void OnMessage(Message myMessage);
    }
}
