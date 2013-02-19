/***************************************************************************
 *
 *      Copyright (c) 2009,2010,2011,2012 KaiTrade LLC (registered in Delaware)
 *                     All Rights Reserved Worldwide
 *
 * STRICTLY PROPRIETARY and CONFIDENTIAL
 *
 * WARNING:  This file is the confidential property of KaiTrade LLC For
 * use only by those with the express written permission and license from
 * KaiTrade LLC.  Unauthorized reproduction, distribution, use or disclosure
 * of this file or any program (or document) is prohibited.
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace KaiTrade.Interfaces
{
    /// <summary>
    /// Defines an interface that any object manageing a set of
    /// products for KaiTrade must impliment
    /// </summary>
    public interface ProductManager
    {
        /// <summary>
        /// Get a list of all the product identifiers
        /// </summary>
        /// <returns></returns>
        List<string> GetProductID();

        /// <summary>
        /// Create a product based on its Mnemonic
        /// </summary>
        /// <param name="myMnemonic"></param>
        /// <returns></returns>
        TradableProduct CreateProduct(string myMnemonic);

        /// <summary>
        /// Create a product based on its security ID
        /// </summary>
        /// <param name="myUserName">users mnemonic - the product is assigned this mnemonic if specified</param>
        /// <param name="myTradeVenue">Venue where the product is traded</param>
        /// <param name="myExchange">Exchange</param>
        /// <param name="mySecID">Product ID</param>
        /// <param name="mySecIDSrc">ID source - type of ID e.g. reuter, bloomberg etc</param>
        /// <returns></returns>
        TradableProduct CreateProductWithSecID(string myUserName, string myTradeVenue, string myExchange, string mySecID, string mySecIDSrc);

        /// <summary>
        /// Make a clone of a product
        /// </summary>
        /// <param name="ID">identity of the product we will clone</param>
        /// <returns></returns>
        TradableProduct CloneProduct(string ID);

        /// <summary>
        /// Register a product with the manager - this will add the product to the manager
        /// if its not already and assign its mnemonic into the mnemonic product map.
        /// Note previously registered products will be replaced
        /// </summary>
        /// <param name="product"></param>
        void RegisterProduct(TradableProduct product);

        /// <summary>
        /// Add a product based on its product data - an exception will be thrown if the product exists
        /// otherwise it return the id of the product added
        /// </summary>
        /// <param name="productData"></param>
        /// <returns>the ID of the product added</returns>
        string AddProduct(ProductData productData);

        /// <summary>
        /// Load a product based on its xml databinding
        /// If the product already exists return the existing product
        /// the existance is based on the KAIID, then on Mnemonic within a trade venue
        /// </summary>
        /// <param name="myProduct">product as a databinding object</param>
        /// <returns></returns>
        TradableProduct LoadProduct(KAI.kaitns.Product myProduct);

        /// <summary>
        /// Create a product based on its symbol
        /// </summary>
        /// <param name="myUserName"></param>
        /// <param name="myTradeVenue"></param>
        /// <param name="myExchange"></param>
        /// <param name="mySymbol"></param>
        /// <returns></returns>
        TradableProduct CreateProductWithSymbol(string myUserName, string myTradeVenue, string myExchange, string mySymbol);

        /// <summary>
        /// Get a product from the manager using its identity
        /// </summary>
        /// <param name="myID"></param>
        /// <returns></returns>
        TradableProduct GetProduct(string myID);

        /// <summary>
        /// Get a product from the manager using the Mnemonic
        /// </summary>
        /// <param name="myMnemonic"></param>
        /// <returns></returns>
        TradableProduct GetProductMnemonic(string myMnemonic);

        /// <summary>
        /// Save products to the file specified
        /// </summary>
        /// <param name="myFilePath"></param>
        void ToFile(string myFilePath);

        /// <summary>
        /// Save products for a particular venue to the file specified
        /// </summary>
        /// <param name="myFilePath"></param>
        void ToFile(string myVenueCode, string myFilePath);

        /// <summary>
        /// Write a file of JSON encoded ProductData
        /// </summary>
        /// <param name="myVenueCode"></param>
        /// <param name="myFilePath"></param>
        void ToFileJSON(string myVenueCode, string myFilePath);

        /// <summary>
        /// Save multi leg products to  the file specified
        /// </summary>
        /// <param name="myFilePath"></param>
        void MLProductsToFile( string myFilePath);

        /// <summary>
        /// Load the manager from a file of data bindings
        /// </summary>
        /// <param name="myFilePath"></param>
        void FromFile(string myFilePath);

        /// <summary>
        /// Load products from a CSV file
        /// </summary>
        /// <param name="filePath"></param>
        void LoadProductsCSV(string filePath);

        /// <summary>
        /// Apply a FIX security defintion message to the products
        /// we manage - this will add or update an existing entry
        /// </summary>
        /// <param name="mySecDef"></param>
        void ApplySecurityDefinition(QuickFix.Message mySecDef);

        /// <summary>
        /// remove the product with the ID specified
        /// </summary>
        /// <param name="myID"></param>
        void RemoveProduct(string myID);

        /// <summary>
        /// Get a list of products for a given by venue and cficode(type)
        /// </summary>
        /// <param name="venueCode"></param>
        /// <param name="exchange"></param>
        /// <param name="CFICode"></param>
        /// <returns></returns>
        List<KaiTrade.Interfaces.TradableProduct> GetProducts(string venueCode, string exchange, string CFICode);
    }
}
