// ---------- ---------- ---------- ---------- ---------- ----------
// By: Patrick McDougle
// Class: CSE 681
// Date: Spring of 2022
// ---------- ---------- ---------- ---------- ---------- ----------
using CSE681.Project4.Core.Data;
using CSE681.Project4.Core.Utilities;
using CSE681.Project4.GUI.Service.P2G;
using CSE681.Project4.GUI.Service.P2P;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSE681.Project4.GUI.Chat
{
    public class Model : ObservableObject
    {
        private readonly MainWindow _mainWindow;

        public Model(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            _mainWindow.Peer2ServerSendService.OnListOfUsersUpdateEvent += UpdateListOfUsers;
            P2GListenService.OnListOfChannelsUpdateEvent += UpdateListOfChannels;

            Task.Run(() => ProcessP2PMessages());
            Task.Run(() => ProcessP2GMessages());
        }

        public LinkedList<ChannelInformation> Channels { get; set; } = new LinkedList<ChannelInformation>();
        public UserInformation LoggedUserInformation { get; internal set; }

        public MainWindow MainWindow => _mainWindow;

        public P2GListenService P2GListenService { get; set; }
        public ConcurrentQueue<MessageInfo> P2GMessages { get; set; } = new ConcurrentQueue<MessageInfo>();

        public P2PListenService P2PListenService { get; set; }

        public ConcurrentQueue<MessageInfo> P2PMessages { get; set; } = new ConcurrentQueue<MessageInfo>();

        public P2PSendService P2PSendService { get; set; }

        public LinkedList<UserInformation> Users { get; set; } = new LinkedList<UserInformation>();
        private Dictionary<Guid, P2GSendService> Peer2GroupSendServiceList { get; set; } = new Dictionary<Guid, P2GSendService>();

        public UserInformation GetUserInformation(Guid key)
        {
            return Users.First(x => x.Id == key);
        }

        internal void TellEveryoneAboutTheNewChannel(ChannelInformation ci, string userUuid)
        {
            Peer2GroupSendServiceList.ToList().ForEach(x =>
            {
                x.Value.SendChannelAnnouncement(ci.Id.ToString(), ci.Name, userUuid);
            });
        }

        private void ProcessP2GMessages()
        {
            while (true)
            {
                MessageInfo messageInfo = _mainWindow.Peer2GroupListenService.GetNextMessage();

                if (messageInfo != null)
                {
                    P2GMessages.Enqueue(messageInfo);
                    OnPropertyChanged(nameof(P2GMessages));
                }
            }
        }

        private void ProcessP2PMessages()
        {
            while (true)
            {
                MessageInfo messageInfo = _mainWindow.Peer2PeerListenService.GetNextMessage();

                P2PMessages.Enqueue(messageInfo);
                OnPropertyChanged(nameof(P2PMessages));
            }
        }

        private void UpdateListOfChannels(ChannelInformation[] allChannels)
        {
            foreach (ChannelInformation channel in allChannels)
            {
                if (Channels.Contains(channel))
                {
                    // update
                    //ChannelInformation ci= Channels.Find(channel).Value;  // no need to update at this time.
                }
                else
                {
                    Channels.AddLast(channel);
                    OnPropertyChanged(nameof(Channels));
                    //UserInformation user = Users.First(x => x.Id == channel.HostInfo.Id);
                }
            }
        }

        private void UpdateListOfUsers(UserInformation[] userInformation)
        {
            foreach (UserInformation user in userInformation)
            {
                if (Users.Contains(user))
                {
                    // update
                    UserInformation currentUser = Users.Find(user).Value;
                    currentUser.IsActive = user.IsActive;
                }
                else
                {
                    // add
                    Users.AddLast(user);
                    P2GSendService p2gSendService = new P2GSendService(user, $"http://{user.Address}/Peer2Group");
                    Peer2GroupSendServiceList.Add(user.Id, p2gSendService);

                    if (user.IsActive)
                    {
                        p2gSendService.GetChannelRequest();
                    }
                }
            }

            OnPropertyChanged(nameof(Users));
        }
    }
}