// ---------- ---------- ---------- ---------- ---------- ----------
// By: Patrick McDougle
// Class: CSE 681
// Date: Spring of 2022
// ---------- ---------- ---------- ---------- ---------- ----------
using CSE681.Project4.Data;
using CSE681.Project4.DataStructures;
using CSE681.Project4.ServiceContracts;
using System;
using System.ServiceModel;
using System.Text;

namespace CSE681.Project4.Server
{
    /*
   * InstanceContextMode determines the activation policy, e.g.:
   *
   *   PerCall    - remote object created for each call
   *              - runs on thread dedicated to calling client
   *              - this is default activation policy
   *   PerSession - remote object created in session on first call
   *              - session times out unless called again within timeout period
   *              - runs on thread dedicated to calling client
   *   Singleton  - remote object created in session on first call
   *              - session times out unless called again within timeout period
   *              - runs on one thread so all clients see same instance
   *              - access must be synchronized
   */

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class ClientServerService : IServerContract
    {
        private static BlockingLinkedList<UserInformation> _userBlockingList = new BlockingLinkedList<UserInformation>();
        private ServiceHost _serviceHost;
        private UserInformation currentUser;

        public ClientServerService()
        {
        }

        public void AddUser(string uuid, string username, uint ipAddress, uint port)
        {
            if (Guid.TryParse(uuid, out Guid userId))
            {
                UserInformation newUser = new UserInformation()
                {
                    Id = userId,
                    Name = username,
                    IsActive = true,
                    Created = DateTime.Now,
                    LastSeen = DateTime.Now,
                    Address = new IpAddress()
                    {
                        Address = ipAddress,
                        Port = port
                    }
                };

                if (!_userBlockingList.Contains(newUser))
                {
                    _userBlockingList.AddLast(newUser);

                    Console.WriteLine($"User Added: {newUser.Name} || {newUser.Id} || {newUser.Address} ||");
                    currentUser = newUser;
                }
            }
        }

        public void Close()
        {
            _serviceHost?.Close();
        }

        public void CreateRecevedChannel(string url)
        {
            BasicHttpBinding binding = new BasicHttpBinding();
            Uri address = new Uri(url);
            Type serviceType = typeof(ClientServerService);
            Type contractType = typeof(IServerContract);

            _serviceHost = new ServiceHost(serviceType, address);
            _serviceHost.AddServiceEndpoint(contractType, binding, address);
            _serviceHost.Open();
        }

        public string GetListOfUsers()
        {
            StringBuilder sb = new StringBuilder();
            bool isFirst = true;

            sb.Append("[");
            foreach (UserInformation user in _userBlockingList)
            {
                if (!isFirst) { sb.Append(','); }
                sb.Append(user.ToJson());
                isFirst = false;
            }
            sb.Append("]");

            return sb.ToString();
        }

        public void RemoveUser(string uuid)
        {
            UserInformation _foundUser = null;
            Guid.TryParse(uuid, out Guid guid);
            foreach (UserInformation user in _userBlockingList)
            {
                if (user.Id == guid)
                {
                    _foundUser = user;
                    Console.WriteLine($"User Removed: {_foundUser.Name} - {_foundUser.Id}");
                    break;
                }
            }
            if (_foundUser != null)
            {
                _userBlockingList.Remove(_foundUser);
            }
        }

        public void SetUserActive(string uuid)
        {
            Guid.TryParse(uuid, out Guid guid);
            foreach (UserInformation user in _userBlockingList)
            {
                if (user.Id == guid)
                {
                    user.IsActive = true;
                    break;
                }
            }
        }

        public void SetUserInactive(string uuid)
        {
            Guid.TryParse(uuid, out Guid guid);
            foreach (UserInformation user in _userBlockingList)
            {
                if (user.Id == guid)
                {
                    user.IsActive = false;
                    break;
                }
            }
        }
    }
}