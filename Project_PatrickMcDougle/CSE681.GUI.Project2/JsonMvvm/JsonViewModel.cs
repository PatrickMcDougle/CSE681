// ---------- ---------- ---------- ---------- ---------- ----------
// By: Patrick McDougle
// Class: CSE 681
// Date: Spring of 2022
// ---------- ---------- ---------- ---------- ---------- ----------
using CSE681.Support;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CSE681.GUI.Project2
{
    public class JsonViewModel : ObservableObject
    {
        private readonly JsonModel _model;
        private string _fileName;
        private string _filePath;
        private string _insertJsonText;
        private Visibility _insertSuccessful = Visibility.Hidden;
        private string _progressMessage;
        private int _progressPercent;
        private bool _searchFullMatch;
        private string _searchText;
        private ObservableCollection<TreeViewItem> _subTreeViewItemList;
        private ObservableCollection<TreeViewItem> _treeViewItemList;

        public JsonViewModel(JsonModel model)
        {
            _model = model;

            _model.PropertyChanged += OnModelPropertyChanged;

            FileName = Path.GetFileName(_model.FilePathAndName);
            FilePath = Path.GetDirectoryName(_model.FilePathAndName);

            ButtonLoadCommand = new RelayCommand(o => ButtonLoadClicked(), p => true);
            ButtonSaveCommand = new RelayCommand(o => ButtonSaveClicked(), p => true);
            ButtonSearchCommand = new RelayCommand(o => ButtonSearchClicked(), p => true);
            ButtonUndoSearchCommand = new RelayCommand(o => ButtonUndoSearchClicked(), p => true);
            ButtonAddToSubTreeCommand = new RelayCommand(o => ButtonAddToSubTreeClicked(), p => true);
        }

        public ICommand ButtonAddToSubTreeCommand { get; set; }
        public ICommand ButtonLoadCommand { get; set; }
        public ICommand ButtonSaveCommand { get; set; }
        public ICommand ButtonSearchCommand { get; set; }
        public ICommand ButtonUndoSearchCommand { get; set; }
        public string FileName { get => _fileName; set => SetProperty(ref _fileName, value); }
        public string FilePath { get => _filePath; set => SetProperty(ref _filePath, value); }
        public string InsertJsonText { get => _insertJsonText; set => SetProperty(ref _insertJsonText, value); }
        public Visibility InsertSuccessful { get => _insertSuccessful; set => SetProperty(ref _insertSuccessful, value); }
        public string ProgressMessage { get => _progressMessage; set => SetProperty(ref _progressMessage, value); }
        public int ProgressPercent { get => _progressPercent; set => SetProperty(ref _progressPercent, value); }
        public bool SearchFullMatch { get => _searchFullMatch; set => SetProperty(ref _searchFullMatch, value); }
        public string SearchText { get => _searchText; set => SetProperty(ref _searchText, value); }
        public ObservableCollection<TreeViewItem> SubTreeViewItemList { get => _subTreeViewItemList; set => SetProperty(ref _subTreeViewItemList, value); }
        public ObservableCollection<TreeViewItem> TreeViewItemList { get => _treeViewItemList; set => SetProperty(ref _treeViewItemList, value); }
        private ConstructTreeView ConstructTreeView { get; set; } = new ConstructTreeView();

        private void ButtonAddToSubTreeClicked()
        {
            _model.JsonTextToInsert = InsertJsonText;
            _model.InsertJsonTextIntoSubTree();
        }

        private void ButtonLoadClicked()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "JSON Files (*.json;*.txt)|*.json;*.txt|All files (*.*)|*.*",
                Title = "Open JSON File",
                InitialDirectory = FilePath
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _model.FilePathAndName = openFileDialog.FileName;
                _model.OpenJsonFile();
            }
        }

        private void ButtonSaveClicked()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "JSON Files (*.json;*.txt)|*.json;*.txt|All files (*.*)|*.*",
                Title = "Save JSON File",
                InitialDirectory = FilePath
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                _model.FilePathAndName = saveFileDialog.FileName;
                _model.SaveJsonTree();
            }
        }

        private void ButtonSearchClicked()
        {
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                _model.SearchTreeFor(SearchText, SearchFullMatch);
            }
        }

        private void ButtonUndoSearchClicked()
        {
            _model.UndoSearch();
        }

        private void OnModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(_model.Tree):
                    TreeViewItemList = new ObservableCollection<TreeViewItem>
                    {
                        ConstructTreeView.Build(_model.Tree)
                    };
                    break;

                case nameof(_model.SubTree):
                    if (SubTreeViewItemList != null)
                    {
                        ConstructTreeView.UnsetFoundTreeViewItem(TreeViewItemList.First(), SubTreeViewItemList.First().Uid);
                    }

                    if (_model.SubTree != null)
                    {
                        SubTreeViewItemList = new ObservableCollection<TreeViewItem>
                        {
                            ConstructTreeView.Build(_model.SubTree)
                        };
                        ConstructTreeView.SetFoundTreeViewItem(TreeViewItemList.First(), SubTreeViewItemList.First().Uid);
                        SubTreeViewItemList.First().IsExpanded = true;

                        SubTreeViewItemList.First().Items
                            .Cast<TreeViewItem>()
                            .ToList()
                            .ForEach(x => x.IsExpanded = true);
                    }
                    else
                    {
                        SubTreeViewItemList.Clear();
                        SubTreeViewItemList = null;
                    }
                    break;

                case nameof(_model.FilePathAndName):
                    FileName = Path.GetFileName(_model.FilePathAndName);
                    FilePath = Path.GetDirectoryName(_model.FilePathAndName);
                    break;

                case nameof(_model.ProgressPercent):
                    ProgressPercent = _model.ProgressPercent;
                    break;

                case nameof(_model.ProgressMessage):
                    ProgressMessage = _model.ProgressMessage;
                    break;

                case nameof(_model.InsertSuccessful):
                    InsertSuccessful = _model.InsertSuccessful ? Visibility.Hidden : Visibility.Visible;
                    break;
            }
        }
    }
}