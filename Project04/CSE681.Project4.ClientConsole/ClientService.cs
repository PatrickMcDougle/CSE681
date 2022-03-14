// ---------- ---------- ---------- ---------- ---------- ----------
// By: Patrick McDougle
// Class: CSE 681
// Date: Spring of 2022
// ---------- ---------- ---------- ---------- ---------- ----------
using CSE681.Project4.Data;
using CSE681.Project4.DataStructures;
using CSE681.Project4.ServiceContracts;
using System;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using System.Threading;

namespace CSE681.Project4.ClientConsole
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class ClientService
    {
        private const int MAX_COUNT = 10;
        private readonly BlockingQueue<string> _sendingBlockingQueue = null;
        private readonly Thread _threadSend;
        private readonly BlockingLinkedList<UserInformation> _userInformationList = null;
        private string _lastError = "";
        private IServerContract _serverUserChannel;

        public ClientService(string url)
        {
            int _tryCount = 0;
            _sendingBlockingQueue = new BlockingQueue<string>();
            _userInformationList = new BlockingLinkedList<UserInformation>();

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
            _threadSend = new Thread(ThreadProc)
            {
                IsBackground = true
            };
            _threadSend.Start();
        }

        public void Close()
        {
            ((IClientChannel)_serverUserChannel).Close();
            //ChannelFactory<IClientChannel> temp = (ChannelFactory<IClientChannel>)_serverUserChannel;
            //temp.Close();
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
                string userListJson = _serverUserChannel.GetListOfUsers();

                if (userListJson == null)
                {
                    Thread.Sleep(1000);
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

                Thread.Sleep(1000);
            }
        }
    }
}