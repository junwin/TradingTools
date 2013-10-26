//-----------------------------------------------------------------------
// <copyright file="TSItem.cs" company="KaiTrade LLC">
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
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Newtonsoft.Json;

namespace K2DataObjects
{
    [DataContract]
    [KnownType(typeof(K2DataObjects.TradeSignal))]
    [KnownType(typeof(K2DataObjects.Parameter))]
    [JsonObject(MemberSerialization.OptIn)]
    [global::System.Data.Linq.Mapping.TableAttribute(Name = "dbo.PriceBar")]
    public class TSItem : KaiTrade.Interfaces.ITSItem
    {
        // Defined date values
        private string m_Mnemonic;
        private double m_Avg=0.0;
        private double m_HLC3 = 0.0;
        private double m_High = 0.0;
        private double m_Low = 0.0;
        private double m_Open = 0.0;
        private double m_Close = 0.0;
        private long m_Index= -1;
        private double m_Mid = 0.0;
        private DateTime m_TimeStamp = DateTime.Now;
        private double m_Range = 0.0;
        private double m_TrueRange = 0.0;
        private double m_TrueHigh = 0.0;
        private double m_TrueLow = 0.0;
        private double m_AskVolume = 0.0;
        private double m_BidVolume = 0.0;
        private double m_Volume = 0.0;
        private double m_LastPx = 0.0;
        private double m_LastSize = 0.0;
        private double m_PrevLastPx = 0.0;
        private double m_LastPxUpDownCount = 0.0;
        private bool m_ConditionValue=false;
        private string m_ConditionName="";

        private bool m_LastBar = false;

        KaiTrade.Interfaces.TSItemType m_ItemType = KaiTrade.Interfaces.TSItemType.none;



        KaiTrade.Interfaces.TSItemSourceActionType m_SourceActionType = KaiTrade.Interfaces.TSItemSourceActionType.none;

        /// <summary>
        /// user defined tag
        /// </summary>
        private object m_Tag = null;

        private string externalID = "";

       
        /// <summary>
        /// used by CQG
        /// </summary>
        private bool m_DriverChangedData = false;

        private string m_StrategyID = "";

        /// <summary>
        /// A list of trade signals that may be associated with this TS data item
        /// </summary>
        private Dictionary<string, KaiTrade.Interfaces.ITradeSignal> m_Signals;

        /// <summary>
        /// Used to store curve values from custom studies
        /// </summary>
        private Dictionary<string, double> m_CurveValues;

        /// <summary>
        /// Used to store previous curve values from custom studies
        /// </summary>
        private Dictionary<string, double> m_PrevCurveValues;


        /// <summary>
        /// Number of UD Values we keep
        /// </summary>
        private int m_UDMaxVals = 5;

        /// <summary>
        /// user defined values -
        /// </summary>
        private double[] m_UDValues;

        /// <summary>
        /// Create a logger for use in this Driver
        /// </summary>
        public log4net.ILog m_Log;

        public TSItem()
        {
            // Set up logging - will participate in the standard toolkit log
            m_Log = log4net.LogManager.GetLogger("KaiTrade");

            m_CurveValues = new Dictionary<string, double>();
            m_PrevCurveValues = new Dictionary<string, double>();

            m_UDValues = new double[m_UDMaxVals];

            m_Signals = new Dictionary<string, KaiTrade.Interfaces.ITradeSignal>();
        }

        /// <summary>
        /// A list of trade signals that may be associated with this TS data item
        /// </summary>
        public Dictionary<string, KaiTrade.Interfaces.ITradeSignal> Signals
        {
            get { return m_Signals; }
            set { m_Signals = value; }
        }

        public override string ToString()
        {
            string myRet = "";
            myRet += m_ItemType.ToString() + ",";
            myRet += m_SourceActionType.ToString() + ",";
            myRet += m_Avg.ToString() + ",";
            myRet += m_HLC3.ToString() + ",";
            myRet += m_High.ToString() + ",";
            myRet += m_Low.ToString() + ",";
            myRet += m_Open.ToString() + ",";
            myRet += m_Close.ToString() + ",";
            myRet += m_Index.ToString() + ",";
            myRet += m_Mid.ToString() + ",";
            myRet += m_TimeStamp.ToString() + ",";
            myRet += m_Range.ToString() + ",";
            myRet += m_TrueRange.ToString() + ",";
            myRet += m_TrueHigh.ToString() + ",";
            myRet += m_TrueLow.ToString() + ",";
            myRet += m_AskVolume.ToString() + ",";
            myRet += m_BidVolume.ToString() + ",";
            myRet += m_LastPx.ToString();
            if (m_ConditionName.Length > 0)
            {
                //myRet += "," + m_ConditionName + ",";
                //myRet += m_ConditionValue.ToString();
            }
            if (m_CurveValues != null)
            {
                foreach (string myCurveName in m_CurveValues.Keys)
                {
                    string myTemp = "," + myCurveName + "=" + m_CurveValues[myCurveName].ToString();
                    myRet += myTemp;
                }
            }
            if (m_Tag != null)
            {
                myRet += "," + m_Tag.ToString();
            }
            return myRet;

            //return string.Format("Customer = {0} ID = {1}", Customer, CustomerID);
        }

       

        /// <summary>
        /// Return a string of Tab separated data good for Excel
        /// </summary>
        /// <returns></returns>
        public string ToTabSeparated()
        {
            string myRet = "";
            myRet += m_Mnemonic + "\t";
            myRet += m_Index.ToString() + "\t";
            myRet += m_TimeStamp.ToString() + "\t";
            myRet += m_ItemType.ToString() + "\t";
            myRet += m_SourceActionType.ToString() + "\t";
            myRet += m_Open.ToString() + "\t";
            myRet += m_Close.ToString() + "\t";

            myRet += m_Low.ToString() + "\t";
            myRet += m_Mid.ToString() + "\t";
            myRet += m_High.ToString() + "\t";

            myRet += m_Avg.ToString() + "\t";
            myRet += m_HLC3.ToString() + "\t";

            myRet += m_Range.ToString() + "\t";
            myRet += m_TrueRange.ToString() + "\t";
            myRet += m_TrueHigh.ToString() + "\t";
            myRet += m_TrueLow.ToString() + "\t";
            myRet += m_AskVolume.ToString() + "\t";
            myRet += m_BidVolume.ToString() + "\t";
            myRet += m_LastPx.ToString();
            if (m_ConditionName.Length > 0)
            {
                myRet += "\t" + m_ConditionName + "\t";
                myRet += m_ConditionValue.ToString();
            }
            if (m_CurveValues != null)
            {
                foreach (string myCurveName in m_CurveValues.Keys)
                {
                    string myTemp = "\t" + myCurveName + "=" + m_CurveValues[myCurveName].ToString();
                    myRet += myTemp;
                }
            }
            if (m_StrategyID.Length > 0)
            {
                myRet += m_StrategyID + "\t";
            }
            if (m_Tag != null)
            {
                myRet += "\t" + m_Tag.ToString();
            }
            myRet += "\n";
            return myRet;
        }

        

        #region TSItem Members

        /// <summary>
        /// Store a user defined tag
        /// </summary>
        public object Tag
        {
            get { return m_Tag; }
            set { m_Tag = value; }
        }

        [DataMember]
        [JsonProperty]
        public string ExternalID
        {
            get { return externalID; }
            set { externalID = value; }
        }


        [DataMember]
        [JsonProperty]
        public bool DriverChangedData
        {
            get { return m_DriverChangedData; }
            set { m_DriverChangedData = value; }
        }

        /// <summary>
        /// Defines what ttype of item this is e.g. timed bar, constant volume bar
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public KaiTrade.Interfaces.TSItemType ItemType
        {
            get { return m_ItemType; }
            set { m_ItemType = value; }
        }

        /// <summary>
        /// Defines the source of this item - bar addedm, bar updated, bar deleted
        /// </summary>
        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public KaiTrade.Interfaces.TSItemSourceActionType SourceActionType
        {
            get { return m_SourceActionType; }
            set { m_SourceActionType = value; }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(36)")]
        public string StrategyID
        {
            get { return m_StrategyID; }
            set { m_StrategyID = value; }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(255)")]
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
        [System.Data.Linq.Mapping.Column]
        public double LastPx
        {
            get
            {
                return m_LastPx;
            }
            set
            {
                if (m_LastPx == value)
                {
                    m_LastPxUpDownCount = 0;
                }
                else if (m_LastPx > value)
                {
                    m_LastPxUpDownCount++;
                }
                else
                {
                    m_LastPxUpDownCount--;
                }
                m_PrevLastPx = m_LastPx;

                m_LastPx = value;
            }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public double Volume
        {
            get
            {
                return m_Volume;
            }
            set
            {
                m_Volume = value;
            }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public bool LastBar
        {
            get
            {
                return m_LastBar;
            }
            set
            {
                m_LastBar = value;
            }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public double AskVolume
        {
            get
            {
                return m_AskVolume;
            }
            set
            {
                m_AskVolume = value;
            }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public double BidVolume
        {
            get
            {
                return m_BidVolume;
            }
            set
            {
                m_BidVolume = value;
            }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public double LastSize
        {
            get
            {
                return m_LastSize;
            }
            set
            {
                m_LastSize = value;
            }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public double LastPxPrev
        {
            get
            {
                return m_PrevLastPx;
            }
            set
            {
                m_PrevLastPx = value;
            }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public double UpDownCount
        {
            get
            {
                return m_LastPxUpDownCount;
            }
            set
            {
                m_LastPxUpDownCount = value;
            }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public double Avg
        {
            get
            {
                return m_Avg;
            }
            set
            {
                m_Avg = value;
            }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public double Close
        {
            get
            {
                return m_Close;
            }
            set
            {
                m_Close = value;
            }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public double HLC3
        {
            get
            {
                return m_HLC3;
            }
            set
            {
                m_HLC3 = value;
            }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public double High
        {
            get
            {
                return m_High;
            }
            set
            {
                m_High = value;
            }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public long Index
        {
            get
            {
                return m_Index;
            }
            set
            {
                m_Index = value;
            }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public double Low
        {
            get
            {
                return m_Low;
            }
            set
            {
                m_Low = value;
            }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public double Mid
        {
            get
            {
                return m_Mid;
            }
            set
            {
                m_Mid = value;
            }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public double Open
        {
            get
            {
                return m_Open;
            }
            set
            {
                m_Open = value;
            }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public double Range
        {
            get
            {
                return m_Range;
            }
            set
            {
                m_Range = value;
            }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public DateTime TimeStamp
        {
            get
            {
                return m_TimeStamp;
            }
            set
            {
                m_TimeStamp = value;
            }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public double TrueHigh
        {
            get
            {
                return m_TrueHigh;
            }
            set
            {
                m_TrueHigh = value;
            }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public double TrueLow
        {
            get
            {
                return m_TrueLow;
            }
            set
            {
                m_TrueLow = value;
            }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public double TrueRange
        {
            get
            {
                return m_TrueRange;
            }
            set
            {
                m_TrueRange = value;
            }
        }

        [DataMember]
        [JsonProperty]
        public List<string> CustomValues
        {
            get
            {
                List<string> valuePairs = new List<string>();
                try
                {
                    foreach (string myName in m_CurveValues.Keys)
                    {
                        string valuePair = string.Format("{0}={1}", myName, m_CurveValues[myName]);
                        valuePairs.Add(valuePair);
                    }
                }
                catch
                {
                }
                return valuePairs;
            }
            set
            {
                try
                {
                    foreach (string myPair in value)
                    {
                        string[] pairValues = myPair.Split('=');
                        if (m_CurveValues.ContainsKey(pairValues[0]))
                        {
                            m_CurveValues[pairValues[0]] = double.Parse(pairValues[1]);
                        }
                        else
                        {
                            m_CurveValues.Add(pairValues[0], double.Parse(pairValues[1]));
                        }
                         
                    }
                }
                catch
                {
                }
            }
        }

        public double GetBarValuebyName(string myName)
        {
            double myVal = -1;
            switch (myName.ToUpper())
            {
                case KaiTrade.Interfaces.TSBarField.HLC3:
                    myVal = HLC3;
                    break;
                case KaiTrade.Interfaces.TSBarField.OPEN:
                    myVal = Open;
                    break;
                case KaiTrade.Interfaces.TSBarField.CLOSE:
                    myVal = Close;
                    break;
                case KaiTrade.Interfaces.TSBarField.HIGH:
                    myVal = High;
                    break;
                case KaiTrade.Interfaces.TSBarField.LOW:
                    myVal = Low;
                    break;
                default:

                    break;
            }
            return myVal;
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column(DbType = "NVarChar(255)")]
        public string ConditionName
        {
            get { return m_ConditionName; }
            set { m_ConditionName = value; }
        }

        [DataMember]
        [JsonProperty]
        [System.Data.Linq.Mapping.Column]
        public bool ConditionValue
        {
            get { return m_ConditionValue; }
            set { m_ConditionValue = value; }
        }

        [DataMember]
        [JsonProperty]
        public double[] UDValues
        {
            get { return m_UDValues; }
            set { m_UDValues = value; }
        }



        /// <summary>
        /// Get/Set the max number of UDValues - note that changing this
        /// will erase any existing values;
        /// </summary>
        [DataMember]
        [JsonProperty]
        public int MaxUDValues
        { get { return m_UDMaxVals; } set { m_UDMaxVals = value; } }


        /// <summary>
        /// Get/Set the array of curve values
        /// </summary>
        [DataMember]
        [JsonProperty]
        public KaiTrade.Interfaces.IParameter[] CurveValues
        {
            get
            {
                KaiTrade.Interfaces.IParameter[] parms = new Parameter[m_CurveValues.Count];
                int i = 0;
                foreach (string key in m_CurveValues.Keys)
                {
                    KaiTrade.Interfaces.IParameter p = new K2DataObjects.Parameter(key, m_CurveValues[key].ToString());
                    parms[i] = p;
                }
                return parms;
            }
            set
            {
                m_CurveValues = new Dictionary<string, double>();
                foreach (KaiTrade.Interfaces.IParameter p in value)
                {
                    if (p != null)
                    {
                        m_CurveValues.Add(p.ParameterName, double.Parse(p.ParameterValue));
                    }
                }
            }
        }

        /// <summary>
        /// Get/Set the array of trade signals associated with this TSItem
        /// </summary>
        public KaiTrade.Interfaces.ITradeSignal[] TradeSignals
        { 
            get { return m_Signals.Values.ToArray(); }
            set
            {
                m_Signals = new Dictionary<string, KaiTrade.Interfaces.ITradeSignal>();
                foreach (KaiTrade.Interfaces.ITradeSignal s in value)
                {
                    m_Signals.Add(s.Identity, s);
                }
            }
 
        }


        public void SetCurveValue(string myName, double myValue)
        {
            try
            {
                if (m_CurveValues.ContainsKey(myName))
                {
                    SetPrevCurveValue(myName, m_CurveValues[myName]);
                    m_CurveValues[myName] = myValue;
                }
                else
                {
                    m_CurveValues.Add(myName, myValue);
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("SetCurveValue", myE);
            }
        }

        private void SetPrevCurveValue(string myName, double myValue)
        {
            try
            {
                if (m_PrevCurveValues.ContainsKey(myName))
                {
                    m_PrevCurveValues[myName] = myValue;
                }
                else
                {
                    m_PrevCurveValues.Add(myName, myValue);
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("SetPrevCurveValue", myE);
            }
        }

        public double GetCurveValue(string myName)
        {
            return m_CurveValues[myName];
        }
        public double GetPrevCurveValue(string myName)
        {
            return m_PrevCurveValues[myName];
        }

        public double GetUDCurveValue(int myIndex)
        {
            return m_UDValues[myIndex];
        }
        
        public void SetUDCurveValue(int myIndex, double myValue)
        {
            m_UDValues[myIndex] = myValue;
        }

        /// <summary>
        /// Reset the User  defined values to 0
        /// </summary>
        public void ResetUDValues()
        {
            for (int i = 0; i < m_UDMaxVals; i++)
            {
                    m_UDValues[i] = 0.0;
            }
        }

        public List<string> GetCurveNames()
        {
            List<string> myCurveNames = new List<string>();
            try
            {
                foreach (string myName in m_CurveValues.Keys)
                {
                    myCurveNames.Add(myName);
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("GetCurveNames", myE);
            }
            return myCurveNames;
        }

        #endregion
    }
}
