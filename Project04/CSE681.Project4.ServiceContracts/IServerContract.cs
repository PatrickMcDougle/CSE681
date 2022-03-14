// ---------- ---------- ---------- ---------- ---------- ----------
// By: Patrick McDougle
// Class: CSE 681
// Date: Spring of 2022
// ---------- ---------- ---------- ---------- ---------- ----------
using System.ServiceModel;

namespace CSE681.Project4.ServiceContracts
{
    [ServiceContract(Namespace = "UserStuff")]
    public interface IServerContract
    {
        [OperationContract]
        void AddUser(string uuid, string username, uint ipAddress, uint port);

        [OperationContract]
        string GetListOfUsers();

        [OperationContract]
        void RemoveUser(string uuid);

        [OperationContract]
        void SetUserActive(string uuid);

        [OperationContract]
        void SetUserInactive(string uuid);
    }
}