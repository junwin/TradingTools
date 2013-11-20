using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using KaiTrade.Interfaces;
using K2DataObjects;

namespace RMQDBUpdater
{
    public class DBUpdater
    {

        public void Update(Account account)
        {
            K2DS.K2AccountDS accountDS = new K2DS.K2AccountDS();
            accountDS.Insert(account, true);
        }

        public void Update(Product product)
        {
            K2DS.K2ProductDS productDS = new K2DS.K2ProductDS();
            productDS.Insert(product);

        }

        public void Update(TSItem[] bar)
        {
            K2DS.K2BarData barDS = new K2DS.K2BarData();           
            foreach (var b in bar)
            {
                PriceBar priceBar = BarDataTools.BarDataUtils.GetPriceBar(b);
                barDS.Insert(priceBar, true);
            }
        }

    }
}
