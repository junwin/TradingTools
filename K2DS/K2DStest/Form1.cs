using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace K2DStest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnAddAccount_Click(object sender, EventArgs e)
        {
            K2DS.K2DataSvc svc = new K2DS.K2DataSvc();
            K2DataObjects.Account account = new K2DataObjects.Account();
            account.FirmCode="ZZZZ";
            account.AccountCode="acme";
            account.LongName="Some very long name";
            account.VenueCode="super";

            svc.AddAccount(account);
        }

        private void btnTestCreate_Click(object sender, EventArgs e)
        {
            try
            {
                //K2DataObjects.DataContext db = new K2DataObjects.DataContext(@"Data Source=JUWIN7\SQLEXPRESS;Initial Catalog=K2DS;Integrated Security=True;Pooling=False");
                K2DataObjects.DataContext db = K2DS.Factory.Instance().GetDSContext();
                //K2DataObjects.DataContext db = new K2DataObjects.DataContext(@"Data Source=10.1.11.15;Initial Catalog=K2DS;User ID=sa;Password=quink1nk");
                if (db.DatabaseExists())
                {
                    Console.WriteLine("Deleting old database...");
                    db.DeleteDatabase();
                }
                db.CreateDatabase();

            }
            catch (Exception myE)
            {
            }
        }

         
        private void btnAddUser_Click(object sender, EventArgs e)
        {
            try
            {
                K2DataObjects.User user = new K2DataObjects.User();
                if (txtUserId.Text.Length != 0)
                {
                    user.UserID = txtUserId.Text;
                    user.UserPwd = txtPassword.Text;
                    user.K2Config = txtConfig.Text;
                    user.Enabled = chkEnabled.Checked;

                    K2DS.K2UserDS ds = new K2DS.K2UserDS();
                    ds.InsertUser(user);

                    txtID.Text = user.ID;
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("you must enter a name");
                }

                 
            }
            catch (Exception myE)
            {
            }
        }

        private void btnDeleteUser_Click(object sender, EventArgs e)
        {
            try
            {
                

                K2DS.K2UserDS ds = new K2DS.K2UserDS();
                ds.DeleteUser(txtID.Text);

                txtID.Text = "";


            }
            catch (Exception myE)
            {
            }
        }

        private void btnSignInValidate_Click(object sender, EventArgs e)
        {
            try
            {
                /*
                K2DS.K2UserDS ds = new K2DS.K2UserDS();
                K2DataObjects.User user =  ds.ValidateUserSignOn(txtUserId.Text, txtPassword.Text, System.Environment.MachineName);
                if (user != null)
                {
                    txtUserId.Text = user.UserID;
                    txtID.Text = user.ID;
                }
                else
                {
                    txtUserId.Text = "NOT KNOWN";
                    txtID.Text = "NOT KNOWN";
                }
                 */
            }
            catch (Exception myE)
            {
            }
        }
    }
}
