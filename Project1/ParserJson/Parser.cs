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
    public class Parser
    {
        private readonly string fullJsonString;

        private int stringPointer = 0;

        public Parser(string jsonString)
        {
            fullJsonString = jsonString.Trim(); // trim off any white space characters.
        }

        public Value GetJsonValue()
        {
            if (!string.IsNullOrWhiteSpace(fullJsonString))
            {
                char charFirst = fullJsonString[stringPointer];
                char charLast = fullJsonString[fullJsonString.Length - 1];

                // JSON string needs to be an object or an array. Nothing else
                if (charFirst == '{' && charLast == '}')
                {
                    stringPointer++;
                    return ParseObject();
                }

                if (charFirst == '[' && charLast == ']')
                {
                    stringPointer++;
                    return ParseArray();
                }
            }

            // the file did not start with an object or an array. That means it was not valid, so
            // just return a blank object.
            return new Object();
        }

        private Value ParseArray()
        {
            Array array = new Array();

            // in the array... so parse out values

            while (fullJsonString.Length > stringPointer)
            {
                Value jsonValue = ParseValue();

                array.Add(jsonValue);
                array.IsValid = jsonValue == null || jsonValue.IsValid; // TODO: Might be an issue here with the valid logic for larger sets.

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

        private Value ParseBoolean()
        {
            if (Char.ToLower(fullJsonString[stringPointer]) == 't')
            {
                string substring = fullJsonString.Substring(stringPointer, 4);
                if (substring.Equals("true", StringComparison.OrdinalIgnoreCase))
                {
                    stringPointer += 4;
                    return new Boolean(true);
                }
            }
            else if (Char.ToLower(fullJsonString[stringPointer]) == 'f')
            {
                string substring = fullJsonString.Substring(stringPointer, 5);
                if (substring.Equals("false", StringComparison.OrdinalIgnoreCase))
                {
                    stringPointer += 5;
                    return new Boolean(false);
                }
            }

            return new Boolean();
        }

        private Value ParseNull()
        {
            if (Char.ToLower(fullJsonString[stringPointer]) == 'n')
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

        private Value ParseNumber()
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
                if (Double.TryParse(substring, out double value))
                {
                    return new Number(value);
                }
            }
            else
            {
                if (Int32.TryParse(substring, out int value))
                {
                    return new Number(value);
                }
            }

            return new Number();
        }

        private Value ParseObject()
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

                Value jsonValue = ParseValue();

                Members keyValueSet = new Members
                {
                    Key = key,
                    Member = jsonValue,
                    IsValid = jsonValue.IsValid && key.Length > 0
                };

                obj.Add(keyValueSet);
                obj.IsValid = keyValueSet.IsValid; // TODO: Might be an issue here with the valid logic for larger sets.

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

        private Value ParseValue()
        {
            Trim();

            // string
            if (fullJsonString[stringPointer] == '"')
            {
                string value = ParseString();

                return new String(value);
            }
            // object
            else if (fullJsonString[stringPointer] == '{')
            {
                stringPointer++;
                return ParseObject();
            }
            // array
            else if (fullJsonString[stringPointer] == '[')
            {
                stringPointer++;
                return ParseArray();
            }
            // number
            else if (fullJsonString[stringPointer] >= '0' && fullJsonString[stringPointer] <= '9')
            {
                // we have a number!
                return ParseNumber();
            }
            // boolean
            else if (Char.ToLower(fullJsonString[stringPointer]) == 't' || Char.ToLower(fullJsonString[stringPointer]) == 'f')
            {
                // we have a number!
                return ParseBoolean();
            }
            // null
            else if (Char.ToLower(fullJsonString[stringPointer]) == 'n')
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