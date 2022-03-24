// ---------- ---------- ---------- ---------- ---------- ----------
// By: Patrick McDougle
// Class: CSE 681
// Date: Spring of 2022
// ---------- ---------- ---------- ---------- ---------- ----------
using CSE681.Project4.Data;
using CSE681.Project4.GUI.P2P;
using CSE681.Project4.GUI.Service;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace CSE681.Project4.GUI
{
    /// <summary>Interaction logic for MainWindow.xaml</summary>
    public partial class MainWindow : Window
    {
        private Login.Model _loginModel;
        private IProgress<(int, int)> _progressSize;
        private IProgress<string> _progressTitle;
        private IProgress<(Visibility, Visibility)> _progressVisibility;

        public MainWindow()
        {
            InitializeComponent();
            SetupProgressCalls();
            SetupServices();

            Random = new Random();

            Task.Run(() =>
            {
                Thread.Sleep(2000);
                TransitionToLoginWindow();
            });
        }

        ~MainWindow()
        {
            ClientToServer.Close();
        }

        public ClientToServer ClientToServer { get; private set; }
        public UserInformation LoggedUserInformation { get; set; }
        public P2PListenService P2PListenService { get; set; }
        public Random Random { get; private set; }

        public void SetupProgressCalls()
        {
            _progressSize = new Progress<(int, int)>(wh =>
            {
                Height = wh.Item1;
                Width = wh.Item2;
            });

            _progressTitle = new Progress<string>(s =>
            {
                Title = s;
            });

            _progressVisibility = new Progress<(Visibility, Visibility)>(v =>
            {
                if (v.Item1 == Visibility.Hidden && v.Item2 == Visibility.Hidden)
                {
                    SplashScreen.Visibility = Visibility.Visible;
                    LoginScreen.Visibility = Visibility.Hidden;
                }
                if (v.Item1 == Visibility.Visible && v.Item2 == Visibility.Hidden)
                {
                    DisplayLoginScreen();
                }
                if (v.Item1 == Visibility.Hidden && v.Item2 == Visibility.Visible)
                {
                    SetupPeep2PeerService();
                    DisplayChatScreen();
                }
            });
        }

        public void SetupServices()
        {
            ClientToServer = new ClientToServer("http://localhost:58080/HostServer");
        }

        public void TransitionToChatWindow(UserInformation loggedUserInformation)
        {
            LoggedUserInformation = loggedUserInformation;
            _progressTitle.Report($"2P-Mess : Chat Window - {LoggedUserInformation.Name} [{LoggedUserInformation.Address}] [{LoggedUserInformation.Id}]");
            _progressVisibility.Report((Visibility.Hidden, Visibility.Visible));
            _progressSize.Report((600, 900));
        }

        public void TransitionToLoginWindow()
        {
            _progressTitle.Report("2P-Mess : Login");
            _progressSize.Report((600, 450));
            _progressVisibility.Report((Visibility.Visible, Visibility.Hidden));
        }

        private void DisplayChatScreen()
        {
            SplashScreen.Visibility = Visibility.Hidden;
            LoginScreen.Visibility = Visibility.Hidden;
            ChatScreen.Visibility = Visibility.Visible;

            Chat.Model model = new Chat.Model(this)
            {
                LoggedUserInformation = LoggedUserInformation,
                P2PListenService = P2PListenService
            };
            Chat.ViewModel viewModel = new Chat.ViewModel(model);
            Chat.View view = new Chat.View(viewModel)
            {
                Height = ChatScreen.Height,
                Width = ChatScreen.Width
            };

            ChatScreen.Children.Add(view);
        }

        private void DisplayLoginScreen()
        {
            SplashScreen.Visibility = Visibility.Hidden;
            LoginScreen.Visibility = Visibility.Visible;
            ChatScreen.Visibility = Visibility.Hidden;

            _loginModel = new Login.Model(this);
            Login.ViewModel viewModel = new Login.ViewModel(_loginModel);
            Login.View view = new Login.View(viewModel)
            {
                Height = LoginScreen.Height,
                Width = LoginScreen.Width
            };

            LoginScreen.Children.Add(view);
        }

        private void SetupPeep2PeerService()
        {
            string endPointUrl = $"http://{LoggedUserInformation.Address}/Peer2Peer";
            //string endPointUrl = $"http://localhost:{LoggedUserInformation.Address.Port}/Peer2Peer";

            try
            {
                P2PListenService = new P2PListenService();
                P2PListenService.CreateRecevedChannel(endPointUrl);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"EXCEPTION: {ex.Message}");
            }
        }
    }
}