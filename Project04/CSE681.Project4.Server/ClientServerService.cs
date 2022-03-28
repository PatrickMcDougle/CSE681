// ---------- ---------- ---------- ---------- ---------- ----------
// By: Patrick McDougle
// Class: CSE 681
// Date: Spring of 2022
// ---------- ---------- ---------- ---------- ---------- ----------
using CSE681.Project4.Core.Data;
using CSE681.Project4.DataStructures;
using CSE681.Project4.Core.ServiceContracts;
using System;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ClientServerService : IServerContract
    {
        private static BlockingLinkedList<UserInformation> _userBlockingList = new BlockingLinkedList<UserInformation>();
        private readonly CancellationTokenSource _inactiveUserCancellationSource;
        private bool _processRunning = false;
        private ServiceHost _serviceHost;
        private UserInformation currentUser;

        public ClientServerService()
        {
            _inactiveUserCancellationSource = new CancellationTokenSource();
            CreateProcessOnce();
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
                    Created = DateTime.UtcNow,
                    LastSeen = DateTime.UtcNow,
                    Address = new IpAddress()
                    {
                        Address = ipAddress,
                        Port = port
                    }
                };

                if (!_userBlockingList.Contains(newUser))
                {
                    _userBlockingList.AddLast(newUser);

                    Console.WriteLine($"User Added   : {newUser.Name} || {newUser.Id} || {newUser.Address} ||");
                    currentUser = newUser;
                }
            }
        }

        public void Close()
        {
            _inactiveUserCancellationSource.Cancel();
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

            _userBlockingList.ToList().ForEach(x =>
            {
                if (!isFirst) { sb.Append(','); }
                sb.Append(x.ToJson());
                isFirst = false;
            });

            sb.Append("]");

            return sb.ToString();
        }

        public void ProcessIfUsersAreInactive(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                Thread.Sleep(1000);

                DateTime secondsAgo = DateTime.UtcNow.AddSeconds(-10);

                _userBlockingList
                    .Where(x => x.IsActive && x.LastSeen < secondsAgo)
                    .ToList()
                    .ForEach(y => SetUserInactive(y));
            }
        }

        public void RemoveUser(string uuid)
        {
            if (Guid.TryParse(uuid, out Guid guid))
            {
                UserInformation user = _userBlockingList.FirstOrDefault(x => x.Id == guid);

                if (user != null)
                {
                    Console.WriteLine($"User Removed : {user.Name} || {user.Id} || {user.Address} ||");
                    _userBlockingList.Remove(user);
                }
            }
        }

        public void SetUserActive(string uuid)
        {
            if (Guid.TryParse(uuid, out Guid guid))
            {
                UserInformation user = _userBlockingList.FirstOrDefault(x => x.Id == guid);

                if (user != null)
                {
                    SetUserActive(user);
                }
            }
        }

        public void SetUserInactive(string uuid)
        {
            if (Guid.TryParse(uuid, out Guid guid))
            {
                SetUserInactive(_userBlockingList.First(x => x.Id == guid));
            }
        }

        private void CreateProcessOnce()
        {
            if (_processRunning) return;
            Task.Run(() => ProcessIfUsersAreInactive(_inactiveUserCancellationSource.Token));
            _processRunning = true;
        }

        private void SetUserActive(UserInformation user)
        {
            if (!user.IsActive)
            {
                Console.WriteLine($"User Active  : {user.Name} || {user.Id} || {user.Address} ||");
            }

            user.IsActive = true;
            user.LastSeen = DateTime.UtcNow;
        }

        private void SetUserInactive(UserInformation user)
        {
            user.IsActive = false;

            Console.WriteLine($"User Inactive: {user.Name} || {user.Id} || {user.Address} ||");
        }
    }
}