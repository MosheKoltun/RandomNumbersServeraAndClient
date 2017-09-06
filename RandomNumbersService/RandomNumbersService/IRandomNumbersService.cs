using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace RandomNumbersService
{
    // Associate callback contract with service contract using CallbackContract attribute
    [ServiceContract(CallbackContract = typeof(IRandomNumbersCallback))]
    public interface IRandomNumbersService
    {
        // Since we have not set IsOnway=true, the operation is Request/Reply operation
        [OperationContract(IsOneWay = true)]
        void GenerateRandomNumbers();
        bool StartStopFlag { [OperationContract] get; [OperationContract] set; }
        [OperationContract]
        bool VerifyUserNameAndPassword(string username, string password);
        [OperationContract]
        bool PingServer();
    }

    // This is the callback contract
    public interface IRandomNumbersCallback
    {
        // Since we have not set IsOnway=true, the operation is Request/Reply operation
        [OperationContract(IsOneWay = true)]
        void CallRecieveStreamingNumbersMsg(string StreamingNumbersMsg);
    }
}
