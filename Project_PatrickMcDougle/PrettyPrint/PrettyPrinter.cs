// ---------- ---------- ---------- ---------- ---------- ----------
// By: Patrick McDougle
// Class: CSE 681
// Date: Spring of 2022
// ---------- ---------- ---------- ---------- ---------- ----------
using CSE681.JSON.DOMs;
using System;
using System.Text;
using Array = CSE681.JSON.DOMs.Array;
using Boolean = CSE681.JSON.DOMs.Boolean;
using Object = CSE681.JSON.DOMs.Object;
using String = CSE681.JSON.DOMs.String;

namespace CSE681.JSON.PrettyPrint
{
    /// <summary>
    /// This class will take a CSE681 JSON DOMs objects and print out the contents in a JSON format.
    /// </summary>
    public class PrettyPrinter
    {
        private readonly int _indent = 4;

        private int _indentLevel = 0;

        public PrettyPrinter()
        {
        }

        /// <summary>This method is the method that takes in one of the CSE681 JSON DOMs</summary>
        /// <param name="obj">The object should be one of the DOMs classes.</param>
        /// <returns>A string that represents the DOMs classes structure in JSON format.</returns>
        public string PrettyPrintDOM(object obj)
        {
            return PrintObject(obj);
        }

        private string Print(Array value)
        {
            bool firstTimeSkip = true;
            StringBuilder sb = new StringBuilder();
            if (_indentLevel > 0)
            {
                sb.Append("\n");
            }
            sb.Append($"{PrintIndent()}[");
            _indentLevel++;
            foreach (object obj in value.TheValue)
            {
                if (firstTimeSkip)
                {
                    firstTimeSkip = false;
                }
                else
                {
                    sb.Append($",");
                }
                if (obj is Object Obj)
                {
                    sb.Append($"{PrintIndent()}{Print(Obj)}");
                }
                else
                {
                    sb.Append($"\n{PrintIndent()}{PrintObject(obj)}");
                }
            }
            _indentLevel--;
            sb.Append($"\n{PrintIndent()}]");

            return sb.ToString();
        }

        private string Print(Boolean value)
        {
            return value.TheValue ? "true" : "false";
        }

        private string Print(Members value)
        {
            if (value == null) { return "null"; }
            return $"\"{value.Key}\": {PrintObject(value.Member)}";
        }

        private string Print(Number value)
        {
            if (value.IsWholeNumber)
            {
                return Convert.ToInt32(value.TheValue).ToString();
            }
            return $"{value.TheValue}";
        }

        private string Print(Object value)
        {
            bool firstTimeSkip = true;
            StringBuilder sb = new StringBuilder();
            if (_indentLevel > 0)
            {
                sb.Append("\n");
            }
            sb.Append($"{PrintIndent()}{{");
            _indentLevel++;
            foreach (Members members in value.TheValue)
            {
                if (firstTimeSkip)
                {
                    firstTimeSkip = false;
                }
                else
                {
                    sb.Append($",");
                }
                sb.Append($"\n{PrintIndent()}{Print(members)}");
            }
            _indentLevel--;
            sb.Append($"\n{PrintIndent()}}}");

            return sb.ToString();
        }

        private string Print(String value)
        {
            return $"\"{value.TheValue}\"";
        }

        private string PrintIndent()
        {
            return new string(' ', _indent * _indentLevel);
        }

        private string PrintObject(object obj)
        {
            if (obj == null) return "null";
            if (obj is Boolean b)
            {
                return Print(b);
            }
            if (obj is Number n)
            {
                return Print(n);
            }
            if (obj is String s)
            {
                return Print(s);
            }
            if (obj is Array a)
            {
                return Print(a);
            }
            if (obj is Object o)
            {
                return Print(o);
            }
            throw new NotImplementedException();
        }
    }
}