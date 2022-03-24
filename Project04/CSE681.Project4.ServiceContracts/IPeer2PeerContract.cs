// ---------- ---------- ---------- ---------- ---------- ----------
// By: Patrick McDougle
// Class: CSE 681
// Date: Spring of 2022
// ---------- ---------- ---------- ---------- ---------- ----------
using CSE681.Project4.Data;
using System.ServiceModel;

namespace CSE681.Project4.ServiceContracts
{
    [ServiceContract(Namespace = "Peer2Peer")]
    public interface IPeer2PeerContract
    {
        MessageInfo GetMessage();

        [OperationContract]
        void SendMessage(string toUuid, string fromUuid, string message);
    }
}