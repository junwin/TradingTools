//-----------------------------------------------------------------------
// <copyright file="IDOM.cs" company="KaiTrade LLC">
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
    public delegate void OnDOMUpdate(object sender, decimal? price);
    public delegate void OnDOMImage(object sender);
    public delegate void OnDOMSlotUpdate(object sender, decimal? price, decimal? bidSize, decimal? askSize);
    public interface IDOM
    {
        
        OnDOMImage DOMImage
        { get; set; }

        OnDOMUpdate DOMUpdate
        { get; set; }

        /// <summary>
        /// Get set the DOM data associated with this object
        /// </summary>
        IDOMData DOMData
        { get; set; }

        /// <summary>
        /// Initialize a DOM 
        /// </summary>
        /// <param name="startPx"> startPX for the DOM - this will be the middle of the range</param>
        /// <param name="maxPxMovement">Maximum price movement allowed </param>
        /// <param name="minPxIncrement">minimum price increment - usually a tick</param>
        /// <returns></returns>
        IDOMData Create(decimal startPx, decimal maxPxMovement, decimal minPxIncrement);

        /// <summary>
        /// This will update the depth based on the slices passed, will replace the qty
        /// at each slice
        /// </summary>
        /// <param name="updateSlices"></param>
        void Update(IDOMSlot[] updateSlices);

        /// <summary>
        /// Update a DOM using a pxUpdate
        /// </summary>
        /// <param name="pxUpdate"></param>
        void Update(IPXUpdate pxUpdate);

        /// <summary>
        /// Update a specifc bid offer
        /// </summary>
        /// <param name="domData"></param>
        /// <param name="price">price to apply update</param>
        /// <param name="BidSize">bid size at that level</param>
        /// <param name="AskSize">asksize at the level</param>
        void Update(decimal price, decimal? BidSize, decimal? AskSize);

        /// <summary>
        /// Get the slot index for some price
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        int GetSlotIndex(decimal price);
    }
}
