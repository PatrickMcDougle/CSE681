// ---------- ---------- ---------- ---------- ---------- ----------
// By: Patrick McDougle
// Class: CSE 681
// Date: Spring of 2022
// ---------- ---------- ---------- ---------- ---------- ----------

namespace Parser
{
    public interface IToker
    {
        void Close();

        string GetTok();

        bool OpenFile(string fileName);
    }
}