using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using K2DataObjects;
using Newtonsoft.Json;

namespace RMQDBUpdater
{
    public class RMQMessageListener
    {
        private DBUpdater dbUpdater;
        RabbitMQPublisher.RMQListner listner = null;

        

        public RMQMessageListener()
        {
            dbUpdater = new DBUpdater();
            
        }

        public void Start()
        {
            listner = new RabbitMQPublisher.RMQListner();
            RabbitMQPublisher.RMQFactory.Instance().HostName = "10.1.11.37";
            listner.SubscribeInfo();
            listner.OnRMQMessage += OnRMQMessage;
            listner.SubscribeProductsRMQ("");
            listner.SubscribeAccountsRMQ("");
            listner.SubscribeProductsRMQ("");
            listner.SubscribeTSBarsRMQ("*");
        }
        public void OnRMQMessage(string propType, string messageData)
        {
            switch (propType)
            {
                case KaiTrade.Interfaces.MQType.ACCOUNT:
                    Account account = JsonConvert.DeserializeObject<Account>(messageData);
                    dbUpdater.Update(account);
                    break;
                case KaiTrade.Interfaces.MQType.PRODUCT:
                    Product product = JsonConvert.DeserializeObject<Product>(messageData);
                    dbUpdater.Update(product);
                    break;
                case KaiTrade.Interfaces.MQType.TSBAR:
                    TSItem[] bar = JsonConvert.DeserializeObject<TSItem[]>(messageData);
                    dbUpdater.Update(bar);
                    break;

            }
        }
    }
}
