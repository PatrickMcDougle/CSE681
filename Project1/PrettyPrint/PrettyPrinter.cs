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
    public class PrettyPrinter
    {
        private readonly int _indent = 4;

        private int _indentLevel = 0;

        public PrettyPrinter()
        {
        }

        public string PrettyPrintDOM(Value value)
        {
            return PrintValue(value);
        }

        private string PrintArray(Array value)
        {
            bool firstTimeSkip = true;
            StringBuilder sb = new StringBuilder();
            if (_indentLevel > 0)
            {
                sb.Append("\n");
            }
            sb.Append($"{PrintIndent()}[");
            _indentLevel++;
            foreach (Value jsonValue in value.Items)
            {
                if (firstTimeSkip)
                {
                    firstTimeSkip = false;
                }
                else
                {
                    sb.Append($",");
                }
                if (jsonValue is Object)
                {
                    sb.Append($"{PrintIndent()}{PrintValue(jsonValue)}");
                }
                else
                {
                    sb.Append($"\n{PrintIndent()}{PrintValue(jsonValue)}");
                }
            }
            _indentLevel--;
            sb.Append($"\n{PrintIndent()}]");

            return sb.ToString();
        }

        private string PrintBoolean(Boolean value)
        {
            return value.Value ? "true" : "false";
        }

        private string PrintIndent()
        {
            return new string(' ', _indent * _indentLevel);
        }

        private string PrintMembers(Members value)
        {
            return $"\"{value.Key}\": {PrintValue(value.Member)}";
        }

        private string PrintNumber(Number value)
        {
            if (value.IsWholeNumber)
            {
                return Convert.ToInt32(value.Value).ToString();
            }
            return value.Value.ToString();
        }

        private string PrintObject(Object value)
        {
            bool firstTimeSkip = true;
            StringBuilder sb = new StringBuilder();
            if (_indentLevel > 0)
            {
                sb.Append("\n");
            }
            sb.Append($"{PrintIndent()}{{");
            _indentLevel++;
            foreach (Members keyValueSet in value.Properties)
            {
                if (firstTimeSkip)
                {
                    firstTimeSkip = false;
                }
                else
                {
                    sb.Append($",");
                }
                sb.Append($"\n{PrintIndent()}{PrintMembers(keyValueSet)}");
            }
            _indentLevel--;
            sb.Append($"\n{PrintIndent()}}}");

            return sb.ToString();
        }

        private string PrintString(String value)
        {
            return $"\"{value.Value}\"";
        }

        private string PrintValue(Value value)
        {
            if (value == null) return "null";
            if (value is Boolean b)
            {
                return PrintBoolean(b);
            }
            if (value is Number n)
            {
                return PrintNumber(n);
            }
            if (value is String s)
            {
                return PrintString(s);
            }
            if (value is Array a)
            {
                return PrintArray(a);
            }
            if (value is Object o)
            {
                return PrintObject(o);
            }
            throw new NotImplementedException();
        }
    }
}