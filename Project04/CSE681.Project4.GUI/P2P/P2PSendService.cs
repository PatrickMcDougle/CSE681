// ---------- ---------- ---------- ---------- ---------- ----------
// By: Patrick McDougle
// Class: CSE 681
// Date: Spring of 2022
// ---------- ---------- ---------- ---------- ---------- ----------
using CSE681.Project4.ServiceContracts;
using System;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

namespace CSE681.Project4.GUI.P2P
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class P2PSendService
    {
        private const int MAX_COUNT = 10;
        private string _lastError = "";
        private IPeer2PeerContract _peer;

        public P2PSendService(string url)
        {
            _peer = CreateConnection(url);
        }

        public void Close(IPeer2PeerContract peer2PeerContract)
        {
            ((IClientChannel)peer2PeerContract).Close();
        }

        internal void SendMessage(string toUuid, string fromUuid, string message)
        {
            if (string.IsNullOrEmpty(message)) return;

            Task.Run(() =>
            {
                _peer.SendMessage(toUuid, fromUuid, message);
            });
        }

        private IPeer2PeerContract CreateConnection(string url)
        {
            int tryCount = 0;
            IPeer2PeerContract peer2PeerContract = null;

            while (true)
            {
                try
                {
                    peer2PeerContract = CreateP2PChannel(url);
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

        private IPeer2PeerContract CreateP2PChannel(string url)
        {
            EndpointAddress baseAddress = new EndpointAddress(url);
            BasicHttpBinding binding = new BasicHttpBinding();
            ChannelFactory<IPeer2PeerContract> factory = new ChannelFactory<IPeer2PeerContract>(binding, baseAddress);
            IPeer2PeerContract p2pChannel = factory.CreateChannel();

            return p2pChannel;
        }
    }
}