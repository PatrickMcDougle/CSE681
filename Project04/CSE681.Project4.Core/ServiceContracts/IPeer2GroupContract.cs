// ---------- ---------- ---------- ---------- ---------- ----------
// By: Patrick McDougle
// Class: CSE 681
// Date: Spring of 2022
// ---------- ---------- ---------- ---------- ---------- ----------
using System.ServiceModel;

namespace CSE681.Project4.Core.ServiceContracts
{
    [ServiceContract(Namespace = "Peer2Group")]
    public interface IPeer2GroupContract
    {
        [OperationContract]
        void ChannelAnnouncement(string channelUuid, string channelName, string userUuid);

        [OperationContract]
        void GetRegisteredChannels(string userUuid);

        [OperationContract]
        void InviteToChannel(string channelName, string userUuid);

        [OperationContract]
        void JoinChannel(string channelName, string userUuid);

        [OperationContract]
        void ReceiveGroupMessage(string channelName, string fromUuid, string message);

        [OperationContract]
        void SendGroupMessage(string channelName, string fromUuid, string message);
    }
}