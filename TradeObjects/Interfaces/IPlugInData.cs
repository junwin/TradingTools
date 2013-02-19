//-----------------------------------------------------------------------
// <copyright file="IPlugInData.cs" company="KaiTrade LLC">
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
    /// Defines the data that describes some plugin
    /// </summary>
    public interface IPlugInData
    {
        /// <summary>
        /// Unique identity for this plugin
        /// </summary>
        string Identity
        { get; set; }

        /// <summary>
        /// Path/Url for plugin
        /// </summary>
        string Path
        { get; set; }

        /// <summary>
        /// Is the plugin enabled
        /// </summary>
        bool Enabled
        { get; set; }

        /// <summary>
        /// Name of the plugin
        /// </summary>
        string Name
        { get; set; }

        /// <summary>
        /// PlugIn Vendor
        /// </summary>
        string Vendor
        { get; set; }

        /// <summary>
        /// Determines if assembly info is added to names
        /// </summary>
        bool AddAssemblyInfo
        { get; set; }

        /// <summary>
        /// String based config data
        /// </summary>
        string Config
        { get; set; }
    }
}
