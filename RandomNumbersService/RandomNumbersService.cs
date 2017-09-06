using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

/*
 * Author: Moshe Koltun 
 * Date: 2/Sep/2017
 */

namespace RandomNumbersService
{

    /*
    General explanation about the project:
    ======================================
        
    1)  This project has a WCF service with two endpoints in order to support two diffrent protocols (wsDualHttpBinding and netTcpBinding).
        It had an additional mexHttpBinding protocol for Metadata exchange.
        The endpoits are defigned in the App.config file.

    2)  The communication between the server and the clients is by 'Duplex' messaging pattern using 'One-Way' operations.
        This is in order to enable the server to send (push) messages to the client (messages are random numbers and time and date).
        Both of the protocols used here support Duplex communication (i.e. wsDualHttpBinding and netTcpBinding). 
        BasicHttpBinding for example was not used because it does not support Duplex communication.
    
    3)  If you are running this project for the first time please run these two commands in the Command Line (Admin):
            'netsh http add urlacl url=http://+:8080/ user=USERNAME'
            'netsh http add urlacl url=http://+:8090/ user=USERNAME'  
    */

    public class RandomNumbersService : IRandomNumbersService
    {
        //Class variables
        private bool authenticationSucceeded = false;
        private bool _startStopFlag;

        public  bool StartStopFlag {
            get
            {
                return _startStopFlag;
            }
            set
            {
                _startStopFlag = value;
            }
        }

        //This method is for to be used by the client in order to detect if the server is turned on or not 
        public bool PingServer()
        {
            return true;
        }

        // This method is a simple hardcoded authentication mechanism of the server 
        public bool VerifyUserNameAndPassword(string username, string password)
        {
            // Hardcoded username and password values
            const string HardcodedUsername = "admin";
            const string HardcodedPassword = "1234";
            
            // Verification
            if (username == HardcodedUsername && password == HardcodedPassword)
            {
                // Update the class variable
                authenticationSucceeded = true;
                return authenticationSucceeded;
            }
            else
            {
                // Update the class variable
                authenticationSucceeded = false;
                return authenticationSucceeded;
            }
        }

        // This Method is resposable for generating random numbers and sending them to the client 
        public void GenerateRandomNumbers()
        {
            authenticationSucceeded = true; // for debug
            // Continue with this method only if the authenticationSucceeded flag is set to true
            if (!authenticationSucceeded)
            {
                return;
            }
            
            // local constants and variables
            const int maxRandom = 10000;
            const int sleepTime = 2000;
            string streamingNumbersMsg = string.Empty;
            Random rnd = new Random();
            var current = OperationContext.Current; // Execution context for the current thread. See call back below.

            Thread thread = new Thread(() =>
            {
                //Send random numbers to the client in an infinite loop
                while (true)
                {
                    // Check if start/stop button was clicked on the client side
                    if (this._startStopFlag)
                    {
                        // Generate a random number in the range of zero till maxRandom
                        int rndNum = rnd.Next(maxRandom);
                        // Prepare a string with message to send for the client
                        streamingNumbersMsg = string.Format("#{0} @ {1}", rndNum, DateTime.Now);
                        // Display this message on host as well
                        Console.WriteLine(streamingNumbersMsg);
                        // Get the callback channel to send messages to the client
                        try
                        {
                            current.GetCallbackChannel<IRandomNumbersCallback>().CallRecieveStreamingNumbersMsg(streamingNumbersMsg);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            Console.ReadLine();
                        }
                    }
                    // Wait for two seconds
                    Thread.Sleep(sleepTime);
                }
            });
            thread.Start();
        }
    }
}
