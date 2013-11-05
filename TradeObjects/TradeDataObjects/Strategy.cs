//-----------------------------------------------------------------------
// <copyright file="Account.cs" company="KaiTrade LLC">
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
    /// <summary>
    /// This class models some strategy that can be run, it typlically contains N orders
    /// and a product (single or multi leg)
    /// </summary>
    [global::System.Data.Linq.Mapping.TableAttribute(Name = "dbo.Strategy")]
    [JsonObject(MemberSerialization.OptIn)]
    [DataContract]
    [KnownType(typeof(K2DataObjects.Parameter))]
    public class StrategyData : KaiTrade.Interfaces.IStrategy
    {
        /// <summary>
        /// tag used on messages to subscribers
        /// </summary>
        //NOT USED? private string m_Tag = "";

        private string m_RunIdentifier = "";
        private string m_CorrelationID  ="";

        /// <summary>
        /// Friendly nickname for this strategy
        /// </summary>
        private string m_UserName = "";

        /// <summary>
        /// User id (or system ID) that the strategy belongs to
        /// </summary>
        private string m_UserID;

        private string m_SessionID;

        /// <summary>
        /// Type of the strategy
        /// </summary>
        private KaiTrade.Interfaces.StrategyType m_Type = KaiTrade.Interfaces.StrategyType.other;

        /// <summary>
        /// The identity of the last order processed in this strategy
        /// </summary>
        private string m_LastOrdIdentity = "";

        /// <summary>
        /// Alg used to process price changes for the strategy
        /// </summary>
        //NOT USED? KaiTrade.Interfaces.PriceAlg m_PXAlg = null;

        /// <summary>
        /// Price alg name
        /// </summary>
        string m_PXAlgName = "";

        /// <summary>
        /// The algorithm used for order routing operations
        /// </summary>
        //NOT USED? private KaiTrade.Interfaces.ORStrategyAlgorithm m_ORAlg = null;

        /// <summary>
        /// Trigger name associated with the strategy
        /// </summary>
        private string m_TriggerName = "";

        private List<string> m_SignalNames;

        private string m_EntryTriggerName = "";
        private string m_ExitTriggerName = "";

        /// <summary>
        /// determines if we try to connect am Exit and entry name to a driver
        /// assuming the driver supports conditions e.g. CQG
        /// </summary>
        private bool m_AutoConnectTrg = false;

        /// <summary>
        /// The interval used on conditons used for autoConnect
        /// </summary>
        private int m_ConditionInterval = 1;
        /// <summary>
        /// is the strategy enabled to run/trade
        /// </summary>
        private bool m_Enabled = false;

        private DateTime m_StartTime;

        private DateTime m_EndTime;

        /// <summary>
        /// determines if the start and end times are used
        /// </summary>
        private bool m_UseStrategyTimes = false;

        /// <summary>
        /// Identity of the product for this strategy
        /// </summary>
        private string m_ProductID = "";

        /// <summary>
        /// Product mnemonic
        /// </summary>
        private string m_Mnemonic = "";

        /// <summary>
        /// Used when getting data, if not specified then the base menmonic is
        /// used for both trades and data
        /// </summary>
        private string m_DataMnemonic = "";

        /// <summary>
        /// Product used in the strategy
        /// </summary>
        //NOT USED? private KaiTrade.Interfaces.TradableProduct m_Product = null;

        /// <summary>
        /// default or last Side of the strategy
        /// </summary>
        string m_Side = null;
        private string m_ShortSaleLocate = "";

        /// <summary>
        /// Default order type for the strategy
        /// </summary>
        private string m_OrdType = null;

        /// <summary>
        /// Time in force for the strategy
        /// </summary>
        private string m_TimeInForce = "Day";

        /// <summary>
        /// Date/Time for time in force
        /// </summary>
        private string m_TIFDateTime = "";

        /// <summary>
        /// This is the qty waiting to be processed, it is used where the qtyLimit is
        /// greater than the qty - in this case the strategy will be run repeatedly
        /// until the full(QtyLimit) is processed
        /// It is set at Entry()
        /// </summary>
        //NOT USED? private double m_QtyWaitingToProcess = 0;

        private double m_MaxPrice = double.MaxValue;
        private double  m_MinPrice = 1;

        /// <summary>
        /// Qty limit for the strategy
        /// </summary>
        private double m_QtyLimit = 0.0;

        /// <summary>
        /// Default qty for the strategy
        /// </summary>
        private double m_Qty = 0.0;

        /// <summary>
        /// Max floor used for icebergs
        /// </summary>
        private double? m_MaxFloor = null;

        /// <summary>
        /// Default Price for the strategy
        /// </summary>
        private double m_Price = 0.0;

        /// <summary>
        /// StopPX only applied to single legs
        /// </summary>
        private double m_StopPx = 0.0;

        /// <summary>
        /// Max number of iterations allowed for any alg running in the
        /// strategy
        /// </summary>
        private int m_MaxIterations = 99999;

        /// <summary>
        /// the maximum number of times the strategy may be entered - this is reset
        /// when a strategy is loaded or created, unlike max iterations which is the max number of
        /// runs/orders perminted in each entry, this is the max number of times you
        /// can enter the strategy. It defaults to -1 i.e. not limited
        /// </summary>
        private int m_MaxEntries = -1;

        /// <summary>
        /// number of times we have entered this strategy since it was created or loaded
        /// </summary>
        //NOT USED? private int m_EntryCount = 0;

        /// <summary>
        /// Account used on strategy orders
        /// </summary>
        private string m_Account = "";

        /// <summary>
        /// path of a TS Query group that may be used in the
        /// strategy
        /// </summary>
        private string m_TSQueryGroupPath = "";

        /// <summary>
        /// Free format info about the strategy - for example output from an Algo
        /// </summary>
        private string m_Info = "";

        /// <summary>
        /// determines if filled positions are flattened on Exit
        /// </summary>
        private bool m_FlattenOnExit = true;

        /// <summary>
        /// Determies if working orders are cancelled on exit
        /// </summary>
        private bool m_CancelOnExit = true;

        /// <summary>
        /// State of the strategy
        /// </summary>
        private KaiTrade.Interfaces.StrategyState m_State = KaiTrade.Interfaces.StrategyState.init;

        // has this strategy been initialized - i.e. key data fields set ?
        // Assume it is - this is used by RTD
        private bool m_Initialized = true;

        /// <summary>
        /// unique identifier for the order
        /// </summary>
        private string m_Identity;

        private string m_AlgName = "";
        private List<KaiTrade.Interfaces.IParameter> m_K2Parameters;

        private bool m_AutoCreate = false;

        public StrategyData()
        {
            m_K2Parameters = new List<KaiTrade.Interfaces.IParameter>();
            m_SignalNames = new List<string>();
        }
        /// <summary>
        /// get the identity of the last order submitted for this strategy
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(255)")]
        public string LastOrdIdentity
        {
            get
            {
                return m_LastOrdIdentity;
            }
            set
            {
                m_LastOrdIdentity = value;
            }
        }

        #region Strategy Members

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(36)")]
        public string User
        {
            get { return m_UserID; }
            set { m_UserID = value; }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(36)")]
        public string SessionID
        {
            get { return m_SessionID; }
            set { m_SessionID = value; }
        }

        [DataMember]
        [JsonProperty]
        [global::System.Data.Linq.Mapping.ColumnAttribute(Name = "[Identity]", Storage = "m_Identity", DbType = "NVarChar(36) NOT NULL", CanBeNull = false, IsPrimaryKey = true)]
        public string Identity
        {
            get
            {
                return m_Identity;
            }
            set
            {
                m_Identity = value;
            }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public bool AutoCreate
        {
            get
            {
                return m_AutoCreate;
            }
            set
            {
                m_AutoCreate = value;
            }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(36)")]
        public string ProductID
        {
            get
            {
                return m_ProductID;
            }
            set
            {
                m_ProductID = value;
            }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(256)")]
        public string Mnemonic
        {
            get
            {
                return m_Mnemonic;
            }
            set
            {
                m_Mnemonic = value;
            }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(256)")]
        public string DataMnemonic
        {
            get { return m_DataMnemonic; }
            set { m_DataMnemonic = value; }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(1)")]
        public string Side
        {
            get
            {
                return m_Side;
            }
            set
            {
                m_Side = value;
            }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(256)")]
        public string ShortSaleLocate
        {
            get
            {
                return m_ShortSaleLocate;
            }
            set
            {
                m_ShortSaleLocate = value;
            }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(1)")]
        public string OrdType
        {
            get
            {
                return m_OrdType;
            }
            set
            {
                m_OrdType = value;
            }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(26)")]
        public string TimeInForce
        {
            get
            {
                return m_TimeInForce;
            }
            set
            {
                m_TimeInForce = value;
            }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(26)")]
        public string TIFDateTime
        {
            get
            {
                return m_TIFDateTime;
            }
            set
            {
                m_TIFDateTime = value;
            }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public double QtyLimit
        {
            get
            {
                return m_QtyLimit;
            }
            set
            {
                m_QtyLimit = value;
            }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public int MaxIterations
        {
            get { return m_MaxIterations; }
            set { m_MaxIterations = value; }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public int MaxEntries
        {
            get { return m_MaxEntries; }
            set { m_MaxEntries = value; }
        }

        /// <summary>
        /// Max price for the strategy: max price allowed on any orders from the strategey - confirm that the algo used supports this
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public double  MaxPrice
        {
            get { return m_MaxPrice; }
            set { m_MaxPrice = value; }
        }

        /// <summary>
        /// Min price for the strategy: min price allowed on any orders from the strategey - confirm that the algo used supports this
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public double  MinPrice
        {
            get { return m_MinPrice; }
            set { m_MinPrice = value; }
        }

        /// <summary>
        /// Set the default qty  for the strategy
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public double Qty
        {
            get
            {
                return m_Qty;
            }
            set
            {
                m_Qty = value;
            }
        }

        /// <summary>
        /// Get/Set the max floor
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public double? MaxFloor
        {
            get { return m_MaxFloor; }
            set { m_MaxFloor = value; }
        }

        /// <summary>
        /// Set the default price  for the strategy
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public double Price
        {
            get
            {
                return m_Price;
            }
            set
            {
                m_Price = value;
            }
        }

        /// <summary>
        /// Determines if we flatten position on Exit
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public bool FlattenOnExit
        {
            get { return m_FlattenOnExit; }
            set
            {
                m_FlattenOnExit = value;
            }
        }

        /// <summary>
        /// Determines if we cancels working orders  on Exit
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public bool CancelOnExit
        {
            get { return m_CancelOnExit; }
            set
            {
                m_CancelOnExit = value;
            }
        }

        /// <summary>
        /// determine if we use the strategies start and end times
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public bool UseStrategyTimes
        {
            get { return m_UseStrategyTimes; }
            set { m_UseStrategyTimes = value; }
        }

        /// <summary>
        /// Time of day the strategy can run from(start time) - if specified not time limits
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public DateTime StartTime
        {
            get { return m_StartTime; }
            set
            {
                m_StartTime = value;
            }
        }

        /// <summary>
        /// Time of day the strategy can run to(end time) - if specified not time limits
        /// if the strategy is an enter state when the end time passes the strategy will
        /// Exit and obey any exit rules in force
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public DateTime EndTime
        {
            get { return m_EndTime; }
            set
            {
                m_EndTime = value;
            }
        }

        /// <summary>
        /// get/set the user name for the strategy
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(256)")]
        public string UserName
        {
            get
            {
                return m_UserName;
            }
            set
            {
                m_UserName = value;
            }
        }

        /// <summary>
        /// Get/Set the stop PX for the strategy - note the behaviour
        /// depends on the type of strategy, in general this works only for
        /// single legs
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public double StopPx
        {
            get { return m_StopPx; }
            set { m_StopPx = value; }
        }

        /// <summary>
        /// Get/Set the account for the strategy
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(256)")]
        public string Account
        {
            get { return m_Account; }
            set { m_Account = value; }
        }

        /// <summary>
        /// Get/Set the type of strategy
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public KaiTrade.Interfaces.StrategyType Type
        {
            get
            {
                return m_Type;
            }
            set
            {
                m_Type = value;
            }
        }

        /// <summary>
        /// Get/Set the name of the PXAlgorithm used in the strategy
        /// this is used for load/store of the strategy so that we can
        /// create an instance of the alg as required.
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(256)")]
        public string PXAlgorithmName
        {
            get { return m_PXAlgName; }
            set { m_PXAlgName = value; }
        }

        /// <summary>
        /// Get the trigger name used for this strategy
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(256)")]
        public string TriggerName
        {
            get
            {
                return m_TriggerName;
            }
            set
            {
                m_TriggerName = value;
            }
        }

        [DataMember]
        [JsonProperty]
        public List<string> SignalNames
        {
            get { return m_SignalNames; }
            set { m_SignalNames = value; }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(256)")]
        public string EntryTriggerName
        {
            get { return m_EntryTriggerName; }
            set { m_EntryTriggerName = value; }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(256)")]
        public string ExitTriggerName
        {
            get { return m_ExitTriggerName; }
            set { m_ExitTriggerName = value; }
        }

        /// <summary>
        /// Add an exit trigger
        /// </summary>
        /// <param name="name">name of trigger</param>
        /// <param name="isSet">state of trigger</param>
        public void AddExitTriggerName(string name, bool isSet)
        {
        }

        /// <summary>
        /// Add an entry trigger
        /// </summary>
        /// <param name="name">name of trigger</param>
        /// <param name="isSet">state of trigger</param>
        public void AddEntryTriggerName(string name, bool isSet)
        {
        }

        /// <summary>
        /// get/set whether the strategy will attempt to auto connect the enter and exit
        /// trigger names to constions in the trade venue
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public bool AutoConnectTrg
        {
            get { return m_AutoConnectTrg; }
            set { m_AutoConnectTrg = value; }
        }

        /// <summary>
        /// Get/Set the interval used on conditions
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public int ConditionInterval
        {
            get { return m_ConditionInterval; }
            set { m_ConditionInterval = value; }
        }

        /// <summary>
        /// Get/Set the enabled flag for the strategy
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public bool Enabled
        {
            get { return m_Enabled; }
            set { m_Enabled = value; }
        }

        /// <summary>
        /// Get/Set the name of the OR Alg used in the strategy
        /// this is used for load/store of the strategy so that we can
        /// create an instance of the alg as required.
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(256)")]
        public string ORAlgorithmName
        {
            get { return m_AlgName; }
            set { m_AlgName = value; }
        }

        /// <summary>
        /// get/set the list of parameters used with the alg
        /// </summary>
        [DataMember]
        [JsonProperty]
        public List<KaiTrade.Interfaces.IParameter> K2Parameters
        {
            get { return m_K2Parameters; }
            set { m_K2Parameters = value; }
        }

        /// <summary>
        /// Get/Set paramters as a string delimited bag of values
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(4000)")]
        public string ParameterBag
        {
            get
            {
                //return Factory.Instance().AppFacade.BagAsString(m_AlgParameters, "|");
                return "";
            }
            set
            {
                //Factory.Instance().AppFacade.SetBag(out m_AlgParameters, value, "|");
            }
        }

        /// <summary>
        /// The identifier for a particuar run/entry  of the strategy
        /// can be used to publish status and information from a particular run
        /// for example fills for a particular run.
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(256)")]
        public string RunIdentifier
        {
            get
            {
                return m_RunIdentifier;
            }
            set
            {
                m_RunIdentifier = value;
            }
        }

        /// <summary>
        /// Identifier used to track a trade system use of orders, strategeies and algos against some
        /// ID
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(256)")]
        public string CorrelationID
        {
            get { return m_CorrelationID; }
            set { m_CorrelationID = value; }
        }

        /// <summary>
        /// Get/Set the path of a TS Query group that may be used in the
        /// strategy
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(1024)")]
        public string TSQueryGroupPath
        {
            get { return m_TSQueryGroupPath; }
            set { m_TSQueryGroupPath = value; }
        }

        /// <summary>
        /// Free format info about the strategy - for example output from an Algo
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(512)")]
        public string Info
        {
            get { return m_Info; }
            set
            {
                m_Info = value;
            }
        }

        /// <summary>
        /// Get the state of the strategy
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public KaiTrade.Interfaces.StrategyState State
        {
            get { return m_State; }
            set
            {
                m_State = value;
            }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public bool Initialized
        {
            get { return m_Initialized; }
            set { m_Initialized = value; }
        }
    }

#endregion

}
