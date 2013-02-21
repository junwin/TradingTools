//-----------------------------------------------------------------------
// <copyright file="IPriceFormatConverter.cs" company="KaiTrade LLC">
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
    /// privides price format conversions
    /// </summary>
    public interface IPriceFormatConverter
    {
        /// <summary>
        /// Identity of the this price formater
        /// </summary>
        string ID
        { get; set; }

        /// <summary>
        /// get the display price in the format specified by the target format
        /// from a decimal
        /// </summary>
        /// <param name="sourcePrice">source price </param>
        /// <param name="targetFormat">required output format </param>
        /// <returns>string in the requested format</returns>
        string GetDisplayPrice(decimal? sourcePrice, KaiTrade.Interfaces.PriceFormat targetFormat);

        /// <summary>
        /// get the display price from a double
        /// </summary>
        /// <param name="sourcePrice"></param>
        /// <returns></returns>
        string GetDisplayPrice(double sourcePrice);
    }
}
