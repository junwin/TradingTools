//-----------------------------------------------------------------------
// <copyright file="IDOMSlot.cs" company="KaiTrade LLC">
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
    /// Describes a slice in a DOM (market depth )
    /// </summary>
    public interface IDOMSlot
    {
        /// <summary>
        /// Priceat this slice
        /// </summary>
        decimal Price
        { get; set; }

        /// <summary>
        /// Ask size for this slice
        /// </summary>
        decimal? AskSize
        { get; set; }

        /// <summary>
        /// BidSize
        /// </summary>
        decimal? BidSize
        { get; set; }

        long LastUpdateTicks
        { get; set; }

        /// <summary>
        /// delegate for updates on some particular slot e.g. to
        /// watch the size at some point in the depth
        /// </summary>
        OnDOMSlotUpdate DOMSlotUpdate
        { get; set; }

    }
}
