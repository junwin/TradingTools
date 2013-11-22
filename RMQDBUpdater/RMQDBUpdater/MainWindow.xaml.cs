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

namespace RMQDBUpdater
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        RMQMessageListener listener = null;
        public MainWindow()
        {
            InitializeComponent();
            listener = new RMQMessageListener();
            string rmqHostName = RMQDBUpdater.Properties.Settings.Default.RMQHostName;
            RabbitMQPublisher.RMQFactory.Instance().HostName = rmqHostName;
        }

        private void StartBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string rmqBrokerAddr = RMQDBUpdater.Properties.Settings.Default.RMQHostName;
                RabbitMQPublisher.RMQFactory.Instance().HostName = rmqBrokerAddr;
                listener.Start();
            }
            catch
            {
            }
        }
    }
}
