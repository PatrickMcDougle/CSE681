// ---------- ---------- ---------- ---------- ---------- ----------
// By: Patrick McDougle
// Class: CSE 681
// Date: Spring of 2022
// ---------- ---------- ---------- ---------- ---------- ----------
using CSE681.Project4.Core.Data;
using CSE681.Project4.DataStructures;
using CSE681.Project4.Core.ServiceContracts;
using System;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace CSE681.Project4.GUI.Service.P2G
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class P2GListenService : IPeer2GroupContract
    {
        private static BlockingLinkedList<UserInformation> _userInformationList;
        private ServiceHost _serviceHost;

        public delegate void GetListOfChannels(ChannelInformation[] allChannels);

        public static event GetListOfChannels OnListOfChannelsUpdateEvent;

        public static BlockingQueue<MessageInfo> IncomingMessages { get; private set; } = new BlockingQueue<MessageInfo>();
        public BlockingLinkedList<ChannelInformation> ChannelListings { get; private set; } = new BlockingLinkedList<ChannelInformation>();

        public static void SetUserInformationList(BlockingLinkedList<UserInformation> userInfoList)
        {
            _userInformationList = userInfoList;
        }

        public void ChannelAnnouncement(string channelUuid, string channelName, string userUuid)
        {
            if (Guid.TryParse(userUuid, out Guid userId) && Guid.TryParse(channelUuid, out Guid channelId))
            {
                if (_userInformationList == null) return;

                UserInformation ui = _userInformationList.First(x => x.Id == userId);

                ChannelInformation ci = new ChannelInformation()
                {
                    Name = channelName,
                    Address = ui.Address,
                    Created = DateTime.UtcNow,
                    Id = channelId,
                    HostInfo = ui,
                };

                ChannelListings.AddLast(ci);

                OnListOfChannelsUpdateEvent?.Invoke(ChannelListings.ToArray());
            }
        }

        public void Close(IPeer2GroupContract peer2GroupContract)
        {
            ((IClientChannel)peer2GroupContract).Close();
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
                Type serviceType = typeof(P2GListenService);
                Type contractType = typeof(IPeer2GroupContract);

                _serviceHost = new ServiceHost(serviceType, address);
                _serviceHost.AddServiceEndpoint(contractType, binding, address);
                _serviceHost.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex}");
            }
        }

        public MessageInfo GetNextMessage()
        {
            return IncomingMessages.Dequeue();
        }

        public void GetRegisteredChannels(string userUuid)
        {
            if (string.IsNullOrEmpty(userUuid)) return;
            if (_userInformationList == null) return;

            Task.Run(() =>
            {
                if (Guid.TryParse(userUuid, out Guid userId))
                {
                    UserInformation ui = _userInformationList.First(x => x.Id == userId);

                    ChannelListings
                        .Where(x => x.IsCreatedByHost)
                        .ToList()
                        .ForEach(y => ui.Peer2GroupSendServer.ChannelAnnouncement(y.Id.ToString(), y.Name, y.Id.ToString()));
                }
            });
        }

        public void InviteToChannel(string channelName, string userUuid)
        {
            throw new NotImplementedException();
        }

        public void JoinChannel(string channelName, string userUuid)
        {
            if (string.IsNullOrEmpty(channelName)) return;
            if (string.IsNullOrEmpty(userUuid)) return;

            if (Guid.TryParse(userUuid, out Guid userId))
            {
                UserInformation ui = _userInformationList.First(x => x.Id == userId);

                ChannelInformation ci = ChannelListings.First(x => x.Name == channelName);

                if (!ci.Chaters.Contains(ui))
                {
                    ci.Chaters.AddLast(ui);
                    MessageInfo mi = new MessageInfo()
                    {
                        Message = "[Join Channel]",
                        ChannelName = channelName,
                        UserFrom = ui,
                        IsFromUser = true,
                        IsSystemGenerated = true,
                    };
                    IncomingMessages.Enqueue(mi);
                }
            }
        }

        public void ReceiveGroupMessage(string channelName, string fromUuid, string message)
        {
            throw new NotImplementedException();
        }

        public void SendGroupMessage(string channelName, string fromUuid, string message)
        {
            if (Guid.TryParse(fromUuid, out Guid userFromId))
            {
                UserInformation userFrom = null;

                foreach (UserInformation user in _userInformationList)
                {
                    if (user.Id == userFromId)
                    {
                        userFrom = user;
                        break;
                    }
                }

                MessageInfo mi = new MessageInfo()
                {
                    Message = message,
                    ChannelName = channelName,
                    UserFrom = userFrom,
                    IsFromUser = true,
                };
                IncomingMessages.Enqueue(mi);
            }
        }
    }
}