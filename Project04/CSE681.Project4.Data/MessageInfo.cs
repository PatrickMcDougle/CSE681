// ---------- ---------- ---------- ---------- ---------- ----------
// By: Patrick McDougle
// Class: CSE 681
// Date: Spring of 2022
// ---------- ---------- ---------- ---------- ---------- ----------
using System;

namespace CSE681.Project4.Data
{
    public class MessageInfo
    {
        public string ChannelName { get; set; }
        public DateTime DateTime { get; set; }
        public bool IsFromUser { get; set; } = false;
        public string Message { get; set; }
        public UserInformation UserFrom { get; set; }
        public UserInformation UserTo { get; set; }
    }
}