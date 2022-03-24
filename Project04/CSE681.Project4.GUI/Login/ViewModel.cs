// ---------- ---------- ---------- ---------- ---------- ----------
// By: Patrick McDougle
// Class: CSE 681
// Date: Spring of 2022
// ---------- ---------- ---------- ---------- ---------- ----------
using CSE681.Project4.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CSE681.Project4.GUI.Login
{
    public class ViewModel : ObservableObject
    {
        private Guid _guid;
        private string _id;
        private Model _model;
        private string _username;

        public ViewModel(Model model)
        {
            _model = model;

            Random random = new Random();

            ButtonLoginCommand = new RelayCommand(o => ButtonLoginClicked(), p => true);

            _guid = Guid.NewGuid();
            Id = _guid.ToString();
            Username = $"User{random.Next(1000, 9999)}";
        }

        public ICommand ButtonLoginCommand { get; set; }

        public string Id { get => _id; set => SetProperty(ref _id, value); }

        public string Username { get => _username; set => SetProperty(ref _username, value); }

        private void ButtonLoginClicked()
        {
            _model.LogInUser(Username, _guid);
        }
    }
}