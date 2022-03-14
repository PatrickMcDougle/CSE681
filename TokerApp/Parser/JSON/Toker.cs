// ---------- ---------- ---------- ---------- ---------- ----------
// By: Patrick McDougle
// Class: CSE 681
// Date: Spring of 2022
// ---------- ---------- ---------- ---------- ---------- ----------
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Parser.JSON
{
    public class Toker : IToker
    {
        private readonly List<string> _tokBuffer = null;
        private TextReader _textReader = null;

        public Toker()
        {
            _tokBuffer = new List<string>();
        }

        public void Close()
        {
            _textReader.Close();
        }

        public string GetTok()
        {
            if (_tokBuffer.Count > 0)
            {
                String tok = _tokBuffer[0];
                _tokBuffer.RemoveAt(0);
                return tok.Trim();
            }
            StringBuilder temp = new StringBuilder();
            while (true)
            {
                int i = _textReader.Read();
                if (i == -1)
                {
                    return temp.ToString().Trim();
                }
                char ch = (char)i;
                if (IsTerminator(ch))
                {
                    if (temp.Length > 0)
                    {
                        _tokBuffer.Add(ch.ToString());
                        return temp.ToString().Trim();
                    }
                    return ch.ToString();
                }
                if (ch != ' ' && ch != '\n' && ch != '\r')
                    temp.Append(ch);
            }
        }

        public bool OpenFile(string fileName)
        {
            try
            {
                _textReader = new StreamReader(fileName);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        private bool IsTerminator(char tok)
        {
            switch (tok)
            {
                case ',': return true;
                case '{': return true;
                case '}': return true;
                case ':': return true;
                case '[': return true;
                case ']': return true;
                default: return false;
            }
        }
    }
}