using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DriverBase
{
    public class PXPublisher : KaiTrade.Interfaces.IPublisher
    {
        public void ApplyUpdate(KaiTrade.Interfaces.IPXUpdate update)
        {
        }
        public void Close()
        {
            throw new NotImplementedException();
        }

        public List<KaiTrade.Interfaces.IField> FieldList
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void OnImage(List<KaiTrade.Interfaces.IField> itemList)
        {
            throw new NotImplementedException();
        }

        public void OnStatusChange(List<KaiTrade.Interfaces.IField> itemList)
        {
            throw new NotImplementedException();
        }

        public void OnUpdate(string mnemonic, KaiTrade.Interfaces.IPXUpdate pxUpdate)
        {
            throw new NotImplementedException();
        }

        public void OnUpdate(string myID, string myValue)
        {
            throw new NotImplementedException();
        }

        public void OnUpdate(List<KaiTrade.Interfaces.IField> itemList)
        {
            throw new NotImplementedException();
        }

        public string Open(string myData)
        {
            throw new NotImplementedException();
        }

        public string PublisherType
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public KaiTrade.Interfaces.Status Status
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void Subscribe(KaiTrade.Interfaces.ISubscriber mySubscriber)
        {
            throw new NotImplementedException();
        }

        public string Tag
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public string TopicID()
        {
            throw new NotImplementedException();
        }

        public string TopicID(string myData)
        {
            throw new NotImplementedException();
        }

        public void UnSubscribe(KaiTrade.Interfaces.ISubscriber mySubscriber)
        {
            throw new NotImplementedException();
        }
    }
}
