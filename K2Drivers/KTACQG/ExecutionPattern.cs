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
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using CQG;

namespace KTACQG
{
    //{"ExecutionPatternLimit:Object":{"Version:String":"1.1","ExecutionRule:String":"Symmetric","Legs:Object":[{"Version:String":"1.1","Duration:String":"DAY","IgnoredQty:Int32":"0","IsPrimary:Boolean":"true","MinQtyIncrease:Int32":"1","Number:Int32":"1","ProportionAdj:Double":"1","VolumeMultiplier:Double":"1","SecondaryOrdersLimit:Object":{"Version:String":"1.1"}},{"Version:String":"1.1","Duration:String":"DAY","IgnoredQty:Int32":"0","IsPrimary:Boolean":"false","MinQtyIncrease:Int32":"1","Number:Int32":"2","ProportionAdj:Double":"1","VolumeMultiplier:Double":"1","SecondaryOrdersLimit:Object":{"Version:String":"1.1"}}]}}

    [JsonObject(MemberSerialization.OptIn)]
    public class SecondaryOrdersLimit
    {
        private string m_Version;

        [JsonProperty]
        public string Version
        {
            get { return m_Version; }
            set { m_Version = value; }
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class Legs
    {
        private string m_Version;
        [JsonProperty]
        public string Version
        {
            get { return m_Version; }
            set { m_Version = value; }
        }
        private string m_Duration;

        [JsonProperty]
        public string Duration
        {
            get { return m_Duration; }
            set { m_Duration = value; }
        }
        private Int32 m_IgnoredQty;

        [JsonProperty]
        public Int32 IgnoredQty
        {
            get { return m_IgnoredQty; }
            set { m_IgnoredQty = value; }
        }
        private bool m_IsPrimary;

        [JsonProperty]
        public bool IsPrimary
        {
            get { return m_IsPrimary; }
            set { m_IsPrimary = value; }
        }
        private Int32 m_MinQtyIncrease;

        [JsonProperty]
        public Int32 MinQtyIncrease
        {
            get { return m_MinQtyIncrease; }
            set { m_MinQtyIncrease = value; }
        }
        private Int32 m_Number;

        [JsonProperty]
        public Int32 Number
        {
            get { return m_Number; }
            set { m_Number = value; }
        }
        private double m_ProportionAdj;

        [JsonProperty]
        public double ProportionAdj
        {
            get { return m_ProportionAdj; }
            set { m_ProportionAdj = value; }
        }
        private double m_VolumeMultiplier;

        [JsonProperty]
        public double VolumeMultiplier
        {
            get { return m_VolumeMultiplier; }
            set { m_VolumeMultiplier = value; }
        }
        private SecondaryOrdersLimit m_SecondaryOrdersLimit;

        [JsonProperty]
        public SecondaryOrdersLimit SecondaryOrdersLimit
        {
            get { return m_SecondaryOrdersLimit; }
            set { m_SecondaryOrdersLimit = value; }
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class ExecutionPatternLimit 
    {
        private string m_Version;
        [JsonProperty]
        public string Version
        {
            get { return m_Version; }
            set { m_Version = value; }
        }

        private string m_ExecutionRule;
        [JsonProperty]
        public string ExecutionRule
        {
            get { return m_ExecutionRule; }
            set { m_ExecutionRule = value; }
        }

        private List<Legs> m_Legs;
        [JsonProperty]
        public List<Legs> Legs
        {
            get { return m_Legs; }
            set { m_Legs = value; }
        }
    }

    public class Root
    {
        private ExecutionPatternLimit m_ExecutionPatternLimit;

        public ExecutionPatternLimit ExecutionPatternLimit
        {
            get { return m_ExecutionPatternLimit; }
            set { m_ExecutionPatternLimit = value; }
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class ExecPatternString
    {
        private string m_Name;        
        private string m_OrderType;
        private string m_PatternString;

        [JsonProperty]
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        [JsonProperty]
        public string OrderType
        {
            get { return m_OrderType; }
            set { m_OrderType = value; }
        }

        [JsonProperty]
        public string PatternString
        {
            get { return m_PatternString; }
            set { m_PatternString = value; }
        }

        
    }

   

}
