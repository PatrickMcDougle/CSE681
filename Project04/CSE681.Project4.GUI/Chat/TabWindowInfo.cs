// ---------- ---------- ---------- ---------- ---------- ----------
// By: Patrick McDougle
// Class: CSE 681
// Date: Spring of 2022
// ---------- ---------- ---------- ---------- ---------- ----------
using CSE681.Project4.Core.Data;
using CSE681.Project4.GUI.Service.P2G;
using CSE681.Project4.GUI.Service.P2P;
using CSE681.Project4.Core.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CSE681.Project4.GUI.Chat
{
    public class TabWindowInfo : ObservableObject
    {
        private string _message;

        public TabWindowInfo()
        {
            SendMessageCommand = new RelayCommand(o =>
            {
                if (string.IsNullOrEmpty(Message)) return;

                if (IsChannel)
                {
                    SendChannelMessage();
                }
                else
                {
                    SendPeerMessage();
                }

                Messages.Add(new MessageInfo()
                {
                    DateTime = DateTime.Now,
                    Message = Message,
                    UserFrom = UserFrom,
                    UserTo = UserTo,
                });
                Message = string.Empty;
            },
            p => true);
        }

        public ChannelInformation ChannelInfo { get; internal set; }
        public string ChannelName { get; set; }
        public string Description { get; set; }

        public bool IsChannel { get; internal set; }

        public string Message { get => _message; set => SetProperty(ref _message, value); }

        public ObservableCollection<MessageInfo> Messages { get; } = new ObservableCollection<MessageInfo>();

        public P2GSendService Peer2GroupSendService { get; internal set; }
        public P2PSendService Peer2PeerSendService { get; set; }
        public ICommand SendMessageCommand { get; set; }

        public string Title { get; set; }
        public UserInformation UserFrom { get; internal set; }

        public UserInformation UserTo { get; internal set; }

        private void SendChannelMessage()
        {
            Peer2GroupSendService.SendGroupMessage(ChannelName, UserFrom.Id.ToString(), Message);
        }

        private void SendPeerMessage()
        {
            Peer2PeerSendService.SendMessage(UserTo.Id.ToString(), UserFrom.Id.ToString(), Message);
        }
    }
}