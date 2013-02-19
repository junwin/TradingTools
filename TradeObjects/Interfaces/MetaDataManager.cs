using System;
using System.Collections.Generic;
using System.Text;

namespace KaiTrade.Interfaces
{
    /// <summary>
    /// Defines the interface a metadata manager needs to provide
    /// </summary>
    public interface MetaDataManager
    {
        /// <summary>
        /// Get the metadata databinding object
        /// </summary>
        KAI.kaitns.KTAMetaData MetaDataDB
        {
            get;
            set;
        }

        /// <summary>
        /// Load the metadata from a set of XML contained in a file
        /// </summary>
        /// <param name="myPath"></param>
        void LoadMetaData(string myPath);

        /// <summary>
        /// Process a meta data databinding object - this sets up indexes to the
        /// tables(grids) defined in the meta data
        /// </summary>
        /// <param name="myMD">new data binding</param>
        void ProcessMetaData(KAI.kaitns.KTAMetaData myMD);

        /// <summary>
        /// Get a grid using its name
        /// </summary>
        /// <param name="myName"></param>
        /// <returns></returns>
        KAI.kaitns.Grid GetGrid(string myName);
    }
}
