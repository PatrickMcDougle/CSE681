using CSE681.JSON.DOMs;
using System.Linq;

namespace CSE681.JSON.Search
{
    public class Searcher
    {
        private Value _domTree;

        public Searcher()
        { }

        public Value LookingFor(string key)
        {
            if (_domTree is Object obj)
            {
                return Search(obj, key);
            }
            if (_domTree is Array array)
            {
                return Search(array, key);
            }
            return null;
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
                    if (jv != null)
                    {
                        return jv;
                    }
                }
                if (jsonValue is Array arr)
                {
                    Value jv = Search(arr, key);

                    if (jv != null)
                    {
                        return jv;
                    }
                }
            }
            return null;
        }

        private Value Search(Object value, string key)
        {
            Members foundKeyValueSet = value.Properties.FirstOrDefault(x => x.Key.Equals(key));
            if (foundKeyValueSet != null)
            {
                return foundKeyValueSet.Member;
            }
            // did not find key at this level, move on to next level.
            foreach (Members keyValueSet in value.Properties)
            {
                if (keyValueSet.Key.Equals(key))
                {
                    return keyValueSet.Member;
                }
                Value jsonValue = Search(keyValueSet.Member, key);
                if (jsonValue != null)
                {
                    return jsonValue;
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