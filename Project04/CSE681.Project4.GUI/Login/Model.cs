// ---------- ---------- ---------- ---------- ---------- ----------
// By: Patrick McDougle
// Class: CSE 681
// Date: Spring of 2022
// ---------- ---------- ---------- ---------- ---------- ----------
using CSE681.Project4.Data;
using CSE681.Project4.Utilities;
using System;

namespace CSE681.Project4.GUI.Login
{
    public class Model : ObservableObject
    {
        private readonly Service.ClientToServer _clientToServer;
        private readonly MainWindow _mainWindow;
        private readonly Random _random;

        public Model(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            _random = _mainWindow.Random;
            _clientToServer = _mainWindow.ClientToServer;
        }

        public UserInformation LoggedUserInformation { get; set; }

        public void LogInUser(string username, Guid guid)
        {
            try
            {
                LoggedUserInformation = new UserInformation()
                {
                    Name = username,
                    Id = guid,
                    Address = new IpAddress()
                    {
                        Address = 2130706433, // 2130706433 = 127.0.0.1 //IpAddress.GetCurrentAddress(),
                        Port = (uint)_random.Next(50_000, 60_000),
                    },
                    IsActive = true
                };
                _clientToServer.SendInitialInfo(LoggedUserInformation);

                _mainWindow.TransitionToChatWindow(LoggedUserInformation);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"EXCEPTION: {ex.Message}");
            }
        }
    }
}