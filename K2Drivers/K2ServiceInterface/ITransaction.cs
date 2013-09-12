using System;
using System.Collections.Generic;
using System.Text;

namespace K2ServiceInterface
{
    /// <summary>
    /// Provide an interface for simple transactions, for example
    /// appling an execution report to an order
    /// </summary>
    public interface ITransaction
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
