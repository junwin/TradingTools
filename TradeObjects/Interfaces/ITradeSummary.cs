//-----------------------------------------------------------------------
// <copyright file="ITradeSummary.cs" company="KaiTrade LLC">
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
    /// Defines an object that summarizes the trades for a given order at some
    /// point in time
    /// </summary>
    public interface ITradeSummary
    {
        /// <summary>
        /// setup the trade summary from an Order
        /// </summary>
        /// <param name="?"></param>
        void FromOrder(KaiTrade.Interfaces.IOrder order);

        /// <summary>
        /// KaiTrade order Identity
        /// </summary>
        string OrderIdentity
        {
            get;
            set;
        }
        /// <summary>
        /// Average price over all fills for a given order
        /// </summary>
        double AvgPx
        {
            get;
            set;
        }

        /// <summary>
        /// position +Ve is long -ve is short
        /// </summary>
        double Position
        {
            get;
            set;
        }
    }
}
