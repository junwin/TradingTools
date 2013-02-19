//-----------------------------------------------------------------------
// <copyright file="ITSItem.cs" company="KaiTrade LLC">
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
    /// Defines the types of TSItem we can process time or constant volume
    /// </summary>
    public enum TSItemType { time, constantVolume,  none };

    /// <summary>
    /// Define the source of a TSItem - bar added, bar updated, bar deleted
    /// </summary>
    public enum TSItemSourceActionType { requestResolved, barAdded, barUpdated, barDeleted, none };

    /// <summary>
    /// Defines an item of data based on some products history e.g
    /// bars, studies, conditions, trade systems - these items contain
    /// data based on some defined slices for example time(sec, min,..)
    /// or constant volumes of trades the type of slices is determined
    /// by the container typically a TSSet
    /// </summary>
    public interface ITSItem
    {
        /// <summary>
        /// Defines what ttype of item this is e.g. timed bar, constant volume bar
        /// </summary>
        TSItemType ItemType { get; set; }

        /// <summary>
        /// Defines the source of this item - bar addedm, bar updated, bar deleted
        /// </summary>
        TSItemSourceActionType SourceActionType {get; set;}

        /// <summary>
        /// index in the set - returned by the broker
        /// </summary>
        long Index { get; set; }

        /// <summary>
        /// The ID of a strategy that this TS Item is associated with, this is used to provide
        /// support for Trading systems, Signals and conditons
        /// </summary>
        string StrategyID { get; set; }

        /// <summary>
        /// time stamp when the item was added or updates
        /// </summary>
        DateTime TimeStamp { get; set; }

        /// <summary>
        /// Product mnemonic for the bar
        /// </summary>
        string Mnemonic { get; set; }

        /// <summary>
        /// bar open
        /// </summary>
        double Open { get; set; }

        /// <summary>
        /// Bar high
        /// </summary>
        double High { get; set; }

        /// <summary>
        /// Bar Low
        /// </summary>
        double Low { get; set; }

        /// <summary>
        /// Bar close
        /// </summary>
        double Close { get; set; }

        /// <summary>
        /// Bar Mid
        /// </summary>
        double Mid { get; set; }

        /// <summary>
        /// Bar HLC3 (High+low+close)/3
        /// </summary>
        double HLC3 { get; set; }

        /// <summary>
        /// max range of bar
        /// </summary>
        double Range { get; set; }

        /// <summary>
        /// Bar average
        /// </summary>
        double Avg { get; set; }

        double TrueHigh { get; set; }
        double TrueLow { get; set; }
         
        double TrueRange { get; set; }

        double LastPx { get; set; }
        double LastSize { get; set; }
        double LastPxPrev { get; set;}


         

        /// <summary>
        /// AskVolume  - see Volume for total
        /// </summary>
        double AskVolume { get; set; }

        /// <summary>
        /// Bid Volume - see Volume for total
        /// </summary>
        double BidVolume { get; set; }

        /// <summary>
        /// Total volume Bid+offer for the bar
        /// </summary>
        double Volume { get; set; }

        /// <summary>
        /// Indicates if this is the last bar of some resultset
        /// </summary>
        bool LastBar { get; set; }

        double GetBarValuebyName(string myName);

        /// <summary>
        /// up or down (-ve) count of the lastpx
        /// </summary>
        double UpDownCount { get; set;}

        /// <summary>
        /// set a condition name specific
        /// </summary>
        string ConditionName { get; set; }

        /// <summary>
        /// get the condition value
        /// </summary>
        bool ConditionValue { get; set;}

        /// <summary>
        /// Get a list of curve names
        /// </summary>
        /// <returns></returns>
        List<string> GetCurveNames();
        void SetCurveValue(string myName, double myValue);
        double GetCurveValue(string myName);
        double GetPrevCurveValue(string myName);

        /// <summary>
        /// Used to return any custom values for the bar as name value pairs
        /// </summary>
        List<string> CustomValues
        { get; set; }


        /// <summary>
        /// get/set the user defined values as a flat array
        /// </summary>
        double[] UDValues
        { get; set; }

        /// <summary>
        /// Get/Set the array of curve values
        /// </summary>
        IParameter[] CurveValues
        { get; set; }

        /// <summary>
        /// Get/Set the array of trade signals associated with this TSItem
        /// </summary>
        ITradeSignal[] TradeSignals
        { get; set; }

        /// <summary>
        /// Get/Set the max number of UDValues - note that changing this
        /// will erase any existing values;
        /// </summary>
        int MaxUDValues
        { get; set; }

        /// <summary>
        /// get an user defined aribitary double value by index
        /// </summary>
        /// <param name="myIndex"></param>
        /// <returns></returns>
        double GetUDCurveValue(int myIndex);
        

        /// <summary>
        /// Set an user defined double value by index
        /// </summary>
        /// <param name="myIndex"></param>
        /// <param name="myValue"></param>
        void SetUDCurveValue(int myIndex, double myValue);

        /// <summary>
        /// A list of trade signals that may be associated with this TS data item
        /// </summary>
        Dictionary<string, KaiTrade.Interfaces.ITradeSignal> Signals
        { get; set;}

        /// <summary>
        /// Reset the User  defined values to 0
        /// </summary>
        void ResetUDValues();

        /// <summary>
        /// Store a user defined tag
        /// </summary>
        object Tag
        {
            get;
            set;
        }

        /// <summary>
        /// has the driver (prices source) changed the data in the item
        /// </summary>
        bool DriverChangedData
        { get; set; }

        /// <summary>
        /// render as XML
        /// </summary>
        /// <returns></returns>
        string AsXML();

        /// <summary>
        /// set state from xml
        /// </summary>
        /// <param name="myDB"></param>
        void FromXML(string xml);

        /// <summary>
        /// Return a string of Tab separated data good for Excel
        /// </summary>
        /// <returns></returns>
        string ToTabSeparated();
    }

    public interface TSPrice
    {
        /// <summary>
        /// Defines what ttype of item this is e.g. timed bar, constant volume bar
        /// </summary>
        TSItemType ItemType { get; set; }

        /// <summary>
        /// Request ID used to request the bar
        /// </summary>
        string RequestID { get; set; }

        /// <summary>
        /// index in the set - returned by the broker
        /// </summary>
        long Index { get; set; }

        /// <summary>
        /// time stamp for the price in ticks
        /// </summary>
        long TimeStamp { get; set; }

        /// <summary>
        /// Product mnemonic for the bar
        /// </summary>
        string Mnemonic { get; set; }

        /// <summary>
        /// bar open
        /// </summary>
        decimal Open { get; set; }

        /// <summary>
        /// Bar high
        /// </summary>
        decimal High { get; set; }

        /// <summary>
        /// Bar Low
        /// </summary>
        decimal Low { get; set; }

        /// <summary>
        /// Bar close
        /// </summary>
        decimal Close { get; set; }

        /// <summary>
        /// Bar Mid
        /// </summary>
        decimal Mid { get; set; }

        /// <summary>
        /// Bar HLC3 (High+low+close)/3
        /// </summary>
        decimal HLC3 { get; set; }

        /// <summary>
        /// max range of bar
        /// </summary>
        decimal Range { get; set; }

        /// <summary>
        /// Bar average
        /// </summary>
        decimal Avg { get; set; }

        decimal TrueHigh { get; set; }
        decimal TrueLow { get; set; }

        decimal TrueRange { get; set; }

        /// <summary>
        /// AskVolume  - see Volume for total
        /// </summary>
        decimal AskVolume { get; set; }

        /// <summary>
        /// Bid Volume - see Volume for total
        /// </summary>
        decimal BidVolume { get; set; }

        /// <summary>
        /// Total volume Bid+offer for the bar
        /// </summary>
        decimal Volume { get; set; }

    }
}
