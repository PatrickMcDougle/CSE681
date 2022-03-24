// ---------- ---------- ---------- ---------- ---------- ----------
// By: Patrick McDougle
// Class: CSE 681
// Date: Spring of 2022
// ---------- ---------- ---------- ---------- ---------- ----------
using CSE681.Project4.DataStructures;
using CSE681.Project4.GUI.Chat;
using CSE681.Project4.ServiceContracts;
using System;
using System.ServiceModel;

namespace CSE681.Project4.GUI.P2P
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class P2PListenService : IPeer2PeerContract
    {
        private ServiceHost _serviceHost;

        public P2PListenService()
        {
        }

        public static BlockingQueue<MessageInfo> IncomingMessages { get; set; } = new BlockingQueue<MessageInfo>();

        public void Close(IPeer2PeerContract peer2PeerContract)
        {
            ((IClientChannel)peer2PeerContract).Close();
        }

        public void Close()
        {
            _serviceHost?.Close();
        }

        public void CreateRecevedChannel(string url)
        {
            try
            {
                BasicHttpBinding binding = new BasicHttpBinding();
                Uri address = new Uri(url);
                Type serviceType = typeof(P2PListenService);
                Type contractType = typeof(IPeer2PeerContract);

                _serviceHost = new ServiceHost(serviceType, address);
                _serviceHost.AddServiceEndpoint(contractType, binding, address);
                _serviceHost.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex}");
            }
        }

        public MessageInfo GetMessage()
        {
            return IncomingMessages.Dequeue();
        }

        public void SendMessage(string toUuid, string fromUuid, string message)
        {
            if (Guid.TryParse(toUuid, out Guid userToId) && Guid.TryParse(fromUuid, out Guid userFromId))
            {
                MessageInfo newMessage = new MessageInfo()
                {
                    Message = message,
                    ToUserId = userToId,
                    FromUserId = userFromId,
                    IsFromUser = true
                };
                IncomingMessages.Enqueue(newMessage);
            }
        }
    }
}