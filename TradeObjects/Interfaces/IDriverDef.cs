using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaiTrade.Interfaces
{
    /// <summary>
    /// Provides the data to define a driver/connection we may load
    /// </summary>
    public interface IDriverDef
    {
        string Name { get; set; }
        string Code { get; set; }
        string LoadPath { get; set; }
        bool Enabled { get; set; }
    }
}
