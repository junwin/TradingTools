using System;
using System.Collections.Generic;
using System.Text;

namespace KaiTrade.Interfaces
{
    /// <summary>
    /// Define some session running in a driver, this can be specific
    /// to the driver e.g. Data, OrderRouting, Prices or some type
    /// of FIX session for FIX based drivers
    /// </summary>
    public interface DriverSession
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
