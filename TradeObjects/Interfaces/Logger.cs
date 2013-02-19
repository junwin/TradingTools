using System;
using System.Collections.Generic;
using System.Text;

namespace KaiTrade.Interfaces
{
    /// <summary>
    /// Defines the interface for some generic logger used for system messages
    /// </summary>
    public interface Logger
    {
        /// <summary>
        /// Log a message, will update subscribers of the logger
        /// </summary>
        /// <param name="myMsg"></param>
        void LogMessage(KaiTrade.Interfaces.Message myMsg);

        /// <summary>
        /// Get the current set of logged messages
        /// </summary>
        List<KaiTrade.Interfaces.Message> Messages
        {
            get;
        }
    }

}
