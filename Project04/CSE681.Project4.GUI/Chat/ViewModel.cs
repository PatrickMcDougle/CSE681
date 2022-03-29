// ---------- ---------- ---------- ---------- ---------- ----------
// By: Patrick McDougle
// Class: CSE 681
// Date: Spring of 2022
// ---------- ---------- ---------- ---------- ---------- ----------
using CSE681.Project4.Core.Data;
using CSE681.Project4.GUI.Service.P2P;
using CSE681.Project4.Core.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using CSE681.Project4.GUI.Service.P2G;

namespace CSE681.Project4.GUI.Chat
{
    public class ViewModel : ObservableObject
    {
        private readonly object _lockChatWindows = new object();
        private readonly object _lockUsers = new object();
        private readonly Model _model;
        private int _chatWindowIndex = 0;

        private string _newChannelName;

        public ViewModel(Model model)
        {
            _model = model;
            _model.PropertyChanged += OnModelPropertyChanged;

            BindingOperations.EnableCollectionSynchronization(Users, _lockUsers);
            BindingOperations.EnableCollectionSynchronization(ChatWindows, _lockChatWindows);

            Random random = new Random();
            NewChannelName = $"Ch{random.Next(1000, 9999)}";

            OpenChannelWindow = new RelayCommand(o =>
            {
                if (string.IsNullOrEmpty(NewChannelName)) return;

                ChannelInformation ci = ButtonOpenChannelWindowClicked(_model.LoggedUserInformation, NewChannelName);

                NewChannelName = $"Ch{random.Next(1000, 9999)}";

                if (ci != null)
                {
                    TellEveryoneAboutTheNewChannel(ci, _model.LoggedUserInformation.Id.ToString());
                }
            },
            p => true);
        }

        public ObservableCollection<ChannelInformation> Channels { get; } = new ObservableCollection<ChannelInformation>();
        public int ChatWindowIndex { get => _chatWindowIndex; set => SetProperty(ref _chatWindowIndex, value); }
        public ObservableCollection<TabWindowInfo> ChatWindows { get; } = new ObservableCollection<TabWindowInfo>();
        public string NewChannelName { get => _newChannelName; set => SetProperty(ref _newChannelName, value); }
        public ICommand OpenChannelWindow { get; set; }
        public ObservableCollection<UserInformation> Users { get; } = new ObservableCollection<UserInformation>();

        private ChannelInformation ButtonOpenChannelWindowClicked(UserInformation creatorUser, string channelName)
        {
            if (ChatWindows.Any(x => x.ChannelName == channelName && x.UserFrom.Id == creatorUser.Id))
            {
                ChatWindowIndex = ChatWindows.ToList().FindIndex(x => x.ChannelName == channelName && x.UserFrom.Id == creatorUser.Id);
                return null;
            }
            ChannelInformation ci = CreateNewGroupChatWindow(creatorUser, channelName, true);
            ChatWindowIndex = ChatWindows.Count - 1;

            return ci;
        }

        private void ButtonOpenChannelWindowClicked(ChannelInformation ci)
        {
            if (ChatWindows.Any(x => x.ChannelName == ci.Name && x.UserFrom.Id == ci.Id))
            {
                ChatWindowIndex = ChatWindows.ToList().FindIndex(x => x.ChannelName == ci.Name && x.UserFrom.Id == ci.Id);
                return;
            }
            CreateNewGroupChatWindow(ci);
            ChatWindowIndex = ChatWindows.Count - 1;
        }

        private void ButtonOpenChatWindowClicked(UserInformation toUser)
        {
            if (ChatWindows.Any(x => x.UserTo != null && x.UserTo.Id == toUser.Id))
            {
                ChatWindowIndex = ChatWindows.ToList().FindIndex(x => x.UserTo != null && x.UserTo.Id == toUser.Id);
                return;
            }
            CreateNewPeerChatWindow(toUser);
            ChatWindowIndex = ChatWindows.Count - 1;
        }

        private void CreateNewGroupChatWindow(ChannelInformation ci)
        {
            ci.OpenChatWindow = new RelayCommand(o => ButtonOpenChannelWindowClicked(ci.HostInfo, ci.Name), p => true);

            TabWindowInfo newChannel = new TabWindowInfo()
            {
                IsChannel = true,
                Title = ci.Name,
                ChannelName = ci.Name,
                ChannelInfo = ci,
                UserFrom = _model.LoggedUserInformation,
                Description = $"{ci.HostInfo.Name} created channel {ci.Name}",
                Peer2GroupSendService = new P2GSendService(ci.HostInfo, $"http://{ci.HostInfo.Address}/Peer2Group")
            };

            ChatWindows.Add(newChannel);

            if (!Channels.Any(x => x.Name == ci.Name))
            {
                Channels.Add(ci);
            }
        }

        private ChannelInformation CreateNewGroupChatWindow(UserInformation userFrom, string channelName, bool isCreatedByHost = false)
        {
            ChannelInformation ci = new ChannelInformation()
            {
                Name = channelName,
                Address = userFrom.Address,
                Created = DateTime.Now,
                Id = Guid.NewGuid(),
                IsCreatedByHost = isCreatedByHost,
                OpenChatWindow = new RelayCommand(o => ButtonOpenChannelWindowClicked(userFrom, channelName), p => true)
            };

            ci.Chaters.AddLast(_model.LoggedUserInformation);
            ci.Chaters.AddLast(userFrom);

            TabWindowInfo newChannel = new TabWindowInfo()
            {
                IsChannel = true,
                Title = channelName,
                ChannelName = channelName,
                ChannelInfo = ci,
                UserFrom = _model.LoggedUserInformation,
                Description = $"{userFrom.Name} created channel {channelName}",
                Peer2GroupSendService = new P2GSendService(userFrom, $"http://{userFrom.Address}/Peer2Group")
            };

            ChatWindows.Add(newChannel);

            if (!Channels.Any(x => x.Name == ci.Name))
            {
                Channels.Add(ci);
            }

            return ci;
        }

        private void CreateNewPeerChatWindow(UserInformation toUser)
        {
            TabWindowInfo newChat = new TabWindowInfo()
            {
                Title = toUser.Name,
                Description = $"{toUser.Id} - {toUser.Created}",
                UserTo = toUser,
                UserFrom = _model.LoggedUserInformation,
                Peer2PeerSendService = new P2PSendService(toUser, $"http://{toUser.Address}/Peer2Peer")
            };

            ChatWindows.Add(newChat);
        }

        private void MakeSureGroupChatWindowExists(MessageInfo messageInfo)
        {
            if (!ChatWindows.Any(x => x.ChannelName == messageInfo.ChannelName))
            {
                // ChatWindow does not have any windows open with this channel name... so create it.
                CreateNewGroupChatWindow(messageInfo.UserFrom, messageInfo.ChannelName);
            }
        }

        private void MakeSurePeerChatWindowExists(UserInformation user)
        {
            if (!ChatWindows.Any(x => x.UserTo != null && x.UserTo.Id == user.Id))
            {
                // ChatWindow does not have any windows open with this user ID... so create it.
                CreateNewPeerChatWindow(user);
            }
        }

        private void OnModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(_model.Users):
                    UpdateUserList(_model.Users);
                    UpdateActiveStatus();
                    break;

                case nameof(_model.P2PMessages):
                    UpdatePeerMessages(_model.P2PMessages);
                    break;

                case nameof(_model.P2GMessages):
                    UpdateGroupMessages(_model.P2GMessages);
                    break;

                case nameof(_model.Channels):
                    UpdateChannelList(_model.Channels);
                    break;
            }
        }

        private void TellEveryoneAboutTheNewChannel(ChannelInformation ci, string userUuid)
        {
            _model.TellEveryoneAboutTheNewChannel(ci, userUuid);
        }

        private void UpdateActiveStatus()
        {
            if (Users == null) return;

            // 2000_000 should be 2 seconds in ticks.
            long secondsAgo = DateTime.Now.Ticks - 2000_000;
            Users
                .Where(x => x.LastSeen.Ticks < secondsAgo)
                .ToList()
                .ForEach(x => x.IsActive = false);
        }

        private void UpdateChannelList(LinkedList<ChannelInformation> channels)
        {
            if (channels == null) return;

            foreach (ChannelInformation ci in channels)
            {
                if (!Channels.Contains(ci))
                {
                    // add
                    ci.OpenChatWindow = new RelayCommand(o =>
                    {
                        bool requestToJoin = !ChatWindows.Any(x => x.ChannelName == ci.Name);

                        ButtonOpenChannelWindowClicked(ci);

                        if (requestToJoin)
                        {
                            TabWindowInfo twi = ChatWindows.First(x => x.ChannelInfo == ci);
                            twi.Peer2GroupSendService.SendJoinChannelMessage(ci.Name, _model.LoggedUserInformation.Id.ToString());
                        }
                    }, p => true);
                    Channels.Add(ci);
                }
            }
        }

        private void UpdateGroupMessages(ConcurrentQueue<MessageInfo> p2gMessages)
        {
            while (p2gMessages.TryDequeue(out MessageInfo messageInfo))
            {
                MakeSureGroupChatWindowExists(messageInfo);

                ObservableCollection<MessageInfo> messages = ChatWindows.First(x => x.ChannelName == messageInfo.ChannelName).Messages;

                //ChannelInformation ci = ChatWindows.First(x => x.ChannelName == messageInfo.ChannelName).ChannelInfo;

                //ChatWindows.First(x => x.ChannelName == messageInfo.ChannelName).ChannelInfo.Chaters.ToList().ForEach(x =>
                //{
                //    P2GSendService p2gSendService = new P2GSendService(x, $"http://{x.Address}/Peer2Group");
                //    p2gSendService.SendReceiveGroupMessage(messageInfo.ChannelName, messageInfo.UserFrom.Id.ToString(), messageInfo.Message);

                //    //x.Peer2GroupSendServer.ReceiveGroupMessage(messageInfo.ChannelName, messageInfo.UserFrom.Id.ToString(), messageInfo.Message);
                //});

                //messageInfo.UserFrom = _model.Users.First(x => x.Id == messageInfo.UserFrom.Id);  // not needed anymore
                //messageInfo.UserTo = _model.LoggedUserInformation; // not needed anymore.????
                _model.MainWindow.Dispatcher.Invoke(() =>
                {
                    messageInfo.DateTime = DateTime.Now;
                    messages.Add(messageInfo);
                });
            }
        }

        private void UpdatePeerMessages(ConcurrentQueue<MessageInfo> p2pMessages)
        {
            while (p2pMessages.TryDequeue(out MessageInfo messageInfo))
            {
                MakeSurePeerChatWindowExists(messageInfo.UserFrom);

                ObservableCollection<MessageInfo> messages = ChatWindows.Where(x => x.UserTo != null && x.UserTo.Id == messageInfo.UserFrom.Id).Select(y => y.Messages).First();

                //messageInfo.UserFrom = _model.Users.First(x => x.Id == messageInfo.UserFrom.Id);  // not needed anymore
                //messageInfo.UserTo = _model.LoggedUserInformation; // not needed anymore.????
                _model.MainWindow.Dispatcher.Invoke(() =>
                {
                    messageInfo.DateTime = DateTime.Now;
                    messages.Add(messageInfo);
                });
            }
        }

        private void UpdateUserList(LinkedList<UserInformation> users)
        {
            if (users == null) return;

            foreach (UserInformation otherUser in users)
            {
                if (Users.Contains(otherUser))
                {
                    // update
                    UserInformation current = Users.First(x => x.Id == otherUser.Id);

                    current.LastSeen = DateTime.Now;
                }
                else
                {
                    // add.
                    otherUser.OpenChatWindow = new RelayCommand(o => ButtonOpenChatWindowClicked(otherUser), p => true);
                    Users.Add(otherUser);
                }
            }
        }
    }
}