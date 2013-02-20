//-----------------------------------------------------------------------
// <copyright file="Group.cs" company="KaiTrade LLC">
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
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace K2DataObjects
{
    [DataContract]
    [KnownType(typeof(K2DataObjects.Node))]
    public class Group : KaiTrade.Interfaces.IGroup
    {
        private string m_ID = "";
        private string m_Name = "";
        private List<KaiTrade.Interfaces.INode> m_Children;

        public Group()
        {
            m_Children = new List<KaiTrade.Interfaces.INode>();
        }

        [DataMember]
        public string ID
        {
            get { return m_ID; }
            set { m_ID = value; }
        }

        [DataMember]
        public string Name
        {
            get { return m_Name; }
            set{m_Name = value;}
        }

        [DataMember]
        public List<KaiTrade.Interfaces.INode> Children
        {
            get { return m_Children; }
            set { m_Children = value; }
        }

        public void AddChild(KaiTrade.Interfaces.INode child)
        {
        }

        public void Clear()
        {
            m_Children.Clear();
            m_Children = new List<KaiTrade.Interfaces.INode>();
        }
    }

    [DataContract]
    public class Node : KaiTrade.Interfaces.INode
    {
        private string m_ID = "";
        //NOT USED? private string m_Name = "";
        private List<KaiTrade.Interfaces.INode> m_Children;
        private KaiTrade.Interfaces.NodeType m_NodeType = KaiTrade.Interfaces.NodeType.other;

        public Node()
        {
            m_Children = new List<KaiTrade.Interfaces.INode>();
        }

        [DataMember]
        public string ID
        {
            get { return m_ID; }
            set { m_ID = value; }
        }

        [DataMember]
        public string Name
        {
            get;
            set;
        }

        [DataMember]
        public List<KaiTrade.Interfaces.INode> Children
        {
            get { return m_Children; }
            set { m_Children = value; }
        }

        [DataMember]
        public KaiTrade.Interfaces.NodeType NodeType
        {
            get { return m_NodeType; }
            set { m_NodeType = value; }
        }

        public void AddChild(KaiTrade.Interfaces.INode child)
        {
        }

        public void Clear()
        {
            m_Children.Clear();
            m_Children = new List<KaiTrade.Interfaces.INode>();
        }
    }
}
