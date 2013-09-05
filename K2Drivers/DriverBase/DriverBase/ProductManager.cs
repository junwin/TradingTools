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
using System.IO;

using System.Net;
using Newtonsoft.Json;



namespace DriverBase
{
    public class ProductManager 
    {
        /// <summary>
        /// Singleton instance of the AdapterManager class
        /// </summary>
        private static volatile ProductManager s_instance;

        /// <summary>
        /// Object we use to lock during singleton instantiation.
        /// </summary>
        private static object s_Token = new object();

        // Create a logger for use in this class
        public new log4net.ILog m_Log;

        /// <summary>
        /// Products mapped by thier identity
        /// </summary>
        private System.Collections.Hashtable m_Products;
        //private Dictionary<string, KaiTrade.Interfaces.IProduct> m_Products;
        /// <summary>
        /// Map of mnemonics to a product
        /// </summary>
        private System.Collections.Hashtable m_MnemonicProduct;
        //private Dictionary<string, KaiTrade.Interfaces.IProduct> m_MnemonicProduct;

        private string m_Name = "";

        private void DoUpdate(string s1, string s2)
        {
        }

        protected ProductManager()
		{
            //m_Products = new Dictionary<string, KaiTrade.Interfaces.IProduct>();
            //m_MnemonicProduct = new Dictionary<string, KaiTrade.Interfaces.IProduct>();
            m_Products = new System.Collections.Hashtable();
            m_MnemonicProduct = new System.Collections.Hashtable();
            m_Name = "KTAProductManager";
            m_Log = log4net.LogManager.GetLogger("Kaitrade");
            m_Log.Info("ProductManager Created");
		}

        public static ProductManager Instance()
		{	
            if (s_instance == null)
            {
                lock (s_Token)
                {
                    if (s_instance == null)
                    {
                        s_instance = new ProductManager();
                    }
                }
            }
			return s_instance;
		}

        

        #region ProductManager Members

        public KaiTrade.Interfaces.IProduct CreateProduct(string myMnemonic)
        {
            KaiTrade.Interfaces.IProduct myProduct = new K2DataObjects.Product();
            try
            {
                m_Products.Add(myProduct.Identity, myProduct);
                myProduct.Mnemonic = myMnemonic;
                m_MnemonicProduct.Add(myMnemonic, myProduct);
                DoUpdate("CREATE", myProduct.Identity);
            }
            catch (Exception myE)
            {
                m_Log.Error("CreateProduct", myE);
            }
            return myProduct;
        }

        /// <summary>
        /// Create a product based on its security ID
        /// </summary>
        /// <param name="myUserName">users mnemonic - the product is assigned this mnemonic if specified</param>
        /// <param name="myTradeVenue">Venue where the product is traded</param>
        /// <param name="myExchange">Exchange</param>
        /// <param name="mySecID">Product ID</param>
        /// <param name="mySecIDSrc">ID source - type of ID e.g. reuter, bloomberg etc</param>
        /// <returns></returns>
        public KaiTrade.Interfaces.IProduct CreateProductWithSecID(string myUserName, string myTradeVenue, string myExchange, string mySecID, string mySecIDSrc)
        {
            KaiTrade.Interfaces.IProduct myProduct = new K2DataObjects.Product();
            try
            {
                myProduct.TradeVenue = myTradeVenue;
                myProduct.Exchange = myExchange;
                myProduct.SecurityID =mySecID;
                myProduct.IDSource = mySecIDSrc;

                string myMnemonic = myProduct.Mnemonic;
                // if they specify a user name for the product it overrides the mnemonic generated
                if (myUserName.Length > 0)
                {
                    myMnemonic = myUserName;
                }
                m_Products.Add(myProduct.Identity, myProduct);
                myProduct.Mnemonic = myMnemonic;
                m_MnemonicProduct.Add(myMnemonic, myProduct);
                DoUpdate("CREATE", myProduct.Identity);
            }
            catch (Exception myE)
            {
                m_Log.Error("CreateProduct", myE);
            }
            return myProduct;
        }

        /// <summary>
        /// Create a product based on its symbol
        /// </summary>
        /// <param name="myUserName"></param>
        /// <param name="myTradeVenue"></param>
        /// <param name="myExchange"></param>
        /// <param name="mySymbol"></param>
        /// <returns></returns>
        public KaiTrade.Interfaces.IProduct CreateProductWithSymbol(string myUserName, string myTradeVenue, string myExchange, string mySymbol)
        {
            KaiTrade.Interfaces.IProduct myProduct = new K2DataObjects.Product();
            try
            {
                myProduct.TradeVenue = myTradeVenue;
                myProduct.Exchange = myExchange;
                myProduct.Symbol = mySymbol;

                string myMnemonic = myProduct.Mnemonic;
                // if they specify a user name for the product it overrides the mnemonic generated
                if (myUserName.Length > 0)
                {
                    myMnemonic = myUserName;
                }
                m_Products.Add(myProduct.Identity, myProduct);
                myProduct.Mnemonic = myMnemonic;
                m_MnemonicProduct.Add(myMnemonic, myProduct);
                DoUpdate("CREATE", myProduct.Identity);
            }
            catch (Exception myE)
            {
                m_Log.Error("CreateProduct", myE);
            }
            return myProduct;
        }

        /// <summary>
        /// Make a clone of a product
        /// </summary>
        /// <param name="ID">identity of the product we will clone</param>
        /// <returns></returns>
        public KaiTrade.Interfaces.IProduct CloneProduct(string ID)
        {
            KaiTrade.Interfaces.IProduct newProduct = null;
            try
            {
                KaiTrade.Interfaces.IProduct srcProduct = this.GetProduct(ID);
                if (srcProduct != null)
                {
                    newProduct = new K2DataObjects.Product();
                    string newID = newProduct.Identity;
                    newProduct.From(srcProduct);
                    newProduct.Identity = newID;
                }
                DoUpdate("CREATE", newProduct.Identity);
            }
            catch (Exception myE)
            {
                m_Log.Error("CreateProduct", myE);
            }
            return newProduct;
        }

        /// <summary>
        /// Register a product with the manager - this will add the product to the manager
        /// if its not already and assign its mnemonic into the mnemonic product map
        /// </summary>
        /// <param name="product"></param>
        public void RegisterProduct(KaiTrade.Interfaces.IProduct product)
        {
            try
            {
                if (m_Products.ContainsKey(product.Identity))
                {
                    m_Products[product.Identity]=product ;
                }
                else
                {
                    m_Products.Add(product.Identity, product);
                }

                if (m_MnemonicProduct.ContainsKey(product.Mnemonic))
                {
                    m_MnemonicProduct[product.Mnemonic] = product;
                }
                else
                {
                    m_MnemonicProduct.Add(product.Mnemonic, product);
                }
                DoUpdate("REGISTER", product.Identity);
            }
            catch (Exception myE)
            {
                m_Log.Error("RegisterProduct", myE);
            }
        }

        /// <summary>
        /// Add a product based on its product data - an exception will be thrown if the product exists
        /// otherwise it return the product added
        /// </summary>
        /// <param name="productData"></param>
        /// <returns></returns>
        public string AddProductX(KaiTrade.Interfaces.IProduct productData)
        {
            try
            {
                // is the product already there?
                if (m_MnemonicProduct.ContainsKey(productData.Mnemonic))
                {
                    throw new Exception("product already exists");
                }

                KaiTrade.Interfaces.IProduct product = new K2DataObjects.Product();
                productData.To(product);
                m_Products.Add(product.Identity, product);
                m_MnemonicProduct.Add(productData.Mnemonic, product);
                DoUpdate("ADD", product.Identity);
                return product.Identity;
            }
            catch(Exception)
            {
            }
            return "";
        }

        /// <summary>
        /// Load a product based on its xml databinding
        /// If the product already exists return the existing product
        /// </summary>
        /// <param name="myProduct">product as a databinding object</param>
        /// <returns></returns>
        public KaiTrade.Interfaces.IProduct LoadProduct(string JSONData)
        {
            KaiTrade.Interfaces.IProduct myProduct = null;
            try
            {
                KaiTrade.Interfaces.IProduct inProduct = JsonConvert.DeserializeObject<K2DataObjects.Product>(JSONData);
                // if the product ID already exists use the existing product without change
                
                    if (m_Products.ContainsKey(inProduct.Identity))
                    {
                        myProduct = m_Products[inProduct.Identity] as KaiTrade.Interfaces.IProduct;
                        return myProduct;
                    }
                

                // If the menmonic exists and its for the same trade venue use that
                if (m_MnemonicProduct.ContainsKey(inProduct.Mnemonic))
                {
                    KaiTrade.Interfaces.IProduct myTemp = m_MnemonicProduct[inProduct.Mnemonic] as KaiTrade.Interfaces.IProduct;
                    if (inProduct.TradeVenue == myTemp.TradeVenue)
                    {
                        myProduct = myTemp;
                    }
                }

                // If the product does not exist create it
                if (myProduct == null)
                {
                    myProduct = CreateProduct(inProduct.Mnemonic);
                    // will load any underlyings
                    myProduct.From(inProduct);
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("LoadProduct", myE);
            }
            return myProduct;
        }

        public  string AddProduct(KaiTrade.Interfaces.IProduct product)
        {
            try
            {
                KaiTrade.Interfaces.IProduct myProd;

                if (m_MnemonicProduct.ContainsKey(product.Mnemonic))
                {
                    myProd = GetProductMnemonic(product.Mnemonic);
                }
                else
                {
                    myProd = CreateProduct(product.Mnemonic);
                }
                // we dont load the underlyings here since we expect they have been perisisted in
                // the file save
                myProd.From(product);
                try
                {
                    // If the driver supports this - then calling request product details
                    // will get the driver to fill in additional information
                    // regarding the product
                    Factory.Instance().AppFacade.RequestProductDetails(myProd);
                }
                catch (Exception myE)
                {
                    m_Log.Error("FromFile:RequestProductDetails", myE);
                }
                DoUpdate("ADD", myProd.Identity);
                return myProd.Identity;

            }
            catch (Exception myE)
            {
                m_Log.Error("AddProduct", myE);
            }
            return "";
        }

        /// <summary>
        /// Load products from a CSV file (Venue,Mnemonic,CFI,Exch,Symbol,SecID,MMY,Strk,Curr,TickSize,LotSize,TickValue,LongDescName)
        /// </summary>
        /// <param name="filePath"></param>
        public void LoadProductsCSV(string filePath)
        {
            try
            {
                FileStream prodFile;
                StreamReader prodReader;
                prodFile = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                prodReader = new StreamReader(prodFile);
                string myLine = "";
                KaiTrade.Interfaces.IProduct product = null;
                while (!prodReader.EndOfStream)
                {
                    try
                    {
                        myLine = prodReader.ReadLine();
                        string[] prodData = myLine.Split(',');
                        if (prodData.Length > 8)
                        {
                            if (m_MnemonicProduct.ContainsKey(prodData[1]))
                            {
                                product = m_MnemonicProduct[prodData[1]] as KaiTrade.Interfaces.IProduct;
                            }
                            else
                            {
                                product = new K2DataObjects.Product();
                            }

                            product.TradeVenue = prodData[0];
                            product.Exchange = prodData[3];
                            product.CFICode = prodData[2];
                            product.SecurityID = prodData[5];
                            product.IDSource = prodData[0];
                            product.Mnemonic = prodData[1];
                            product.MMY = prodData[6];
                            if (prodData[7].Length > 0)
                            {
                                product.StrikePrice = decimal.Parse(prodData[7]);
                            }
                            product.Currency = prodData[8];

                            m_Products.Add(product.Identity, product);

                            m_MnemonicProduct.Add(product.Mnemonic, product);

                            if (product != null)
                            {
                                if (prodData.Length > 9)
                                {
                                    product.TickSize = decimal.Parse(prodData[9]);
                                }
                                if (prodData.Length > 10)
                                {
                                    product.PriceFeedQuantityMultiplier = int.Parse(prodData[10]);
                                }
                                if (prodData.Length > 11)
                                {
                                    product.TickValue = decimal.Parse(prodData[11]);
                                }
                                if (prodData.Length > 12)
                                {
                                    product.LongName = prodData[12];
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                //DoUpdate("CREATE", product.Identity);
            }
            catch (Exception myE)
            {
                m_Log.Error("LoadProductsCSV", myE);
            }
        }

        private string GetWebFileAsString(string url)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            // you may need these next two lines to prevent a .net bug
            // System.IO.IOException : Unable to read data from the transport connection: The connection was closed.
            // see http://support.microsoft.com/default.aspx?scid=kb;EN-US;915599
            httpWebRequest.KeepAlive = false;
            httpWebRequest.ProtocolVersion = HttpVersion.Version10;

            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            Stream stream = httpWebResponse.GetResponseStream();
            StreamReader streamReader = new StreamReader(stream, Encoding.ASCII);
            return streamReader.ReadToEnd();


        }

        public void FromFile(string myFilePath)
        {
            try
            {
                KaiTrade.Interfaces.IProduct[] products = null;
                if (myFilePath.ToLower().StartsWith("http"))
                {
                    string productData = GetWebFileAsString(myFilePath);
                    products = JsonConvert.DeserializeObject<K2DataObjects.Product[]>(productData);
                }
                else
                {
                    FileStream file = null;
                    StreamReader reader = null;
                    try
                    {


                        file = new FileStream(myFilePath, FileMode.Open, FileAccess.Read);
                        reader = new StreamReader(file);
                        products = JsonConvert.DeserializeObject<K2DataObjects.Product[]>(reader.ReadToEnd());
                    }
                    catch
                    {

                    }
                    finally
                    {
                        if (reader != null)
                        {
                            reader.Close();
                            reader = null;
                        }
                        if (file != null)
                        {
                            file.Close();
                            file = null;
                        }
                    }
                }
                //NOT USED? KaiTrade.Interfaces.IProduct myProd;

                foreach (K2DataObjects.Product prod  in products)
                {
                    AddProduct(prod as KaiTrade.Interfaces.IProduct);
                }
                DoUpdate("LOAD", myFilePath);
            }
            catch (Exception myE)
            {
                m_Log.Error("ProductManager.FromFile", myE);
            }
        }

        public KaiTrade.Interfaces.IProduct GetProduct(string myID)
        {
            KaiTrade.Interfaces.IProduct myProdIF = m_Products[myID] as KaiTrade.Interfaces.IProduct;
            return myProdIF;
        }

        public List<string> GetProductID()
        {
            List<string> myID = new List<string>();
            foreach (KaiTrade.Interfaces.IProduct myProdIF in m_Products.Values)
            {
                myID.Add(myProdIF.Identity);
            }
            return myID;
        }

        /// <summary>
        /// Get a product using its mnemonic
        /// </summary>
        /// <param name="myMnemonic"></param>
        /// <returns></returns>
        public KaiTrade.Interfaces.IProduct GetProductMnemonic(string myMnemonic)
        {
            KaiTrade.Interfaces.IProduct myProdIF = m_MnemonicProduct[myMnemonic] as KaiTrade.Interfaces.IProduct;
            return myProdIF;
        }
        /// <summary>
        /// Save products for a particular venue to the file specified
        /// </summary>
        /// <param name="myFilePath"></param>
        public void ToFile(string myVenueCode, string myFilePath)
        {
            try
            {
                ToFileJSON(myVenueCode, myFilePath);
                //genDemoFile();

                
            }
            catch (Exception myE)
            {
                m_Log.Error("ProductManager.ToFile", myE);
            }
        }
        /// <summary>
        /// Write a file of JSON encoded ProductData
        /// </summary>
        /// <param name="myVenueCode"></param>
        /// <param name="myFilePath"></param>
        public void ToFileJSON(string myVenueCode, string myFilePath)
        {
            FileStream file;
            StreamWriter writer;
            try
            {
               

                file = new FileStream(myFilePath, FileMode.Create, FileAccess.Write);
                writer = new StreamWriter(file);

                foreach (KaiTrade.Interfaces.IProduct myProd in m_Products.Values)
                {

                    if (myProd.TradeVenue == myVenueCode)
                    {
                        K2DataObjects.Product prodData = new K2DataObjects.Product();
                        prodData.From(myProd);
                        string prodJSON = Newtonsoft.Json.JsonConvert.SerializeObject(prodData);
                        writer.WriteLine(prodJSON);
                        writer.Flush();
                    }
                    else if (myVenueCode=="*")
                    {
                        K2DataObjects.Product prodData = new K2DataObjects.Product();
                        prodData.From(myProd);
                        string prodJSON = Newtonsoft.Json.JsonConvert.SerializeObject(prodData);
                        writer.WriteLine(prodJSON);
                        writer.Flush();
                    }
                }
                writer.Close();
                file.Close();
                

            
            }
            catch (Exception myE)
            {
                m_Log.Error("ProductManager.ToFile:JSON", myE);
            }
            writer = null;
            file = null;

        }
        

        

        /// <summary>
        /// Add or modify a security in the manager using the FIX security
        /// definintion passed in.
        /// </summary>
        /// <param name="mySecDef"></param>
       
        /// <summary>
        /// Remove the product with the ID specified
        /// </summary>
        /// <param name="myID"></param>
        public void RemoveProduct(string myID)
        {
            try
            {
                KaiTrade.Interfaces.IProduct myProduct = GetProduct(myID);
                if (myProduct != null)
                {
                    m_MnemonicProduct.Remove(myProduct.Mnemonic);
                    m_Products.Remove(myID);
                }
                DoUpdate("DELETE", myID);
            }
            catch (Exception myE)
            {
                m_Log.Error("RemoveProduct", myE);
            }
        }

        /// <summary>
        /// Get a list of products for a given by venue and cficode(type)
        /// </summary>
        /// <param name="venueCode"></param>
        /// <param name="exchange"></param>
        /// <param name="CFICode"></param>
        /// <returns></returns>
        public List<KaiTrade.Interfaces.IProduct> GetProducts(string venueCode, string exchange, string CFICode)
        {
            List<KaiTrade.Interfaces.IProduct> products = new List<KaiTrade.Interfaces.IProduct>();
            try
            {
                foreach (KaiTrade.Interfaces.IProduct product in m_Products.Values)
                {
                    if (venueCode == product.TradeVenue)
                    {
                        if ((CFICode.Length > 0) && (CFICode!=product.CFICode))
                        {
                            continue;
                        }
                        if ((exchange.Length > 0) && (exchange != product.Exchange))
                        {
                            continue;
                        }
                        products.Add(product);
                    }
                }
            }
            catch (Exception myE)
            {
                m_Log.Error("GetProducts", myE);
            }
            return products;
        }
        #endregion
    }
}
