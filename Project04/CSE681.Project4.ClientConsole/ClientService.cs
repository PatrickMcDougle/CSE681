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

namespace CSE681.Project4.ClientConsole
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class ClientService
    {
        private const int MAX_COUNT = 10;
        private readonly BlockingLinkedList<UserInformation> _userInformationList = null;
        private string _lastError = "";
        private IServerContract _serverUserChannel;

        public ClientService(string url)
        {
            int tryCount = 0;
            _userInformationList = new BlockingLinkedList<UserInformation>();

            while (true)
            {
                try
                {
                    CreateUserChannel(url);
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
            Thread threadSend = new Thread(ThreadProc)
            {
                IsBackground = true
            };
            threadSend.Start();
        }

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

        private void ThreadProc()
        {
            string[] spliters = { "},{" };

            while (true)
            {
                Thread.Sleep(1000);

                string userListJson = _serverUserChannel.GetListOfUsers();

                if (userListJson == null)
                {
                    continue;
                }

                if (userListJson[0] == '[' && userListJson[1] == '{')
                {
                    userListJson = userListJson.Substring(2, userListJson.Length - 4);
                    string[] usersList = userListJson.Split(spliters, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string user in usersList)
                    {
                        if (UserInformation.TryParse(user, out UserInformation userInformation))
                        {
                            if (_userInformationList.Contains(userInformation))
                            {
                                _userInformationList.Find(userInformation).IsActive = userInformation.IsActive;
                            }
                            else
                            {
                                _userInformationList.AddLast(userInformation);
                            }
                        }
                    }
                }

                foreach (UserInformation ui in _userInformationList)
                {
                    Console.WriteLine(ui.ToJson());
                }
            }
        }
    }
}