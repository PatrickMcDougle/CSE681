// ---------- ---------- ---------- ---------- ---------- ----------
// By: Patrick McDougle
// Class: CSE 681
// Date: Spring of 2022
// ---------- ---------- ---------- ---------- ---------- ----------
using CSE681.Project4.Data;
using CSE681.Project4.GUI.P2P;
using CSE681.Project4.Utilities;
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

                P2PSendService.SendMessage(ToUserId.ToString(), FromUserId.ToString(), Message);

                Messages.Add(new MessageInfo()
                {
                    DateTime = DateTime.Now,
                    Message = Message,
                    FromUserId = FromUserId,
                    ToUserId = ToUserId,
                });
                Message = string.Empty;
            },
            p => true);
        }

        public string Description { get; set; }
        public Guid FromUserId { get; internal set; }
        public string Message { get => _message; set => SetProperty(ref _message, value); }
        public ObservableCollection<MessageInfo> Messages { get; } = new ObservableCollection<MessageInfo>();
        public P2PSendService P2PSendService { get; set; }
        public ICommand SendMessageCommand { get; set; }
        public string Title { get; set; }
        public Guid ToUserId { get; internal set; }
    }
}