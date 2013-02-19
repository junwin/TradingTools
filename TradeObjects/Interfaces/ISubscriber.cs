//-----------------------------------------------------------------------
// <copyright file="ISubscriber.cs" company="KaiTrade LLC">
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
    public interface ISubscriber
    {
        /// <summary>
        /// Called by a publisher with a complete set of fields - always the
        /// first message after a a subscriber  subcribes to some topic
        /// </summary>
        /// <param name="mySender">The sending publisher - can be null</param>
        /// <param name="itemList">list of fields and values</param>
        void OnImage(IPublisher mySender, System.Collections.Generic.List<Field> itemList);

       /// <summary>
        /// Called by a publisher when one or more fields value changes
       /// </summary>
        /// <param name="mySender">The sending publisher - can be null</param>
       /// <param name="itemList">list of changed feilds</param>
        void OnUpdate(IPublisher mySender, System.Collections.Generic.List<Field> itemList);

        /// <summary>
        /// Called when the subject status changes
        /// </summary>
        /// <param name="mySender">The sending publisher - can be null</param>
        /// <param name="itemList">list of status fields</param>
        void OnStatusChange(IPublisher mySender, System.Collections.Generic.List<Field> itemList);
    }
}
