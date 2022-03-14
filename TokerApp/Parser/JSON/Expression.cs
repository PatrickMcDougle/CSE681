// ---------- ---------- ---------- ---------- ---------- ----------
// By: Patrick McDougle
// Class: CSE 681
// Date: Spring of 2022
// ---------- ---------- ---------- ---------- ---------- ----------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser.JSON
{
    public class Expression : IExpression
    {
        private readonly List<string> _semiExpressionList = null;
        private readonly IToker _toker = null;

        public Expression(IToker toker)
        {
            this._toker = toker;
            _semiExpressionList = new List<string>();
        }

        public void Close()
        {
            _toker.Close();
        }

        public string Display()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\n");
            foreach (string token in _semiExpressionList)
            {
                sb.Append(token);
            }
            return sb.ToString();
        }

        public bool Get()
        {
            _semiExpressionList.RemoveRange(0, _semiExpressionList.Count);  // empty container
            string currTok;
            do
            {
                currTok = _toker.GetTok();
                if (currTok == "")
                {
                    return false;  // end of file
                }

                _semiExpressionList.Add(currTok);
            } while (!IsTerminator(currTok));

            return _semiExpressionList.Count > 0;
        }

        public List<string> GetSemi()
        {
            return _semiExpressionList;
        }

        public bool Open(string fileName)
        {
            return _toker.OpenFile(fileName);
        }

        private bool IsTerminator(string tok)
        {
            switch (tok)
            {
                case ",": return true;
                case "{": return true;
                case "}": return true;
                case "[": return true;
                case "]": return true;
                default: return false;
            }
        }
    }
}