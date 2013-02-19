using System;
using System.Collections.Generic;
using System.Text;

namespace KaiTrade.Interfaces
{
    /// <summary>
    /// Provide an interface for simple transactions, for example
    /// appling an execution report to an order
    /// </summary>
    public interface Transaction
    {
        /// <summary>
        /// Indicates that a set of updates will be applied
        /// </summary>
        void StartUpdate();

        /// <summary>
        /// Indicates that a set of updates have been completed
        /// </summary>
        void EndUpdate();
    }
}
