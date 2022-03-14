// ---------- ---------- ---------- ---------- ---------- ----------
// By: Patrick McDougle
// Class: CSE 681
// Date: Spring of 2022
// ---------- ---------- ---------- ---------- ---------- ----------
using CSE681.JSON.DOMs;
using System.Linq;

namespace CSE681.JSON.Search
{
    /// <summary>
    /// This class will take in a CSE681 JSON DOMs tree and search for the given key. It will
    /// remember the last key it found and return the next key in the JSON DOMs tree.
    /// </summary>
    public class Searcher
    {
        private object _alreadyFound;
        private object _domTree;

        public Searcher()
        { }

        /// <summary>This method will look for the next key in the JSON DOMs tree.</summary>
        /// <param name="key">The key to look for.</param>
        /// <returns>
        /// The method will return an object that references the DOMs element that has this key.
        /// </returns>
        public object LookingFor(string key, bool searchFullMatch)
        {
            object found = null;

            if (_domTree is Object obj)
            {
                found = Search(obj, key.ToLower(), searchFullMatch);
                if (found == null)
                {
                    // search one more time.
                    found = Search(obj, key.ToLower(), searchFullMatch);
                }
            }
            else if (_domTree is Array array)
            {
                found = Search(array, key.ToLower(), searchFullMatch);
                if (found == null)
                {
                    // search one more time.
                    found = Search(array, key.ToLower(), searchFullMatch);
                }
            }
            return found;
        }

        /// <summary>
        /// This method allows for the searcher to set the key elemnt that has already been found so
        /// that the search will skip all the other keys until it finds this one and then search for
        /// the next occurance of the key.
        /// </summary>
        /// <param name="foundValue">The previous found key element.</param>
        /// <returns>
        /// A Reference to this Searcher to allow for method stacking or whatever it is called.
        /// </returns>
        public Searcher SetAlreadyFound(object foundValue)
        {
            _alreadyFound = foundValue;
            return this;
        }

        /// <summary>
        /// This method allows the seracher to set the CSE681 JSON DOMs tree element that should be searched.
        /// </summary>
        /// <param name="value">The CSE681 JSON DOMs tree root element.</param>
        /// <returns>
        /// A Reference to this Searcher to allow for method stacking or whatever it is called.
        /// </returns>
        public Searcher SetDom(object value)
        {
            _domTree = value;
            return this;
        }

        private object Search(Array value, string key, bool searchFullMatch)
        {
            foreach (object jsonValue in value.TheValue)
            {
                if (jsonValue is Object obj)
                {
                    object jv = Search(obj, key, searchFullMatch);

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
                    object jv = Search(arr, key, searchFullMatch);

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

        private object Search(Object value, string key, bool searchFullMatch)
        {
            foreach (Members members in value.TheValue)
            {
                if (searchFullMatch ? members.Key.ToLower().Equals(key) : members.Key.ToLower().Contains(key))
                {
                    if (_alreadyFound != null)
                    {
                        if (members == _alreadyFound)
                        {
                            _alreadyFound = null;
                        }
                    }
                    else
                    {
                        return members;
                    }
                }
                object jsonValue = Search(members.Member, key, searchFullMatch);
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

        private object Search(object value, string key, bool searchFullMatch)
        {
            if (value is Object obj)
            {
                return Search(obj, key, searchFullMatch);
            }
            if (value is Array array)
            {
                return Search(array, key, searchFullMatch);
            }

            return null;
        }
    }
}