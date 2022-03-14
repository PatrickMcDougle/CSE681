// ---------- ---------- ---------- ---------- ---------- ----------
// By: Patrick McDougle
// Class: CSE 681
// Date: Spring of 2022
// ---------- ---------- ---------- ---------- ---------- ----------
using CSE681.JSON.DOMs;
using System;
using Array = CSE681.JSON.DOMs.Array;
using Boolean = CSE681.JSON.DOMs.Boolean;
using Object = CSE681.JSON.DOMs.Object;
using String = CSE681.JSON.DOMs.String;

namespace CSE681.JSON.Parse
{
    /// <summary>This class will Parse a strin/text/file into a set of CSE681 JSON DOMs objects.</summary>
    public class Parser
    {
        private readonly string fullJsonString;

        private int stringPointer = 0;

        /// <summary>
        /// This constructor will set everything up that is needed to parse the string that has a
        /// valid JSON syntax.
        /// </summary>
        /// <param name="jsonString">The string that contains the JSON syntax to be parsed.</param>
        public Parser(string jsonString)
        {
            if (jsonString != null)
            {
                fullJsonString = jsonString.Trim(); // trim off any white space characters.
            }
        }

        /// <summary>
        /// This method will parse the JSON string and return a reference to the base membor object
        /// of the JSON tree.
        /// </summary>
        /// <returns>
        /// A reference to the first/base Members object that was in the JSON string. Or null if not valid.
        /// </returns>
        public Members GetJsonMembers()
        {
            if (string.IsNullOrWhiteSpace(fullJsonString))
            {
                // string is empty.
                return null;
            }

            Trim();

            if (fullJsonString.Length <= stringPointer || fullJsonString[stringPointer] != '"')
            {
                // we did not find a " so return null or we are at the end of our string.
                return null;
            }
            // key first
            string key = ParseString();

            Trim();

            // colon (:) should be next character.
            if (fullJsonString.Length <= stringPointer || fullJsonString[stringPointer] != ':')
            {
                // we did not find a colon so return null or we are at the end of out string.
                return null;
            }

            stringPointer++; // move pointer past colon (:)

            object jsonValue = ParseValue();

            Members members = new Members
            {
                Key = key,
                Member = jsonValue,
                IsValid = key.Length > 0
            };

            return members;
        }

        /// <summary>
        /// This method is the main method to parse all JSON strings. This method will parse the
        /// whole JSON tree structure into CSE681 JSON DOMs objects.
        /// </summary>
        /// <returns>
        /// The first/base/parent JSON Values object in the JSON tree string. This will either be an
        /// Object or an Array. Or will return a blank/empty Object.
        /// </returns>
        public object GetJsonValue()
        {
            return string.IsNullOrWhiteSpace(fullJsonString) ? null : ParseValue();
        }

        private object ParseArray()
        {
            Array array = new Array();

            // in the array... so parse out values

            while (fullJsonString.Length > stringPointer)
            {
                object jsonValue = ParseValue();

                array.Add(jsonValue);
                array.IsValid = jsonValue != null;

                Trim();

                if (fullJsonString[stringPointer] != ',')
                {
                    // did not find comma (,) so this must be end of the array set.
                    if (fullJsonString[stringPointer] == ']')
                    {
                        stringPointer++;
                    }
                    else
                    {
                        // did not find end of array, so flag as error
                        array.IsValid = false;
                        array.IsError = true;
                    }
                    break;
                }
                else
                {
                    stringPointer++;  // move past the (,) character
                }
            }

            return array;
        }

        private Boolean ParseBoolean()
        {
            if (char.ToLower(fullJsonString[stringPointer]) == 't' && fullJsonString.Length >= stringPointer + 4)
            {
                string substring = fullJsonString.Substring(stringPointer, 4);
                if (substring.Equals("true", StringComparison.OrdinalIgnoreCase))
                {
                    stringPointer += 4;
                    return new Boolean(true);
                }
            }
            else if (char.ToLower(fullJsonString[stringPointer]) == 'f' && fullJsonString.Length >= stringPointer + 5)
            {
                string substring = fullJsonString.Substring(stringPointer, 5);
                if (substring.Equals("false", StringComparison.OrdinalIgnoreCase))
                {
                    stringPointer += 5;
                    return new Boolean(false);
                }
            }

            return null;
        }

        private object ParseNull()
        {
            if (char.ToLower(fullJsonString[stringPointer]) == 'n')
            {
                string substring = fullJsonString.Substring(stringPointer, 4);
                if (substring.Equals("null", StringComparison.OrdinalIgnoreCase))
                {
                    stringPointer += 4;
                    return null;
                }
            }
            return null;
        }

        private Number ParseNumber()
        {
            int begining = stringPointer;
            while (fullJsonString.Length > stringPointer
                && (fullJsonString[stringPointer] >= '0' && fullJsonString[stringPointer] <= '9' || fullJsonString[stringPointer] == '.'))
            {
                // keep going until you find something other than a number or decimal place.
                stringPointer++;
            }

            string substring = fullJsonString.Substring(begining, stringPointer - begining);

            if (substring.Contains("."))
            {
                if (double.TryParse(substring, out double value))
                {
                    return new Number(value);
                }
            }
            else
            {
                if (int.TryParse(substring, out int value))
                {
                    return new Number(value);
                }
            }

            return new Number();
        }

        private Object ParseObject()
        {
            Object obj = new Object();

            // in the object... so parse out key value pairs

            while (fullJsonString.Length > stringPointer)
            {
                // trim off white space.
                Trim();

                if (fullJsonString[stringPointer] == '}')
                {
                    // found end of Object. so break.
                    break;
                }

                if (fullJsonString[stringPointer] != '"')
                {
                    // we did not find a " so break
                    obj.IsError = true;
                    break;
                }
                // key first
                string key = ParseString();

                Trim();

                // colon (:) should be next character.
                if (fullJsonString[stringPointer] != ':')
                {
                    // we did not find a colon so break
                    obj.IsError = true;
                    break;
                }

                stringPointer++; // move pointer past colon (:)

                object jsonValue = ParseValue();

                Members members = new Members
                {
                    Key = key,
                    Member = jsonValue,
                    IsValid = key.Length > 0
                };

                obj.Add(members);
                obj.IsValid = members.IsValid; // TODO: Might be an issue here with the valid logic for larger sets.

                Trim();

                if (fullJsonString[stringPointer] != ',')
                {
                    // did not find comma (,) so this must be end of the object set.
                    if (fullJsonString[stringPointer] == '}')
                    {
                        stringPointer++; // move pointer past curly right brace (})
                    }
                    else
                    {
                        // did not find end of object, so flag as error
                        obj.IsValid = false;
                        obj.IsError = true;
                    }
                    break;
                }
                else
                {
                    stringPointer++;  // move past the (,) character
                }
            }

            return obj;
        }

        /// <summary>Parses the string that was sent in looking for double quotes at both ends.</summary>
        /// <param name="s">full string that starts with ".</param>
        /// <returns>
        /// returns the string that is between two " but ignoring escaped double quotes (\")
        /// </returns>
        private string ParseString()
        {
            if (fullJsonString[stringPointer] != '"')
            {
                // throw exception?!
            }

            int begining = ++stringPointer;

            // find matching " pair.
            while (fullJsonString.Length > stringPointer)
            {
                if (fullJsonString[stringPointer] == '"' && fullJsonString[stringPointer - 1] != '\\')
                {
                    // found end of string section.
                    break;
                }
                stringPointer++; // keep advancing utnil we find a "
            }

            // i should be either at the end of the string section or end of the string length.
            if (fullJsonString.Length > stringPointer && fullJsonString[stringPointer] == '"')
            {
                // just return the string without the two "s.
                return fullJsonString.Substring(begining, stringPointer++ - begining).Trim();
            }

            return "";
        }

        private object ParseValue()
        {
            Trim();

            char charFirst = fullJsonString[stringPointer];

            // string
            if (charFirst == '"')
            {
                return new String(ParseString());
            }
            // object
            else if (charFirst == '{')
            {
                stringPointer++;
                return ParseObject();
            }
            // array
            else if (charFirst == '[')
            {
                stringPointer++;
                return ParseArray();
            }
            // number
            else if (charFirst >= '0' && charFirst <= '9')
            {
                // we have a number!
                return ParseNumber();
            }
            // boolean
            else if (char.ToLower(charFirst) == 't' || char.ToLower(charFirst) == 'f')
            {
                // we have a number!
                return ParseBoolean();
            }
            // null
            else if (char.ToLower(charFirst) == 'n')
            {
                return ParseNull();
            }

            return null;
        }

        /// <summary>Method to handle trimming any white space.</summary>
        private void Trim()
        {
            while (fullJsonString.Length > stringPointer && char.IsWhiteSpace(fullJsonString[stringPointer]))
            {
                stringPointer++;
            }
        }
    }
}