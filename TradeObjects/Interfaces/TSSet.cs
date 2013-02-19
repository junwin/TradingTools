/***************************************************************************
 *
 *      Copyright (c) 2009,2010,2011,2012 KaiTrade LLC (registered in Delaware)
 *                     All Rights Reserved Worldwide
 *
 * STRICTLY PROPRIETARY and CONFIDENTIAL
 *
 * WARNING:  This file is the confidential property of KaiTrade LLC For
 * use only by those with the express written permission and license from
 * KaiTrade LLC.  Unauthorized reproduction, distribution, use or disclosure
 * of this file or any program (or document) is prohibited.
 *
 ***************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace KaiTrade.Interfaces
{
    /// <summary>
    /// Provides a closely coupled update for clients of the set - note that
    /// tssets usually act as a conventional publisher and the regular
    /// publisher function will support wider distribution of data
    /// </summary>
    /// <param name="mySet"></param>
    public delegate void TSUpdate(KaiTrade.Interfaces.TSSet mySet);

    /// <summary>
    /// Provides a closely coupled update for clients of the set - note that
    /// tssets usually act as a conventional publisher and the regular
    /// publisher function will support wider distribution of data
    /// Called when a bar is added to the set
    /// </summary>
    /// <param name="mySet"></param>
    public delegate void TSAdded(KaiTrade.Interfaces.TSSet mySet);

    /// <summary>
    /// Provides a closely coupled update for clients of the set - note that
    /// tssets usually act as a conventional publisher and the regular
    /// publisher function will support wider distribution of data
    /// </summary>
    /// <param name="mySet"></param>
    public delegate void TSStatus(KaiTrade.Interfaces.TSSet mySet);

    /// <summary>
    /// Defines periods used when requesting historic bar data (or conditions)
    /// </summary>
    public enum TSPeriod
    {
        IntraDay, Day, Week, Month, Quarter, SemiYear, Year, hour, minute, two_minute, three_minute, five_minute, thirty_minute, fifteen_minute
    }

    /// <summary>
    /// Defines the range type being used
    /// </summary>
    public enum TSRangeType
    {
        IntInt, DateInt, DateDate
    }

    /// <summary>
    /// the volume type used on constant volume queries
    /// </summary>
    public enum TSVolumeType
    {
        Ticks, Actual
    }

    /// <summary>
    /// Defines the query types available
    /// </summary>
    public enum TSType
    {
        BarData, StudyData, Condition, Expression, ConstantBars, TradeSystem, BarExpressionCombo, BarStudyCombo, BarExpressionStudyCombo, BasicData
    }

    public enum TSSessionFlags
    {
        Undefined, DailySession
    }

    /// <summary>
    /// Defines how study and trade system calcs will be don
    /// </summary>
    public enum TSBarCalculationMode
    {
        beginBar, endBar, tick, endBarPeriodic, periodic, none
    }

    /// <summary>
    /// Set of time based data, bars, expressions, conditions and custom studies and the properties
    /// needed to make request
    /// </summary>
    public interface TSSet
    {
        /// <summary>
        /// KaiTrade unique ID
        /// </summary>
        string Identity
        { get; }

        /// <summary>
        /// Get set name used in publishing
        /// </summary>
        string Name
        { get; set; }

        /// <summary>
        /// client request id that is associated with this set.
        /// </summary>
        string RequestID
        { get; set; }

        /// <summary>
        /// Alias name will be used to publish - in particular for CQG Expressions
        /// </summary>
        string Alias
        { get; set; }

        /// <summary>
        /// Create an empty expression
        /// </summary>
        /// <returns></returns>
        KaiTrade.Interfaces.ITSExpression CreateExpression();

        /// <summary>
        /// Called on Set updates
        /// </summary>
        TSUpdate TSUpdate
        {
            get;
            set;
        }

        /// <summary>
        /// Called when an item(bar) is added
        /// </summary>
        TSAdded TSAdded
        {
            get;
            set;
        }

        /// <summary>
        /// Called when Status changes
        /// </summary>
        TSStatus TSStatus
        {
            get;
            set;
        }

        /// <summary>
        /// ID of the strategy that this set is related to (if any)
        /// </summary>
        string StrategyName
        { get; set; }

        /// <summary>
        /// Used to determine if a named strategy can be breated automatically
        /// or if it must already exist before the TSSet can use it.
        /// </summary>
        bool AutoCreateStrategy
        { get; set; }

        /// <summary>
        /// Publisher used to publising trigger results e.g. a CQG Condition
        /// </summary>
        KaiTrade.Interfaces.IPublisher TriggerPublisher
        { get; set; }

        /// <summary>
        /// Get/Set the type of data requested
        /// </summary>
        TSType TSType
        { get; set; }

        /// <summary>
        /// get/set the session flags
        /// </summary>
        TSSessionFlags TSSessionFlags
        { get; set; }

        /// <summary>
        /// CQG Specific - see CQG for doc
        /// </summary>
        long TSSessionFilter
        { get; set; }

        /// <summary>
        /// Identifier provided by external system if any
        /// </summary>
        string ExternalID
        { get; set; }

        /// <summary>
        /// Reference of an external object e.g. from the providers API - should be only used by the system
        /// </summary>
        object ExternalRef
        { get; set;}

        /// <summary>
        /// user defined tag
        /// </summary>
        object Tag
        { get; set;}

        /// <summary>
        /// Get/Set the price agregator associated with the set if any
        /// </summary>
        //KaiTrade.Interfaces.PriceAgregator PriceAgregator
        //{get; set;}

        /// <summary>
        /// Data items/bars
        /// </summary>
        List<KaiTrade.Interfaces.ITSItem> Items
        { get; }

        /// <summary>
        /// get the current item being updated - the current bar
        /// </summary>
        KaiTrade.Interfaces.ITSItem CurrentItem
        { get;}
        /// <summary>
        /// get the last complete bar - the one before the currenty updating top bar
        /// </summary>
        KaiTrade.Interfaces.ITSItem LastCompleteItem
        { get;}

        /// <summary>
        /// Add a individual bar to the set
        /// </summary>
        /// <param name="myTSItem"></param>
        void AddItem(KaiTrade.Interfaces.ITSItem myTSItem);

        /// <summary>
        /// Replace a bar in the set of bars
        /// </summary>
        /// <param name="myTSItem"></param>
        /// <param name="myIndex"></param>
        void ReplaceItem(KaiTrade.Interfaces.ITSItem myTSItem, int myIndex);

        /// <summary>
        /// get a bar item
        /// </summary>
        /// <param name="myIndex"></param>
        /// <returns></returns>
        KaiTrade.Interfaces.ITSItem GetItem( int myIndex);

        /// <summary>
        /// Get/Set a condition/trigger name
        /// </summary>
        string ConditionName { get; set; }

        /// <summary>
        /// Get set a cutome study name
        /// </summary>
        string StudyName { get; set; }

        /// <summary>
        /// Get/Set the mnemonic that the data will be based on
        /// </summary>
        string Mnemonic
        {get; set;}

        /// <summary>
        /// Additional parameters used with the TSSet, for example for 
        /// a trade system
        /// </summary>
        List<KaiTrade.Interfaces.IParameter> Parameters
        { get; set; }

        /// <summary>
        /// Subscribe to the base mnemonic's price updates - this can be used
        /// in connection with the price agregator to build up a set of bars
        /// </summary>
        void SubscribeBaseMnemonic();

        /// <summary>
        /// Gets a user defined curve current value - note that history may be held in the item set
        /// </summary>
        /// <param name="myIndex"></param>
        /// <returns></returns>
        double GetUDCurveValue(int myIndex);

        /// <summary>
        /// Sets a user defined curve current value
        /// </summary>
        /// <param name="myIndex"></param>
        /// <param name="myValue"></param>
        void SetUDCurveValue(int myIndex, double myValue);

        /// <summary>
        /// Get the name of the user defined data for a given index
        /// </summary>
        /// <param name="myIndex"></param>
        /// <returns></returns>
        string GetUDCurveName(int myIndex);

        /// <summary>
        /// Set the name for a user defined value at an index
        /// </summary>
        /// <param name="myIndex"></param>
        /// <param name="myValue"></param>
        void SetUDCurveName(int myIndex, string myValue);

        /// <summary>
        /// Get the set High - high of all the bars
        /// </summary>
        decimal? SetHigh
        { get; set; }

        /// <summary>
        /// Get set's low  - low value of the bars
        /// </summary>
        decimal? SetLow
        { get; set; }

        /// <summary>
        /// Get the open price for the set
        /// </summary>
        decimal? SetOpen
        { get; set; }

        /// <summary>
        /// Get the close for the set
        /// </summary>
        decimal? SetClose
        { get; set; }

        /// <summary>
        /// Apply a simple price update to the TSSet - if a price aggregator is set then
        /// the set may add bars and call other routines
        /// </summary>
        /// <param name="update"></param>
        void ApplyPriceUpdate(KaiTrade.Interfaces.IPXUpdate update);

        /// <summary>
        /// Get the last update the set had
        /// </summary>
        KaiTrade.Interfaces.IPXUpdate LastUpdate
        { get; }

        /// <summary>
        /// Reset all data - will delete all the sets data
        /// </summary>
        void Reset();

        /// <summary>
        /// Reset the User  defined values to 0
        /// </summary>
        void ResetUDValues();

        /// <summary>
        /// Get the calc/algs needed to process the TSSet expressions - use for
        /// KaiTrade internal expression processing
        /// </summary>
        void ResolveExpressions();

        /// <summary>
        /// Cause all expressions to be evaluated on the current set of data
        /// used by the proccess updating the raw data in the set
        /// </summary>
        void EvaluateExpressions();

        /// <summary>
        /// Get/Set the status of a TSSet stricktly speaking this
        /// is only set by the provider of information e.g. CQG
        /// </summary>
        KaiTrade.Interfaces.Status Status
        { get; set;}

        /// <summary>
        /// Time period of data held
        /// </summary>
        TSPeriod Period
        { get; set; }

        /// <summary>
        /// Type of range
        /// </summary>
        TSRangeType RangeType
        {
            get;
            set;
        }

        /// <summary>
        /// time to start - depends on RangeType
        /// </summary>
        DateTime DateTimeStart
            {get; set;}

        /// <summary>
        /// Time to end- depends on RangeType
        /// </summary>
        DateTime DateTimeEnd
            {get; set;}

        /// <summary>
        /// interval in items start
        /// </summary>
        int IntStart
            {get; set;}

        /// <summary>
        /// Interval in items end
        /// </summary>
        int IntEnd
        { get; set; }

        /// <summary>
        /// type of interval 1sec, 2 sec...
        /// </summary>
        int IntraDayInterval
            {get; set;}

        bool IncludeEnd
            {get; set;}

        /// <summary>
        /// Define how when caluculations are done for Studies and Trade systems
        /// </summary>
        TSBarCalculationMode CalculationMode
        { get; set; }

        /// <summary>
        /// Period in sec for calculations (see CalculationMode)
        /// </summary>
        int CalculationPeriod
        { get; set; }

        /// <summary>
        /// Set to true if updates are required else a snapshot is returned
        /// </summary>
        bool UpdatesEnabled
        { get; set; }

        /// <summary>
        /// Determines if the set has been updated
        /// </summary>
        bool Updated
        {
            get;
            set;
        }

        /// <summary>
        /// arbitary text - info about the request
        /// </summary>
        string Text
        { get; set;}

        bool Changed
        { get; set;}

        bool Added
        { get; set;}

        /// <summary>
        /// Get/Set the volume level for constant volume bars
        /// </summary>
        long VolumeLevel
        { get; set;}

        /// <summary>
        /// Get/Set include flat ticks
        /// </summary>
        bool IncludeFlatTicks
        { get; set;}

        /// <summary>
        /// get/set the volume type used on a constant volume query
        /// </summary>
        TSVolumeType VolumeType
        { get; set;}

        /// <summary>
        /// Time stamp of an update
        /// </summary>
        DateTime TimeStamp
        { get; set;}

        /// <summary>
        /// Set to true if all changes are to be reported rather than just bars added
        /// </summary>
        bool ReportAll
        {
            get;
            set;
        }

        /// <summary>
        /// Determines if the current bar will be used for updates - with CQG the current bar can
        /// have undefined data - e.g. until an intrabar update is received. If this is false then
        /// data in images will be from the last complete bar
        /// </summary>
        bool UseCurrentBar
        { get;set; }

        /// <summary>
        /// Get a new empty time series item
        /// </summary>
        /// <returns></returns>
        KaiTrade.Interfaces.ITSItem GetNewItem();

       

        /// <summary>
        /// Return a string of Tab separated data good for Excel
        /// </summary>
        /// <returns></returns>
        string ToTabSeparated();

        /// <summary>
        /// Get/Set the list of expressions
        /// </summary>
        List<KaiTrade.Interfaces.ITSExpression> Expressions
        { get; set;}

        /// <summary>
        /// get/set the list of pattern matchers
        /// </summary>
        //List<KaiTrade.Interfaces.PatternMatcher> PatternMatchers
        //{ get; set;}

        /// <summary>
        /// Add a pattern matcher to the list of matcher in the set
        /// </summary>
        /// <param name="myMatcher"></param>
        //void AddPatternMatcher(KaiTrade.Interfaces.PatternMatcher myMatcher);

        /// <summary>
        /// Switch the set into test mode - the set will replay all the
        /// data as updates 1 per period entered
        /// </summary>
        /// <param name="myWaitTime">period between updates in ms</param>
        /// <param name="setSize">size of set exposed to clients - must be smaller than the original number of bars</param>
        void StartTestMode(int mySetSize, int myWaitTime);
    }
}
