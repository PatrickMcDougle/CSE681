// ---------- ---------- ---------- ---------- ---------- ----------
// By: Patrick McDougle
// Class: CSE 681
// Date: Spring of 2022
// ---------- ---------- ---------- ---------- ---------- ----------
using CSE681.Project4.Core.Data;
using CSE681.Project4.Core.ServiceContracts;
using System;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

namespace CSE681.Project4.GUI.Service.P2G
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class P2GSendService
    {
        private const int MAX_COUNT = 10;
        private static readonly string[] SPLITERS = { "," };
        private readonly IPeer2GroupContract _peer;
        private readonly UserInformation _peerInfo;
        private string _lastError = "";

        public P2GSendService(UserInformation userInfo, string url)
        {
            _peer = CreateConnection(url);
            _peerInfo = userInfo;
        }

        public void Close(IPeer2GroupContract peer2GroupContract)
        {
            ((IClientChannel)peer2GroupContract).Close();
        }

        public void GetChannelRequest()
        {
            Task.Run(() =>
            {
                if (!_peerInfo.IsActive) return;

                try
                {
                    _peer.GetRegisteredChannels(_peerInfo.Id.ToString());
                }
                catch (EndpointNotFoundException ex)
                {
                    Console.WriteLine($"Endpoint Not Found: {ex}");
                }
            });
        }

        public void SendChannelAnnouncement(string channelUuid, string channelName, string userUuid)
        {
            if (string.IsNullOrEmpty(channelName)) return;
            if (string.IsNullOrEmpty(userUuid)) return;

            Task.Run(() =>
            {
                try
                {
                    _peer.ChannelAnnouncement(channelUuid, channelName, userUuid);
                }
                catch (EndpointNotFoundException ex)
                {
                    Console.WriteLine($"User Not Responding: {ex}");
                }
            });
        }

        public void SendGroupMessage(string channelName, string fromUuid, string message)
        {
            if (string.IsNullOrEmpty(channelName)) return;
            if (string.IsNullOrEmpty(fromUuid)) return;
            if (string.IsNullOrEmpty(message)) return;

            Task.Run(() =>
            {
                _peer.SendGroupMessage(channelName, fromUuid, message);
            });
        }

        public void SendJoinChannelMessage(string channelName, string userUuid)
        {
            if (string.IsNullOrEmpty(channelName)) return;
            if (string.IsNullOrEmpty(userUuid)) return;

            Task.Run(() =>
            {
                _peer.JoinChannel(channelName, userUuid);
            });
        }

        internal void SendReceiveGroupMessage(string channelName, string fromUuid, string message)
        {
            _peer.ReceiveGroupMessage(channelName, fromUuid, message);
        }

        private IPeer2GroupContract CreateConnection(string url)
        {
            int tryCount = 0;
            IPeer2GroupContract peer2PeerContract = null;

            while (true)
            {
                try
                {
                    peer2PeerContract = CreateP2GChannel(url);
                    tryCount = 0;
                    break;
                }
                catch (Exception ex)
                {
                    if (++tryCount < MAX_COUNT)
                    {
                        Thread.Sleep(100);
                    }
                    else
                    {
                        _lastError = ex.Message;
                        break;
                    }
                }
            }

            return peer2PeerContract;
        }

        private IPeer2GroupContract CreateP2GChannel(string url)
        {
            EndpointAddress baseAddress = new EndpointAddress(url);
            BasicHttpBinding binding = new BasicHttpBinding();
            ChannelFactory<IPeer2GroupContract> factory = new ChannelFactory<IPeer2GroupContract>(binding, baseAddress);
            IPeer2GroupContract p2gChannel = factory.CreateChannel();

            return p2gChannel;
        }
    }
}