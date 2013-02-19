//-----------------------------------------------------------------------
// <copyright file="ITradeLimits.cs" company="KaiTrade LLC">
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
    /// Defines trade limits applied within KaiTrade when routing orders
    /// </summary>
    public interface ITradeLimits
    {
        /// <summary>
        /// Max size allowed on an individual order
        /// </summary>
        double MaxOrderSize
        { get; set;}

        /// <summary>
        /// Max consideration (qty*price) allowed on an individual order
        /// </summary>
        double MaxOrderConsideration
        { get; set;}

        /// <summary>
        /// Max size allowed to be submitted in a given KaiTrade session
        /// </summary>
        double MaxSize
        { get; set;}

        /// <summary>
        /// Max Consideration in a given given KaiTrade session
        /// </summary>
        double MaxConsideration
        { get; set;}

        /// <summary>
        /// Max number of messages/iterations in a particular run of an Algo
        /// </summary>
        int MaxMessages
        { get; set;}

        /// <summary>
        /// Set values from a string of XML
        /// </summary>
        /// <param name="myXML"></param>
        void FromXML(string myXML);

        /// <summary>
        /// Return values as a string of XML
        /// </summary>
        /// <returns></returns>
        string ToXML();
    }
}
