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
        public DateTime DateTime { get; set; }

        public Guid FromUserId { get; set; }
        public bool IsFromUser { get; set; } = false;
        public string Message { get; set; }

        public Guid ToUserId { get; set; }
    }
}