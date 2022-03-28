// ---------- ---------- ---------- ---------- ---------- ----------
// By: Patrick McDougle
// Class: CSE 681
// Date: Spring of 2022
// ---------- ---------- ---------- ---------- ---------- ----------
using CSE681.Project4.Core.Data;
using CSE681.Project4.DataStructures;
using CSE681.Project4.Core.ServiceContracts;
using System;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using CSE681.Project4.GUI.Service.P2G;
using System.Linq;

namespace CSE681.Project4.GUI.Service.P2S
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class SendService
    {
        private const int MAX_COUNT = 10;
        private static readonly string[] SPLITERS = { "},{" };
        private readonly BlockingQueue<string> _sendingBlockingQueue = null;
        private string _lastError = "";
        private IServerContract _serverUserChannel;

        public SendService(string url)
        {
            int _tryCount = 0;
            _sendingBlockingQueue = new BlockingQueue<string>();

            while (true)
            {
                try
                {
                    CreateUserChannel(url);
                    _tryCount = 0;
                    break;
                }
                catch (Exception ex)
                {
                    if (++_tryCount < MAX_COUNT)
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

            Task.Run(() => ProcessToUpdateHeartBeat());
            Task.Run(() => ProcessToUpdateListOfUsers());
        }

        public delegate void GetListOfUsers(UserInformation[] allUsers);

        public event GetListOfUsers OnListOfUsersUpdateEvent;

        public UserInformation LoggedUserInformation { get; internal set; }

        public BlockingLinkedList<UserInformation> UserInfoList { get; set; } = new BlockingLinkedList<UserInformation>();

        public void Close()
        {
            ((IClientChannel)_serverUserChannel).Close();
        }

        public void CreateUserChannel(string address)
        {
            EndpointAddress baseAddress = new EndpointAddress(address);
            BasicHttpBinding binding = new BasicHttpBinding();
            ChannelFactory<IServerContract> factory = new ChannelFactory<IServerContract>(binding, baseAddress);
            _serverUserChannel = factory.CreateChannel();
        }

        public string GetLastError()
        {
            string temp = _lastError;
            _lastError = "";
            return temp;
        }

        public void SendInitialInfo(UserInformation me)
        {
            _serverUserChannel.AddUser(me.Id.ToString(), me.Name, me.Address.Address, me.Address.Port);

            Console.WriteLine(me);
        }

        private void ProcessToUpdateHeartBeat()
        {
            while (true)
            {
                Thread.Sleep(1000);
                if (LoggedUserInformation != null)
                {
                    _serverUserChannel.SetUserActive(LoggedUserInformation.Id.ToString());
                }
            }
        }

        /// <summary>Background thread.</summary>
        private void ProcessToUpdateListOfUsers()
        {
            while (true)
            {
                Thread.Sleep(1000);
                string userListJson = _serverUserChannel.GetListOfUsers();

                if (userListJson != null)
                {
                    UpdateListOfUsersFromServer(userListJson);
                }
            }
        }

        private void UpdateListOfUsersFromServer(string userListJson)
        {
            if (userListJson[0] == '[' && userListJson[1] == '{')
            {
                // strip off the [{ at the begining and }] at the end of the JSON message.
                userListJson = userListJson.Substring(2, userListJson.Length - 4);

                // split the string into each user information set.
                string[] usersList = userListJson.Split(SPLITERS, StringSplitOptions.RemoveEmptyEntries);

                foreach (string user in usersList)
                {
                    if (UserInformation.TryParse(user, out UserInformation userInformation))
                    {
                        if (UserInfoList.Contains(userInformation))
                        {
                            UserInformation ui = UserInfoList.First(x => x.Id == userInformation.Id);
                            ui.IsActive = userInformation.IsActive;
                        }
                        else
                        {
                            UserInfoList.AddLast(userInformation);
                        }
                    }
                }
            }

            OnListOfUsersUpdateEvent?.Invoke(UserInfoList.ToArray());
        }
    }
}