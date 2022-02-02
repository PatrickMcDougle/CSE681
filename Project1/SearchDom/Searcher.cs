// ---------- ---------- ---------- ---------- ---------- ----------
// By: Patrick McDougle
// Class: CSE 681
// Date: Spring of 2022
// ---------- ---------- ---------- ---------- ---------- ----------
using CSE681.JSON.DOMs;
using System.Linq;

namespace CSE681.JSON.Search
{
    public class Searcher
    {
        private Value _alreadyFound;
        private Value _domTree;

        public Searcher()
        { }

        public Value LookingFor(string key)
        {
            Value found = null;

            if (_domTree is Object obj)
            {
                found = Search(obj, key.ToLower());
                if (found == null)
                {
                    // search one more time.
                    found = Search(obj, key.ToLower());
                }
            }
            else if (_domTree is Array array)
            {
                found = Search(array, key.ToLower());
                if (found == null)
                {
                    // search one more time.
                    found = Search(array, key.ToLower());
                }
            }
            return found;
        }

        public Searcher SetAlreadyFound(Value foundValue)
        {
            _alreadyFound = foundValue;
            return this;
        }

        public Searcher SetDom(Value value)
        {
            _domTree = value;
            return this;
        }

        private Value Search(Array value, string key)
        {
            foreach (Value jsonValue in value.Items)
            {
                if (jsonValue is Object obj)
                {
                    Value jv = Search(obj, key);

                    if (_alreadyFound != null)
                    {
                        if (jv == _alreadyFound)
                        {
                            _alreadyFound = null;
                        }
                    }
                    else
                    {
                        if (jv != null)
                        {
                            return jv;
                        }
                    }
                }
                if (jsonValue is Array arr)
                {
                    Value jv = Search(arr, key);

                    if (_alreadyFound != null)
                    {
                        if (jv == _alreadyFound)
                        {
                            _alreadyFound = null;
                        }
                    }
                    else
                    {
                        if (jv != null)
                        {
                            return jv;
                        }
                    }
                }
            }
            return null;
        }

        private Value Search(Object value, string key)
        {
            Members foundMembers = value.Properties.FirstOrDefault(x => x.Key.ToLower().Equals(key));
            if (foundMembers != null)
            {
                if (_alreadyFound != null)
                {
                    if (foundMembers.Member == _alreadyFound)
                    {
                        _alreadyFound = null;
                    }
                }
                else
                {
                    return foundMembers.Member;
                }
            }
            // did not find key at this level, move on to next level.
            foreach (Members members in value.Properties)
            {
                Value jsonValue = Search(members.Member, key);
                if (_alreadyFound != null)
                {
                    if (jsonValue == _alreadyFound)
                    {
                        _alreadyFound = null;
                    }
                }
                else
                {
                    if (jsonValue != null)
                    {
                        return jsonValue;
                    }
                }
            }
            return null;
        }

        private Value Search(Value value, string key)
        {
            if (value is Object obj)
            {
                return Search(obj, key);
            }
            if (value is Array array)
            {
                return Search(array, key);
            }

            return null;
        }
    }
}