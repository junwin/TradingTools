//-----------------------------------------------------------------------
// <copyright file="IPublisher.cs" company="KaiTrade LLC">
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
    /// Defines the states a publisher or other object can have
    /// </summary>
    public enum Status
    {
        loaded, opening, open, closed, closing, error, undefined
    }

 /// <summary>
 /// Defines an object that can publish data about some specific topic
 /// </summary>
    public interface IPublisher
    {
        /// <summary>
        /// Subscribe the observer specified to the subject
        /// </summary>
        /// <param name="mySubscriber">some susbcriber interface</param>
        void Subscribe(ISubscriber mySubscriber);

        /// <summary>
        /// Unsubscribe the subscriber passed
        /// </summary>
        /// <param name="mySubscriber">some subscriber</param>
        void UnSubscribe(ISubscriber mySubscriber);

        /// <summary>
        /// Return a key based on the state data passed in
        /// </summary>
        /// <param name="myData"></param>
        /// <returns></returns>
        string TopicID(string myData);

        /// <summary>
        /// Return a key for the this subject that can be used to
        /// look up the subject in some map of subjects
        /// </summary>
        /// <returns></returns>
        string TopicID();

        /// <summary>
        /// Open the subject passing data used by this type
        /// of subject.
        /// </summary>
        /// <param name="myData">Data (XML) used by the subject</param>
        /// <returns>A key that can be used to lookup the subject</returns>
        string Open(string myData);

        /// <summary>
        /// Close this subject
        /// </summary>
        void Close();

        /// <summary>
        /// Get/Set the fields list for the publisher - setting this will replace all
        /// existing fields in the publisher and issue an image
        /// </summary>
        System.Collections.Generic.List<KaiTrade.Interfaces.Field> FieldList
        {
            get;
            set;
        }
        /// <summary>
        /// Called by a client/data feed with a complete set of fields - this sets up an image in the subject
        /// </summary>
        /// <param name="itemList">list of items</param>
        void OnImage(System.Collections.Generic.List<Field> itemList);

        /// <summary>
        /// Called by a client/data feed when one or more fields value changes
        /// </summary>
        /// <param name="itemList">list of changed items</param>
        void OnUpdate(System.Collections.Generic.List<Field> itemList);

        /// <summary>
        /// update some arbitary field in the publisher - note not all publishers
        /// may support this
        /// </summary>
        /// <param name="myID"></param>
        /// <param name="myValue"></param>
        void OnUpdate(string myID, string myValue);

        /// <summary>
        /// Update the publisher with some price update - not all publishers
        /// will action this.
        /// </summary>
        /// <param name="pxUpdate"></param>
        void OnUpdate(string mnemonic, KaiTrade.Interfaces.IPXUpdate pxUpdate);

        /// <summary>
        /// Called when the client/datafeed status changes
        /// </summary>
        /// <param name="itemList"></param>
        void OnStatusChange(System.Collections.Generic.List<Field> itemList);

        /// <summary>
        /// get/set the publisher base status - will event all subscribers
        /// </summary>
        KaiTrade.Interfaces.Status Status
        { get; set;}

        /// <summary>
        /// User defined string tag
        /// </summary>
        string Tag
        {
            get;
            set;
        }

        /// <summary>
        /// Get/Set the publisher type - this is user defined
        /// </summary>
        string PublisherType
        { get; set; }
    }
}
