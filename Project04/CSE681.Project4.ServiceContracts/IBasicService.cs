// ---------- ---------- ---------- ---------- ---------- ----------
// By: Patrick McDougle
// Class: CSE 681
// Date: Spring of 2022
// ---------- ---------- ---------- ---------- ---------- ----------
using System.ServiceModel;

namespace CSE681.Project4.ServiceContracts
{
    [ServiceContract(Namespace = "HandCraftedService")]
    public interface IBasicService
    {
        [OperationContract]
        string GetMessage();

        [OperationContract]
        void SendMessage(string msg);
    }
}