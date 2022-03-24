// ---------- ---------- ---------- ---------- ---------- ----------
// By: Patrick McDougle
// Class: CSE 681
// Date: Spring of 2022
// ---------- ---------- ---------- ---------- ---------- ----------
using CSE681.Project4.Data;
using CSE681.Project4.DataStructures;
using CSE681.Project4.GUI.P2P;
using CSE681.Project4.GUI.Service;
using CSE681.Project4.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSE681.Project4.GUI.Chat
{
    public class Model : ObservableObject
    {
        private readonly MainWindow _mainWindow;

        private readonly Thread _p2pMessageThread;

        public Model(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            _mainWindow.ClientToServer.OnListOfUsersUpdate += UpdateListOfUsers;

            _p2pMessageThread = new Thread(() => ThreadProcessP2PMessages())
            {
                IsBackground = true
            };
            _p2pMessageThread.Start();
        }

        public UserInformation LoggedUserInformation { get; internal set; }
        public MainWindow MainWindow => _mainWindow;
        public P2PListenService P2PListenService { get; set; }

        public ConcurrentQueue<MessageInfo> P2PMessages { get; set; } = new ConcurrentQueue<MessageInfo>();
        public P2PSendService P2PSendService { get; set; }

        public LinkedList<UserInformation> Users { get; set; } = new LinkedList<UserInformation>();

        public UserInformation GetUserInformation(Guid key)
        {
            return Users.First(x => x.Id == key);
        }

        public void UpdateListOfUsers(UserInformation[] userInformation)
        {
            foreach (UserInformation user in Users)
            {
                user.IsActive = false;
            }

            foreach (UserInformation user in userInformation)
            {
                if (Users.Contains(user))
                {
                    // update
                    UserInformation currentUser = Users.Find(user).Value;
                    currentUser.IsActive = true;
                }
                else
                {
                    // add
                    Users.AddLast(user);
                }
            }

            OnPropertyChanged(nameof(Users));
        }

        private void ThreadProcessP2PMessages()
        {
            while (true)
            {
                MessageInfo messageInfo = _mainWindow.P2PListenService.GetMessage();

                P2PMessages.Enqueue(messageInfo);
                OnPropertyChanged(nameof(P2PMessages));
            }
        }
    }
}