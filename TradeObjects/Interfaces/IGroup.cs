//-----------------------------------------------------------------------
// <copyright file="IGroup.cs" company="KaiTrade LLC">
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
    public enum NodeType { group, product, order, trade, AllocRpt, TradeCaptureRpt, other }
    /// <summary>
    /// Represents a grouping of aribitary trade objects, products, venues orders etc
    /// </summary>
    public interface IGroup
    {
        /// <summary>
        /// ID of the group
        /// </summary>
        string ID
        {
            get;
            set;
        }

        /// <summary>
        /// Name of the group
        /// </summary>
        string Name
        {
            get;
            set;
        }

        /// <summary>
        /// List of children
        /// </summary>
        List<INode> Children
        {
            get;
            set;
        }

        void AddChild(INode child);

        void Clear();
    }

    public interface INode
    {
        /// <summary>
        /// ID of the object - e.g. a product ID(or Mnemonic)
        /// </summary>
        string ID
        {
            get;
            set;
        }

        /// <summary>
        /// Name of the Node
        /// </summary>
        string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Type of node group, product ..  If other then the group is
        /// userdefined
        /// </summary>
        NodeType NodeType
        {
            get;
            set;
        }
        /// <summary>
        /// List of children
        /// </summary>
        List<INode> Children
        {
            get;
            set;
        }
    }
}
