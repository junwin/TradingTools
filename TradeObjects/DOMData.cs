//-----------------------------------------------------------------------
// <copyright file="DOMData.cs" company="KaiTrade LLC">
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
using System.ServiceModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace K2DataObjects
{
    [DataContract]
    [JsonObject(MemberSerialization.OptIn)]
    public class DOMData :KaiTrade.Interfaces.IDOMData
    {
        private decimal basePrice;
        private KaiTrade.Interfaces.IDOMSlot[] DOMSlots;
        private int maxSlots;
        private decimal minPxIncrement;

        [DataMember]
        [JsonProperty]
        public decimal BasePrice
        {
            get
            {
                return basePrice;
            }
            set
            {
                basePrice = value;
            }
        }

        [DataMember]
        [JsonProperty]
        public KaiTrade.Interfaces.IDOMSlot[] K2DOMSlots
        {
            get
            {
                return DOMSlots;
            }
            set
            {
                DOMSlots = value;
            }
        }

        [DataMember]
        [JsonProperty]
        public int MaxSlots
        {
            get
            {
                return maxSlots;
            }
            set
            {
                maxSlots = value;
            }
        }

        [DataMember]
        [JsonProperty]
        public decimal MinPxIncrement
        {
            get
            {
                return minPxIncrement;
            }
            set
            {
                minPxIncrement = value;
            }
        }
    }
}
