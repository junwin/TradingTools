using System;
namespace K2ServiceInterface
{
    public interface IFactory
    {
        IFacade Facade { get; set; }
        void ApplyUpdate(KaiTrade.Interfaces.IPXUpdate update);
        IPriceAgregator GetPriceAgregator(string name);
        KaiTrade.Interfaces.IDOM  GetProductDOM(string mnemonic);
        KaiTrade.Interfaces.IDOM GetProductDOM(string mnemonic, decimal startPx);
        KaiTrade.Interfaces.IPublisher GetPXPublisher(string mnmonic);
        object K2ServiceClient { get; set; }
    }
}
