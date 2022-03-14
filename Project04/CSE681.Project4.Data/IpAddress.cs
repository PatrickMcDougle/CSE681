// ---------- ---------- ---------- ---------- ---------- ----------
// By: Patrick McDougle
// Class: CSE 681
// Date: Spring of 2022
// ---------- ---------- ---------- ---------- ---------- ----------
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace CSE681.Project4.Data
{
    public class IpAddress
    {
        public uint Address { get; set; }
        public uint Port { get; set; }

        public static string GetAddress(uint address)
        {
            StringBuilder sb = new StringBuilder();

            byte[] bytes = new byte[4];

            bytes[0] = (byte)(address >> 24 & 255);
            bytes[1] = (byte)(address >> 16 & 255);
            bytes[2] = (byte)(address >> 8 & 255);
            bytes[3] = (byte)(address & 255);

            bool isFirst = true;
            foreach (byte b in bytes)
            {
                if (!isFirst) { sb.Append('.'); }
                sb.Append($"{b}");
                isFirst = false;
            }

            return sb.ToString();
        }

        public static uint GetAddress(string address)
        {
            uint addressValue = 0;
            string[] addressParts = address.Split('.');
            if (addressParts.Length != 4) return addressValue;

            foreach (string part in addressParts)
            {
                byte.TryParse(part, out byte value);
                addressValue <<= 8;
                addressValue += value;
            }
            return addressValue;
        }

        public static uint GetCurrentAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return GetAddress(ip.ToString());
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        public static bool TryParse(string address, out IpAddress ipAddress)
        {
            ipAddress = null;
            if (string.IsNullOrEmpty(address)) return false;

            string[] addressParts = address.Split(':');
            if (addressParts.Length != 2) return false;

            ipAddress = new IpAddress()
            {
                Address = GetAddress(addressParts[0]),
                Port = uint.Parse(addressParts[1])
            };
            return true;
        }

        public override int GetHashCode()
        {
            int hashCode = 319242977;
            hashCode = hashCode * -1521134295 + Port.GetHashCode();
            hashCode = hashCode * -1521134295 + Address.GetHashCode();
            return hashCode;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append($"{GetAddress(Address)}:{Port}");

            return sb.ToString();
        }
    }
}