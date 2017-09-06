using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RandomNumbersWinFormsClient
{
    // This data layer type is a thread safe Singleton
    public sealed class DataLayer : RandomNumbersService.IRandomNumbersServiceCallback
    {
        // delegate declaration
        public delegate void RecieveStreamingNumbersMsgDelegate(string streamingNumbersMsg);

        // Singleton class variables
        private static volatile DataLayer instance;
        private static object syncRoot = new Object();
        
        // Other class variables
        private RandomNumbersService.RandomNumbersServiceClient client = null;
        private RecieveStreamingNumbersMsgDelegate callbackDelegate;
        private bool _channelFlag = false;
        
        public bool ChannelFlag
        {
            get
            {
                return _channelFlag;
            }
        }

        // Singleton constrator
        private DataLayer() { }


        //########################################################################################
        //                                      Methods 
        //########################################################################################


        // A thread safe Singleton method for creating a single instance
        public static DataLayer Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new DataLayer();
                    }
                }

                return instance;
            }
        }

        // Method for creating an channel for the client and service 
        public void GenerateChannel(string baseAdressesComboBoxSelectedItemText, string protocolComboBoxItems)
        {
            InstanceContext instanceContext = new InstanceContext(this);
            // Choose between endpoints by specifying the endpointConfigurationName as a parameter to the constructor for a service reference.
            client = new RandomNumbersService.RandomNumbersServiceClient(instanceContext, protocolComboBoxItems+"_IRandomNumbersService", baseAdressesComboBoxSelectedItemText+"/RandomNumbersService");
            this._channelFlag = true;
        }


        //########################################################################################
        //                Methods used by the client to call the server methods
        //########################################################################################


        // Method for calling  the PingServer method of the server
        public bool PingServer()
        {
            return client.PingServer();
        }

        // Method for calling the VerifyUserNameAndPassword method of the server
        public bool VerifyUserNameAndPassword(string userTextBoxText,string passTextBoxText)
        {
            return client.VerifyUserNameAndPassword(userTextBoxText, passTextBoxText);
        }
        
        // Method for calling the GenerateRandomNumbers method of the server
        public void CallGenerateRandomNumbers(RecieveStreamingNumbersMsgDelegate callback)
        {
            this.callbackDelegate = callback;
            client.GenerateRandomNumbers();
        }

        // Property for calling the StartStopFlag method of the server (used to start/stop the random numbers streaming)
        public bool StartStopFlag
        {
            set
            {
                client.set_StartStopFlag(value);
            }
        }


        //########################################################################################
        //                Methods used by the server to call the client methods
        //########################################################################################


        // Method for calling the RecieveStreamingNumbersMsg method of the client (used by server call back)
        public void CallRecieveStreamingNumbersMsg(string streamingNumbersMsg)
        {
            callbackDelegate(streamingNumbersMsg);
        }
    }
}
