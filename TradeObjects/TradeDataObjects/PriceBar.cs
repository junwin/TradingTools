//-----------------------------------------------------------------------
// <copyright file="PriceBar.cs" company="KaiTrade LLC">
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
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace K2DataObjects
{
    [DataContract]
    [JsonObject(MemberSerialization.OptIn)]
    [global::System.Data.Linq.Mapping.TableAttribute(Name = "dbo.PriceBar")]
    public  class PriceBar
    {

        private string _Mnemonic;

        private string _RequestID;

        private int _ItemSize;

        private int _ItemType;

        private long _TimeStamp;

        private decimal _High;

        private decimal _Low;

        private decimal _Open;

        private decimal _Close;

        private System.Nullable<decimal> _Avg;

        private System.Nullable<decimal> _Volume;

        private System.Nullable<decimal> _BidVolume;

        private System.Nullable<decimal> _AskVolume;

       

        public PriceBar()
        {
         
        }

        [DataMember]
        [JsonProperty]
        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_Mnemonic", DbType = "NVarChar(256) NOT NULL", CanBeNull = false, IsPrimaryKey = true)]
        public string Mnemonic
        {
            get
            {
                return this._Mnemonic;
            }
            set
            {
                if ((this._Mnemonic != value))
                {

                    this._Mnemonic = value;

                }
            }
        }

        [DataMember]
        [JsonProperty]
        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_RequestID", DbType = "NVarChar(36) NOT NULL", CanBeNull = false, IsPrimaryKey = true)]
        public string RequestID
        {
            get
            {
                return this._RequestID;
            }
            set
            {
                if ((this._RequestID != value))
                {

                    this._RequestID = value;

                }
            }
        }

        [DataMember]
        [JsonProperty]
        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_ItemSize", DbType = "Int NOT NULL", IsPrimaryKey = true)]
        public int ItemSize
        {
            get
            {
                return this._ItemSize;
            }
            set
            {
                if ((this._ItemSize != value))
                {

                    this._ItemSize = value;

                }
            }
        }

        [DataMember]
        [JsonProperty]
        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_ItemType", DbType = "Int NOT NULL", IsPrimaryKey = true)]
        public int ItemType
        {
            get
            {
                return this._ItemType;
            }
            set
            {
                if ((this._ItemType != value))
                {

                    this._ItemType = value;

                }
            }
        }

        [DataMember]
        [JsonProperty]
        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_TimeStamp", DbType = "BigInt NOT NULL", IsPrimaryKey = true)]
        public long TimeStamp
        {
            get
            {
                return this._TimeStamp;
            }
            set
            {
                if ((this._TimeStamp != value))
                {

                    this._TimeStamp = value;

                }
            }
        }

        [DataMember]
        [JsonProperty]
        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_High", DbType = "Decimal(18,6) NOT NULL")]
        public decimal High
        {
            get
            {
                return this._High;
            }
            set
            {
                if ((this._High != value))
                {

                    this._High = value;

                }
            }
        }

        [DataMember]
        [JsonProperty]
        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_Low", DbType = "Decimal(18,6) NOT NULL")]
        public decimal Low
        {
            get
            {
                return this._Low;
            }
            set
            {
                if ((this._Low != value))
                {
 
                    this._Low = value;

                }
            }
        }

        [DataMember]
        [JsonProperty]
        [global::System.Data.Linq.Mapping.ColumnAttribute(Name = "[Open]", Storage = "_Open", DbType = "Decimal(18,6) NOT NULL")]
        public decimal Open
        {
            get
            {
                return this._Open;
            }
            set
            {
                if ((this._Open != value))
                {

                    this._Open = value;

                }
            }
        }

        [DataMember]
        [JsonProperty]
        [global::System.Data.Linq.Mapping.ColumnAttribute(Name = "[Close]", Storage = "_Close", DbType = "Decimal(18,6) NOT NULL")]
        public decimal Close
        {
            get
            {
                return this._Close;
            }
            set
            {
                if ((this._Close != value))
                {

                    this._Close = value;

                }
            }
        }

        [DataMember]
        [JsonProperty]
        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_Avg", DbType = "Decimal(18,6)")]
        public System.Nullable<decimal> Avg
        {
            get
            {
                return this._Avg;
            }
            set
            {
                if ((this._Avg != value))
                {

                    this._Avg = value;

                }
            }
        }

        [DataMember]
        [JsonProperty]
        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_Volume", DbType = "Decimal(18,0)")]
        public System.Nullable<decimal> Volume
        {
            get
            {
                return this._Volume;
            }
            set
            {
                if ((this._Volume != value))
                {

                    this._Volume = value;

                }
            }
        }

        [DataMember]
        [JsonProperty]
        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_BidVolume", DbType = "Decimal(18,0)")]
        public System.Nullable<decimal> BidVolume
        {
            get
            {
                return this._BidVolume;
            }
            set
            {
                if ((this._BidVolume != value))
                {

                    this._BidVolume = value;

                }
            }
        }

        [DataMember]
        [JsonProperty]
        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_AskVolume", DbType = "Decimal(18,0)")]
        public System.Nullable<decimal> AskVolume
        {
            get
            {
                return this._AskVolume;
            }
            set
            {
                if ((this._AskVolume != value))
                {

                    this._AskVolume = value;

                }
            }
        }

        
    }
}
