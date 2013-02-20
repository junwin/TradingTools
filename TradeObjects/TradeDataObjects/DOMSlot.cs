//-----------------------------------------------------------------------
// <copyright file="DOMSlot.cs" company="KaiTrade LLC">
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
    public class DOMSlot : KaiTrade.Interfaces.IDOMSlot
    {
        private decimal? askSize;
        private decimal? bidSize;
        private decimal price;
        private long lastUpdateTicks;

        private KaiTrade.Interfaces.OnDOMSlotUpdate domSlotUpdate = null;

        public void UpdateClients()
        {
            if (domSlotUpdate != null)
            {
                domSlotUpdate(this, price, bidSize, askSize);
            }
        }

        public DOMSlot(decimal price, decimal? bidSize, decimal? askSize)
        {
            Price = price;
            BidSize = bidSize;
            AskSize = askSize;
            LastUpdateTicks = DateTime.Now.Ticks;
        }

        public KaiTrade.Interfaces.OnDOMSlotUpdate DOMSlotUpdate
        {
            get
            {
                return domSlotUpdate;
            }
            set
            {
                domSlotUpdate = value;
            }
        }

        [DataMember]
        [JsonProperty]
        public decimal? AskSize
        {
            get
            {
                return askSize;
            }
            set
            {
                askSize = value;
            }
        }

        [DataMember]
        [JsonProperty]
        public decimal? BidSize
        {
            get
            {
                return bidSize;
            }
            set
            {
                bidSize = value;
            }
        }

        [DataMember]
        [JsonProperty]
        public long LastUpdateTicks
        {
            get
            {
                return lastUpdateTicks;
            }
            set
            {
                lastUpdateTicks = value ;
            }
        }

        [DataMember]
        [JsonProperty]
        public decimal Price
        {
            get
            {
                return price;
            }
            set
            {
                price = value;
            }
        }


        public override string ToString()
        {
            string myRet = "";
            try
            {
                myRet = string.Format("Price {0}, BidSize {1}, AskSize {2}, LastUpdate {3}", this.Price.ToString(), this.BidSize.ToString(), this.AskSize.ToString(), this.LastUpdateTicks.ToString());
                
            }
            catch
            {
            }

            return myRet;
        }
    }
}
