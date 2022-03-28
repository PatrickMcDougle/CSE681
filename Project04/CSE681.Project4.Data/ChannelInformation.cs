// ---------- ---------- ---------- ---------- ---------- ----------
// By: Patrick McDougle
// Class: CSE 681
// Date: Spring of 2022
// ---------- ---------- ---------- ---------- ---------- ----------
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace CSE681.Project4.Data
{
    public sealed class ChannelInformation : IEquatable<ChannelInformation>, IEqualityComparer<ChannelInformation>
    {
        public IpAddress Address { get; set; }
        public DateTime Created { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ICommand OpenChatWindow { get; set; }

        public static bool TryParse(string json, out ChannelInformation channelInformation)
        {
            channelInformation = null;
            if (string.IsNullOrWhiteSpace(json)) return false;
            if (json.Length < 10) return false;

            string[] spliters01 = { "\",\"" };
            string[] spliters02 = { "\":\"" };

            string[] keyValues = json
                    .Substring(1, json.Length - 2)
                    .Split(spliters01, StringSplitOptions.RemoveEmptyEntries);

            string name = string.Empty;
            Guid guid = Guid.Empty;
            IpAddress address = null;

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
                }
            }
            channelInformation = new ChannelInformation()
            {
                Name = name,
                Id = guid,
                Address = address
            };
            return true;
        }

        public bool Equals(ChannelInformation other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            if (Id == other.Id) return true;
            return false;
        }

        public bool Equals(ChannelInformation x, ChannelInformation y)
        {
            if (x is null) return false;
            if (y is null) return false;
            return x.Equals(y);
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (obj is ChannelInformation information) return Equals(information);
            return false;
        }

        public int GetHashCode(ChannelInformation obj)
        {
            return obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            int hashCode = 319242977;
            hashCode = hashCode * -1521134295 + EqualityComparer<IpAddress>.Default.GetHashCode(Address);
            hashCode = hashCode * -1521134295 + Created.GetHashCode();
            hashCode = hashCode * -1521134295 + Id.GetHashCode();
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

            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}