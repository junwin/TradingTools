//-----------------------------------------------------------------------
// <copyright file="IParameter.cs" company="KaiTrade LLC">
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
    /// Defines the type of paramter - based on FIX ATDL
    /// </summary>
    public enum ATDLType {Int=1, Length, NumInGroup,SeqNum,TagNum,Float,Qty,Price,PriceOffset,Amt,Percentage,Char,Boolean,String,MultipleCharValue,Currency,Exchange,MonthYear,UTCTimestamp,UTCTimeOnly,LocalMktDate,UTCDateOnly,data,MultipleStringValue,Country,Language,TZTimeOnly,TZTimestamp,Tenor};

    /// <summary>
    /// Define how the parameter is used
    /// </summary>
    public enum ParmeterMode { input, output, both, none };

    /// <summary>
    /// This defines a parameter for a strategy or othe algo - it extends existing strategy parameters used
    /// in orders and strategy and allows interoperation using FIX Protocol ADTL
    /// </summary>
    public interface IParameter
    {
        /// <summary>
        /// Name of the parmeter
        /// </summary>
        string ParameterName
        { get; set; }

        /// <summary>
        /// Parameter group that the parameter belongs to
        /// </summary>
        string ParameterGroup
        { get; set; }

        /// <summary>
        /// Type of parameter
        /// </summary>
        KaiTrade.Interfaces.ATDLType ParameterType
        { get; set; }

        /// <summary>
        /// Value of the Paramter as a string
        /// </summary>
        string ParameterValue
        { get; set; }

        /// <summary>
        /// Determines if the paramter is optional
        /// true => optional, false => the parameter is required
        /// </summary>
        bool Optional
        {
            get;
            set;
        }

        /// <summary>
        /// FIXTag if appropriate, else 0
        /// </summary>
        int FIXTag
        { get; set; }
    }
}
