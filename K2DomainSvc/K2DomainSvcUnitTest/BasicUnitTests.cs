using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace K2DomainSvcUnitTest
{
    [TestClass]
    public class BasicUnitTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            ProcessOrderJSONFIle("orderData.txt");
            ProcessProductJSONFIle("productData.txt");
            ProcessFillsJSONFIle("fillData.txt");
        }


       

        #region helpers

        public void ProcessOrderJSONFIle(string path)
        {
            FileStream file = null;
            StreamReader reader = null;
            try
            {

                file = new FileStream(path, FileMode.Open, FileAccess.Read);
                reader = new StreamReader(file);
                while (!reader.EndOfStream)
                {
                    string myLine = reader.ReadLine();

                    K2DataObjects.Order o = Newtonsoft.Json.JsonConvert.DeserializeObject<K2DataObjects.Order>(myLine);

                    K2DomainSvc.Factory.Instance().GetOrderSvc().InsertOrder(o);

                }
            }
            catch (Exception myE)
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

        public void ProcessFillsJSONFIle(string path)
        {
            FileStream file = null;
            StreamReader reader = null;
            try
            {

                file = new FileStream(path, FileMode.Open, FileAccess.Read);
                reader = new StreamReader(file);
                while (!reader.EndOfStream)
                {
                    string myLine = reader.ReadLine();

                    K2DataObjects.Fill f = Newtonsoft.Json.JsonConvert.DeserializeObject<K2DataObjects.Fill>(myLine);

                    K2DomainSvc.Factory.Instance().GetTradeSvc().InsertFill(f);

                }
            }
            catch (Exception myE)
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

        public void ProcessProductJSONFIle(string path)
        {
            FileStream file = null;
            StreamReader reader = null;
            try
            {

                file = new FileStream(path, FileMode.Open, FileAccess.Read);
                reader = new StreamReader(file);
                while (!reader.EndOfStream)
                {
                    string myLine = reader.ReadLine();

                    K2DataObjects.Product p = Newtonsoft.Json.JsonConvert.DeserializeObject<K2DataObjects.Product>(myLine);

                    K2DomainSvc.Factory.Instance().GetProductSvc().Insert(p, true);

                }
            }
            catch (Exception myE)
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

        #endregion

       

    }
}
