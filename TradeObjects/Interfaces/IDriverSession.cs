//-----------------------------------------------------------------------
// <copyright file="IDriverSession.cs" company="KaiTrade LLC">
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

namespace KaiTrade.Interfaces
{
    /// <summary>
    /// Define some session running in a driver, this can be specific
    /// to the driver e.g. Data, OrderRouting, Prices or some type
    /// of FIX session for FIX based drivers
    /// </summary>
    public interface IDriverSession
    {
        /// <summary>
        /// Get the key for the session
        /// </summary>
        string Key
        {
            get;
        }
        /// <summary>
        /// The driver ID that the session is for
        /// can be used to access the driver in its manager
        /// </summary>
        string DriverID
        {
            get;
        }

        /// <summary>
        /// The driver code(human readable) - this is for convenience
        /// </summary>
        string DriverCode
        {
            get;
        }

        /// <summary>
        /// Get the session name
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// get the version(or FIX begin string) of the session
        /// </summary>
        string Version
        {
            get;
        }

        /// <summary>
        /// Get the sender ID is any
        /// </summary>
        string SID
        {
            get;
        }

        /// <summary>
        /// Get the target ID if any
        /// </summary>
        string TID
        {
            get;
        }

        /// <summary>
        ///  get the session status
        /// </summary>
        Status Status
        {
            get;
            set;
        }

        /// <summary>
        /// Get any session status text
        /// </summary>
        string StatusText
        {
            get;
            set;
        }

        /// <summary>
        /// write session onto an XML data bining
        /// </summary>
        /// <returns></returns>
        KAI.kaitns.DriverSession ToXMLDB();
    }
}
