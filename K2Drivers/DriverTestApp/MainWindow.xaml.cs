using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DriverTestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

         KTASimulator.KTASimulator _driver = null;
       

        public MainWindow()
        {
            InitializeComponent();
            _driver = new KTASimulator.KTASimulator();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TestPriceFileStart();
        }

        public void TestPriceFileStart()
        {
            KTASimulator.FilePriceSource priceSrc = new KTASimulator.FilePriceSource(_driver);

            priceSrc.PriceUpdate += new KTASimulator.PriceUpdate(this.PriceUpdate);

            priceSrc.Start(@"TestData\AAPL_data.csv");

            ////List<KaiTrade.Interfaces.IProduct> products = _driver.Facade.GetProductManager().GetProducts("KTSIM", "", "");
            // Assert.AreEqual(products.Count, 1);
        }

        public void PriceUpdate(KaiTrade.Interfaces.IPXUpdate pxUpdate)
        {
        }
    }
}
