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

/*
 * Author: Moshe Koltun 
 * Date: 2/Sep/2017
 */

/*
General explanation about all the possiable clients:
====================================================

1) This project allows you to create two diffrent clients:
   - One client which support the 'netTcpBiding' protocol.
   - And a second client to suppport the 'wsDualHttpBinding' protocol.

2) After login to the client it will call the server 'GenerateRandomNumbers()' method.
   This method will callback the client 'Progress' method. 
  
   The purpose of this operation is to enable the server to constantly send (push) messages to the client
   (in this case string messages of a random number and the time and date) 

3) The client has a Service Reference to RandomNumbersService.
*/

namespace RandomNumbersWinFormsClient
{
    public partial class MainForm : Form
    {

        // Constractor
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Run instructions inside UI going to the server in a diffrent thread in order to disable UI lock   
            Thread thread = new Thread(() =>
            {
                DataLayer.Instance.StartStopFlag = true;
                DataLayer.Instance.CallGenerateRandomNumbers(RecieveStreamingNumbersMsg);
            });
            thread.Start();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            // Run instructions inside UI buttons going to the server in a diffrent thread in order to disable UI lock   
            Thread thread = new Thread(() => { DataLayer.Instance.StartStopFlag = true; });
            thread.Start();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            // Run instructions inside UI buttons going to the server in a diffrent thread in order to disable UI lock   
            Thread thread = new Thread(() => { DataLayer.Instance.StartStopFlag = false; });
            thread.Start();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            DataLayer.Instance.StartStopFlag = false;
            Thread.Sleep(2000);
            this.DialogResult = DialogResult.No;
        }

        //The 'RecieveStreamingNumbersMsg' method is being called back by the server in order to send (push) messages to the client 
        public void RecieveStreamingNumbersMsg(string streamingNumbersMsg)
        {
            /* The 'RecieveStreamingNumbersMsg' method provides a pattern for making thread-safe calls on a Windows Forms control.

            If the calling thread is different from the thread that created the listBox control, this method creates
            a StringArgReturningVoidDelegate and calls itself asynchronously using the Invoke method.

            If the calling thread is the same as the thread that created the TextBox control, the Text property is set directly. */

            // Check if invoke is required
            if (this.listBox1.InvokeRequired)
            {
                // Create a delegate for the invoke
                StringArgReturningVoidDelegate d = new StringArgReturningVoidDelegate(RecieveStreamingNumbersMsg);
                // Control.Invoke: Executes the specified delegate on the thread that owns the control's underlying window handle.
                this.listBox1.Invoke(d, new object[] { streamingNumbersMsg });
            }
            else
            {
                // Add random numbers, time, and date message to the list box
                listBox1.Items.Add(streamingNumbersMsg);
                // Enables automatic scrolling down to bottom of the listbox
                listBox1.TopIndex = listBox1.Items.Count - 1;
            }
        }

        // This delegate enables asynchronous calls for setting the text property on a ListBox control.  
        delegate void StringArgReturningVoidDelegate(string text);

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
