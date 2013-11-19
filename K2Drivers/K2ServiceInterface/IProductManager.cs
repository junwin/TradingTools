using System;
namespace K2ServiceInterface
{
    public interface IProductManager
    {
        ProductUpdate  OnProductUpdate{get; set;}
        string AddProduct(KaiTrade.Interfaces.IProduct product);
        string AddProductX(KaiTrade.Interfaces.IProduct productData);
        KaiTrade.Interfaces.IProduct CloneProduct(string ID);
        KaiTrade.Interfaces.IProduct CreateProduct(string myMnemonic);
        KaiTrade.Interfaces.IProduct CreateProductWithSecID(string myUserName, string myTradeVenue, string myExchange, string mySecID, string mySecIDSrc);
        KaiTrade.Interfaces.IProduct CreateProductWithSymbol(string myUserName, string myTradeVenue, string myExchange, string mySymbol);
        void FromFile(string myFilePath);
        KaiTrade.Interfaces.IProduct GetProduct(string myID);
        System.Collections.Generic.List<string> GetProductID();
        KaiTrade.Interfaces.IProduct GetProductMnemonic(string myMnemonic);
        System.Collections.Generic.List<KaiTrade.Interfaces.IProduct> GetProducts(string venueCode, string exchange, string CFICode);
        KaiTrade.Interfaces.IProduct LoadProduct(string JSONData);
        void LoadProductsCSV(string filePath);
        void RegisterProduct(KaiTrade.Interfaces.IProduct product);
        void RemoveProduct(string myID);
        void ToFile(string myVenueCode, string myFilePath);
        void ToFileJSON(string myVenueCode, string myFilePath);
    }
}
