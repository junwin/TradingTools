//-----------------------------------------------------------------------
// <copyright file="ITSExpression.cs" company="KaiTrade LLC">
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
    /// Defines the format of the expression - Native
    /// </summary>
    public enum TSFormat
    {
        KTA, DriverSpecific
    }

    /// <summary>
    /// Identifies some expression use to get
    /// time series data from a provider
    /// </summary>
    public interface ITSExpression
    {
        /// <summary>
        /// Get/Set the format of this expression
        /// </summary>
        TSFormat Format
        { get; set;}

        /// <summary>
        /// Some expression to be evaluated by the driver or provider
        /// of TS data
        /// </summary>
        string Expression
        { get; set;}

        /// <summary>
        /// Allias used to identify the result of some expression - e.g. for RTD
        /// </summary>
        string Alias
        { get; set; }

        /// <summary>
        /// Name of the Base expression/alg/calc to be used
        /// </summary>
        string BaseExpression
        { get;}

        
    }
}
