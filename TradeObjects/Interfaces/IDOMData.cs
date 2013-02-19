//-----------------------------------------------------------------------
// <copyright file="IDOMData.cs" company="KaiTrade LLC">
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
    

    public interface IDOMData
    {
        decimal MinPxIncrement
        { get; set; }

        int MaxSlots
        { get; set; }

        /// <summary>
        /// Price that is the low point of the slots - this is offset from the
        /// start price used to create the DOMData
        /// </summary>
        decimal BasePrice
        { get; set; }

        K2DOMSlot[] K2DOMSlots
        { get; set; }
    }

   
}
