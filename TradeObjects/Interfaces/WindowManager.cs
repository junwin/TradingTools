using System;
using System.Collections.Generic;
using System.Text;

namespace KaiTrade.Interfaces
{
    /// <summary>
    /// Provides general access to all the windows/views displayed in the system
    /// </summary>
    public interface WindowManager
    {
        /// <summary>
        /// Make some window in the docking manager visible
        /// </summary>
        /// <param name="myTitle"></param>
        void MakeVisible(string myTitle);

        /// <summary>
        /// Get the control in some docking manager content window
        /// used to access user controls displayed by the manager
        /// </summary>
        /// <param name="myTitle"></param>
        /// <returns></returns>
        object GetWindowControl(string myTitle);
    }
}
