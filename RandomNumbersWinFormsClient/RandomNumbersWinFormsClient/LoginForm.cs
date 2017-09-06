using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RandomNumbersWinFormsClient
{
    public partial class LoginForm : Form
    {
        // Constractor
        public LoginForm()
        {
            InitializeComponent();
            // Add items to the base adress combobox  
            baseAdressesComboBox.Items.Add("http://localhost:8080");
            baseAdressesComboBox.Items.Add("net.tcp://localhost:8090");
            // Add items to the protocol combobox
            protocolComboBox.Items.Add("WSDualHttpBinding");
            protocolComboBox.Items.Add("NetTcpBinding");
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            // local variables
            bool authenticationSuccedded = false;
            bool pingResult = false;


            // Generate channel through the singleton instance
            // Run instructions inside UI going to the server in a diffrent thread in order to disable UI lock 
            try
            {
                DataLayer.Instance.GenerateChannel(baseAdressesComboBox.SelectedItem.ToString(), protocolComboBox.SelectedItem.ToString());
            }
            catch (Exception ex)
            {  
                    MessageBox.Show("Server Communication Error!");
                    MessageBox.Show(ex.Message);
                    return;
            }

            // Check if server is on/off by sending ping.
            try
            {
                pingStatusLabel.Text = "Please wait...\nA ping was sent to the server.";
                pingResult = DataLayer.Instance.PingServer();
                if (pingResult)
                {
                    pingStatusLabel.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                pingStatusLabel.Text = string.Empty;
                MessageBox.Show("Server Communication Error!");
                MessageBox.Show(ex.Message);
                return;
            }

            //Check username and password
            authenticationSuccedded = DataLayer.Instance.VerifyUserNameAndPassword(UserTextBox.Text, PassTextBox.Text);
            if (!authenticationSuccedded)
            {
                MessageBox.Show("Incorrect Username and Password!");
                return;
            }

            // Hide the login form
            this.Hide();
            // Open the MainForm as a dialog box
            var myMainForm = new MainForm();
            // Close both forms from the MainForm Exit button
            if (myMainForm.ShowDialog() == DialogResult.No)
            {
                this.Close();
            } 
        }


        // Close the form 
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void PassTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void UserTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButtonWsDualHttp_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private void radioButtonNetTcp_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void baseAdressesComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}




