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
    [global::System.Data.Linq.Mapping.TableAttribute(Name = "dbo.CurveValue")]
    public class CurveValue 
    {

      

        private string _RequestID;

        private string _Mnemonic;

        private long _TimeStamp;

        private int _ItemType;

        private int _ItemSize;

        private string _HeaderID;

        private decimal _Value;

       

        public CurveValue()
        {
             
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
        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_HeaderID", DbType = "NVarChar(36) NOT NULL", CanBeNull = false, IsPrimaryKey = true)]
        public string HeaderID
        {
            get
            {
                return this._HeaderID;
            }
            set
            {
                if ((this._HeaderID != value))
                {

                    this._HeaderID = value;
;
                }
            }
        }

        [DataMember]
        [JsonProperty]
        [global::System.Data.Linq.Mapping.ColumnAttribute(Storage = "_Value", DbType = "Decimal(18,0) NOT NULL")]
        public decimal Value
        {
            get
            {
                return this._Value;
            }
            set
            {
                if ((this._Value != value))
                {

                    this._Value = value;

                }
            }
        }

        
    }
}
