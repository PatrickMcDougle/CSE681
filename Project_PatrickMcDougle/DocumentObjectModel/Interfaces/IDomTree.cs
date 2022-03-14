// ---------- ---------- ---------- ---------- ---------- ----------
// By: Patrick McDougle
// Class: CSE 681
// Date: Spring of 2022
// ---------- ---------- ---------- ---------- ---------- ----------
using System;

namespace CSE681.JSON.DOMs
{
    public interface IDomTree
    {
        bool IsError { get; set; }
        bool IsValid { get; set; }
        Guid UUID { get; set; }
    }
}