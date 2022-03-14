// ---------- ---------- ---------- ---------- ---------- ----------
// By: Patrick McDougle
// Class: CSE 681
// Date: Spring of 2022
// ---------- ---------- ---------- ---------- ---------- ----------
using CSE681.JSON.DOMs;
using CSE681.JSON.Parse;
using CSE681.JSON.PrettyPrint;
using CSE681.JSON.Search;
using CSE681.Support;
using System.Collections.Generic;
using System.IO;

namespace CSE681.GUI.Project2
{
    public class JsonModel : ObservableObject
    {
        public static readonly string DEFAULT_PATH = @"C:\Users\Cland\OneDrive - Syracuse University\681 - CSE - Software Modeling & Analysis\Projects\01";

        private string _filePathAndName;
        private bool _insertSuccesful = true;
        private string _jsonTextToInsert;
        private string _lastSearchText;
        private string _progressMessage;
        private int _progressPercent;
        private string _searchName;
        private object _subTree;
        private object _tree;

        public JsonModel()
        {
            FilePathAndName = $"{DEFAULT_PATH}\\04 - one line.json";
            SubTreeHistoryList = new List<object>();
        }

        public string FilePathAndName { get => _filePathAndName; set => SetProperty(ref _filePathAndName, value); }
        public bool InsertSuccessful { get => _insertSuccesful; set => SetProperty(ref _insertSuccesful, value); }
        public string JsonTextToInsert { get => _jsonTextToInsert; set => SetProperty(ref _jsonTextToInsert, value); }
        public string ProgressMessage { get => _progressMessage; set => SetProperty(ref _progressMessage, value); }
        public int ProgressPercent { get => _progressPercent; set => SetProperty(ref _progressPercent, value); }
        public string SearchName { get => _searchName; set => SetProperty(ref _searchName, value); }
        public object SubTree { get => _subTree; set => SetProperty(ref _subTree, value); }
        public List<object> SubTreeHistoryList { get; set; }
        public object Tree { get => _tree; set => SetProperty(ref _tree, value); }
        private Searcher Searcher { get; set; } = new Searcher();

        internal void InsertJsonTextIntoSubTree()
        {
            Parser jsonParser = new Parser(JsonTextToInsert);

            if (SubTree is Members members)
            {
                if (members.Member is JSON.DOMs.Object obj)
                {
                    Members mem = jsonParser.GetJsonMembers();
                    if (mem != null)
                    {
                        obj.Add(mem);
                    }
                    InsertSuccessful = mem != null;
                }
                else if (members.Member is JSON.DOMs.Array arr)
                {
                    object mem = jsonParser.GetJsonValue();
                    if (mem != null)
                    {
                        arr.Add(mem);
                    }
                    InsertSuccessful = mem != null;
                }
                else
                {
                    InsertSuccessful = false;
                }

                OnPropertyChanged(nameof(Tree));
                OnPropertyChanged(nameof(SubTree));
            }
        }

        internal void OpenJsonFile()
        {
            ProgressMessage = "Loading ...";
            ProgressPercent = 0;
            string theText = System.IO.File.ReadAllText(FilePathAndName);

            ProgressPercent = 33;
            Parser jsonParser = new Parser(theText);

            ProgressPercent = 66;
            Tree = jsonParser.GetJsonValue();

            ProgressPercent = 100;
            ProgressMessage = $"Done Loading {Path.GetFileName(FilePathAndName)}";
        }

        internal void SaveJsonTree()
        {
            ProgressMessage = "Saving ...";
            ProgressPercent = 0;
            PrettyPrinter printer = new PrettyPrinter();

            ProgressPercent = 33;
            string theText = printer.PrettyPrintDOM(Tree);

            ProgressPercent = 66;
            System.IO.File.WriteAllText(FilePathAndName, theText);

            ProgressPercent = 100;
            ProgressMessage = $"Done Saving {Path.GetFileName(FilePathAndName)}";
        }

        internal void SearchTreeFor(string searchText, bool searchFullMatch)
        {
            if (_lastSearchText != searchText)
            {
                // the search text has changed so reset FoundValue.
                SubTree = null;
            }

            _lastSearchText = searchText;

            object jsonValue = Searcher
                 .SetDom(_tree)
                 .SetAlreadyFound(SubTree)
                 .LookingFor(searchText, searchFullMatch);

            if (jsonValue != null)
            {
                SubTree = jsonValue;
                SubTreeHistoryList.Add(jsonValue);
            }
            else
            {
                SubTree = null;
            }
        }

        internal void UndoSearch()
        {
            if (SubTreeHistoryList.Count > 1)
            {
                SubTree = SubTreeHistoryList[SubTreeHistoryList.Count - 2];
                SubTreeHistoryList.RemoveAt(SubTreeHistoryList.Count - 1);
            }
            else
            {
                SubTreeHistoryList.Clear();
                SubTree = null;
            }
        }

        private string GetUuid(object subTree)
        {
            if (subTree is IDomTree val)
            {
                return val.UUID.ToString();
            }
            return null;
        }
    }
}