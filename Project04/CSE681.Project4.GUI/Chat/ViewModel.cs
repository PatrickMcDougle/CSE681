// ---------- ---------- ---------- ---------- ---------- ----------
// By: Patrick McDougle
// Class: CSE 681
// Date: Spring of 2022
// ---------- ---------- ---------- ---------- ---------- ----------
using CSE681.Project4.Data;
using CSE681.Project4.GUI.P2P;
using CSE681.Project4.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;

namespace CSE681.Project4.GUI.Chat
{
    public class ViewModel : ObservableObject
    {
        private readonly object _lockChatWindows = new object();
        private readonly object _lockUsers = new object();
        private readonly Model _model;
        private int _chatWindowIndex = 0;

        public ViewModel(Model model)
        {
            _model = model;
            _model.PropertyChanged += OnModelPropertyChanged;

            BindingOperations.EnableCollectionSynchronization(Users, _lockUsers);
            BindingOperations.EnableCollectionSynchronization(ChatWindows, _lockChatWindows);
        }

        public int ChatWindowIndex { get => _chatWindowIndex; set => SetProperty(ref _chatWindowIndex, value); }
        public ObservableCollection<TabWindowInfo> ChatWindows { get; } = new ObservableCollection<TabWindowInfo>();
        public ObservableCollection<UserInformation> Users { get; } = new ObservableCollection<UserInformation>();

        private void ButtonOpenChatWindowClicked(UserInformation toUser)
        {
            if (ChatWindows.Any(x => x.ToUserId == toUser.Id))
            {
                ChatWindowIndex = ChatWindows.ToList().FindIndex(x => x.ToUserId == toUser.Id);
                return;
            }
            CreateNewChatWindow(toUser);
            ChatWindowIndex = ChatWindows.Count - 1;
        }

        private void CreateNewChatWindow(UserInformation user)
        {
            TabWindowInfo newChat = new TabWindowInfo()
            {
                Title = user.Name,
                Description = $"{user.Id} - {user.Created}",
                ToUserId = user.Id,
                FromUserId = _model.LoggedUserInformation.Id,
                P2PSendService = new P2PSendService($"http://{user.Address}/Peer2Peer")
                //P2PSendService = new P2PSendService($"http://localhost:{user.Address.Port}/Peer2Peer")
            };

            ChatWindows.Add(newChat);
        }

        private void MakeSureChatWindowExists(Guid key)
        {
            if (!ChatWindows.Any(x => x.ToUserId == key))
            {
                // ChatWindow does not have any windows open with this user ID... so create it.
                UserInformation user = _model.GetUserInformation(key);
                CreateNewChatWindow(user);
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
                    UpdateMessagesFromP2P(_model.P2PMessages);
                    break;
            }
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

        private void UpdateMessagesFromP2P(ConcurrentQueue<MessageInfo> p2pMessages)
        {
            while (p2pMessages.TryDequeue(out MessageInfo messageInfo))
            {
                MakeSureChatWindowExists(messageInfo.FromUserId);

                ObservableCollection<MessageInfo> messages = ChatWindows.Where(x => x.ToUserId == messageInfo.FromUserId).Select(y => y.Messages).First();

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