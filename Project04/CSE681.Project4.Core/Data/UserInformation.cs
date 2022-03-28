// ---------- ---------- ---------- ---------- ---------- ----------
// By: Patrick McDougle
// Class: CSE 681
// Date: Spring of 2022
// ---------- ---------- ---------- ---------- ---------- ----------
using CSE681.Project4.Core.ServiceContracts;
using CSE681.Project4.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace CSE681.Project4.Core.Data
{
    public sealed class UserInformation : ObservableObject, IEquatable<UserInformation>, IEqualityComparer<UserInformation>
    {
        private bool _isActive;

        public UserInformation()
        {
        }

        public IpAddress Address { get; set; }
        public DateTime Created { get; set; }
        public Guid Id { get; set; }
        public bool IsActive { get => _isActive; set => SetProperty(ref _isActive, value); }
        public DateTime LastSeen { get; set; }
        public string Name { get; set; }

        public ICommand OpenChatWindow { get; set; }
        public IPeer2GroupContract Peer2GroupSendServer { get; set; }

        public static bool TryParse(string json, out UserInformation userInformation)
        {
            userInformation = null;
            if (string.IsNullOrEmpty(json)) return false;
            if (json.Length < 25) return false;

            string[] spliters01 = { "\",\"" };
            string[] spliters02 = { "\":\"" };

            string[] keyValues = json
                    .Substring(1, json.Length - 2)
                    .Split(spliters01, StringSplitOptions.RemoveEmptyEntries);

            string name = string.Empty;
            Guid guid = Guid.Empty;
            IpAddress address = null;
            bool isActive = false;

            foreach (string keyValue in keyValues)
            {
                string[] parts = keyValue
                    .Split(spliters02, StringSplitOptions.RemoveEmptyEntries);
                switch (parts[0])
                {
                    case nameof(Name):
                        name = parts[1];
                        break;

                    case nameof(Id):
                        Guid.TryParse(parts[1], out guid);
                        break;

                    case nameof(Address):
                        IpAddress.TryParse(parts[1], out address);
                        break;

                    case nameof(IsActive):
                        isActive = parts[1].ToLower() == "true";
                        break;
                }
            }
            userInformation = new UserInformation()
            {
                Name = name,
                Id = guid,
                Address = address,
                IsActive = isActive
            };
            return true;
        }

        public bool Equals(UserInformation other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            if (Id == other.Id) return true;
            return false;
        }

        public bool Equals(UserInformation x, UserInformation y)
        {
            if (x is null) return false;
            if (y is null) return false;
            return x.Equals(y);
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (obj is UserInformation information) return Equals(information);
            return false;
        }

        public int GetHashCode(UserInformation obj)
        {
            return obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            int hashCode = 319242977;
            hashCode = hashCode * -1521134295 + Address.GetHashCode();
            hashCode = hashCode * -1521134295 + Created.GetHashCode();
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
            hashCode = hashCode * -1521134295 + IsActive.GetHashCode();
            hashCode = hashCode * -1521134295 + LastSeen.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            return hashCode;
        }

        public string ToJson()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");

            sb.Append($"\"{nameof(Name)}\":\"{Name}\",");
            sb.Append($"\"{nameof(Id)}\":\"{Id}\",");
            sb.Append($"\"{nameof(Address)}\":\"{Address}\",");
            sb.Append($"\"{nameof(IsActive)}\":\"{IsActive}\"");

            sb.Append("}");

            return sb.ToString();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("{");

            sb.AppendLine($"    \"{nameof(Name)}\"     : \"{Name}\",");
            sb.AppendLine($"    \"{nameof(Id)}\"       : \"{Id}\",");
            sb.AppendLine($"    \"{nameof(Address)}\"  : \"{Address}\",");
            sb.AppendLine($"    \"{nameof(IsActive)}\" : \"{IsActive}\"");

            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}