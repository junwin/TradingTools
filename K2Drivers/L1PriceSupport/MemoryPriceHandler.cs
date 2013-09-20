using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using K2ServiceInterface;
using KaiTrade.Interfaces;

namespace L1PriceSupport
{
    public class MemoryPriceHandler : IPriceHandler
    {
        Dictionary<string, KaiTrade.Interfaces.IDOM> _mnemonicDOM;
        Dictionary<string, L1PriceSupport.PXPublisher> _L1Publisher;
        Dictionary<string, List<IPriceAgregator>> _priceAgregators;
       
        public log4net.ILog _log = log4net.LogManager.GetLogger("KaiTrade");
        public MemoryPriceHandler()
        {
            _mnemonicDOM = new Dictionary<string, IDOM>();
            _L1Publisher = new Dictionary<string, PXPublisher>();
            _priceAgregators = new Dictionary<string, List<IPriceAgregator>>();

        }

        public void ApplyPriceUpdate(KaiTrade.Interfaces.IPXUpdate update)
        {
            try
            {
                
                // try to get a L1PriceSupport.PXPublisher
                //L1PriceSupport.PXPublisher myL1PriceSupport.PXPublisher = m_L1Publisher[myMnemonic] as L1PriceSupport.PXPublisher;
                if (!_L1Publisher.ContainsKey(update.Mnemonic))
                {
                    return;
                }

                
                    try
                    {
                        _L1Publisher[update.Mnemonic].OnUpdate(update.Mnemonic, update);
                    }
                    catch
                    {
                        // silent because we dont want some publisher error stopping
                        // the rest of the update
                    }
                



                KaiTrade.Interfaces.IPublisher myPublisher = _L1Publisher[update.Mnemonic] as KaiTrade.Interfaces.IPublisher;

                if (myPublisher != null)
                {

                    L1PriceSupport.PXPublisher myPXPublisher = myPublisher as L1PriceSupport.PXPublisher;

                    if (myPXPublisher != null)
                    {
                        myPXPublisher.ApplyUpdate(update);

                        myPublisher.OnUpdate(null);
                        if (myPXPublisher.Status != KaiTrade.Interfaces.Status.open)
                        {
                            myPXPublisher.Status = KaiTrade.Interfaces.Status.open;
                        }

                    }
                    else
                    {

                        // to do prices in the new way we expect that the pub impliments the price update interface
                        KaiTrade.Interfaces.IPXUpdate target = myPublisher as KaiTrade.Interfaces.IPXUpdate;
                        if (target != null)
                        {
                            // this will apply the update to the publisher
                            target.From(update);
                        }
                    }
                }

                if (_priceAgregators.ContainsKey(update.Mnemonic))
                {
                    foreach (IPriceAgregator pa in _priceAgregators[update.Mnemonic])
                    {
                        pa.ProcessPrice(update);
                    }
                }
            }
            catch (Exception myE)
            {
                _log.Error("DoApplyPriceUpdate", myE);
            }
        }

        public KaiTrade.Interfaces.IPublisher CreatePxPub(KaiTrade.Interfaces.IProduct product)
        {
            L1PriceSupport.PXPublisher pxPub = null;
            if (!_L1Publisher.ContainsKey(product.Mnemonic))
            {
                pxPub = new L1PriceSupport.PXPublisher();
                pxPub.Product = product;
                _L1Publisher.Add(product.Mnemonic, pxPub);
            }
            return pxPub;
        }

        public void ApplyDOMUpdate(IProduct product, KaiTrade.Interfaces.IDOMSlot[] slots)
        {
            try
            {
                if (slots.Length > 0)
                {
                    KaiTrade.Interfaces.IDOM DOM = GetProductDOM(product, slots[0].Price);

                    DOM.Update(slots);

                }
            }
            catch (Exception myE)
            {
            }
        }

        public void ApplyDOMUpdate(IProduct product, KaiTrade.Interfaces.IPXUpdate update)
        {
            try
            {

                KaiTrade.Interfaces.IDOM dom = _mnemonicDOM[product.Mnemonic];
                if (dom == null)
                {
                    // must have a least a price
                    decimal? initPx = null;
                    if (update.BidPrice.HasValue)
                    {
                        initPx = update.BidPrice;
                    }
                    else if (update.OfferPrice.HasValue)
                    {
                        initPx = update.OfferPrice;
                    }
                    else if (update.TradePrice.HasValue)
                    {
                        initPx = update.TradePrice;
                    }
                    if (initPx.HasValue)
                    {
                        dom = GetProductDOM(product, initPx.Value);

                    }
                }
                dom.Update(update);

            }
            catch (Exception myE)
            {
            }
        }


        public KaiTrade.Interfaces.IPublisher GetPXPublisher(KaiTrade.Interfaces.IProduct product)
        {
            if (!_L1Publisher.ContainsKey(product.Mnemonic))
            {
                _L1Publisher.Add(product.Mnemonic, new L1PriceSupport.PXPublisher());

            }
            return _L1Publisher[product.Mnemonic];
        }

        public KaiTrade.Interfaces.IDOM GetProductDOM(IProduct product, decimal startPx)
        {
            if (!_mnemonicDOM.ContainsKey(product.Mnemonic))
            {
                K2Depth.K2DOM dom = new K2Depth.K2DOM();
                dom.Create(startPx, (decimal)200, product.TickSize.Value);
                _mnemonicDOM.Add(product.Mnemonic, dom);

            }
            return _mnemonicDOM[product.Mnemonic];

        }
        public KaiTrade.Interfaces.IDOM GetProductDOM(IProduct product)
        {
            if (_mnemonicDOM.ContainsKey(product.Mnemonic))
            {
                return _mnemonicDOM[product.Mnemonic];

            }
            return null;

        }

    }
}
