// ---------- ---------- ---------- ---------- ---------- ----------
// By: Patrick McDougle
// Class: CSE 681
// Date: Spring of 2022
// ---------- ---------- ---------- ---------- ---------- ----------
using CSE681.Project4.Core.Data;
using CSE681.Project4.GUI.Service.P2P;
using CSE681.Project4.GUI.Service.P2S;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CSE681.Project4.GUI.Service.P2G;

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
            Peer2ServerSendService.Close();
        }

        public UserInformation LoggedUserInformation { get; set; }
        public P2GListenService Peer2GroupListenService { get; set; }
        public P2GSendService Peer2GroupSendService { get; private set; }
        public P2PListenService Peer2PeerListenService { get; set; }
        public SendService Peer2ServerSendService { get; private set; }
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
                    SetupPeer2PeerService();
                    SetupPeer2GroupService();
                    DisplayChatScreen();
                }
            });
        }

        public void SetupServices()
        {
            Peer2ServerSendService = new SendService("http://localhost:58080/HostServer");
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
                P2PListenService = Peer2PeerListenService,
                P2GListenService = Peer2GroupListenService
            };
            Chat.ViewModel viewModel = new Chat.ViewModel(model);
            Chat.View view = new Chat.View(viewModel)
            {
                Height = ChatScreen.Height,
                Width = ChatScreen.Width
            };

            Peer2ServerSendService.LoggedUserInformation = LoggedUserInformation;

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

        private void SetupPeer2GroupService()
        {
            string endPointUrl = $"http://{LoggedUserInformation.Address}/Peer2Group";

            try
            {
                Peer2GroupListenService = new P2GListenService();
                Peer2GroupListenService.CreateRecevedChannel(endPointUrl);
                P2GListenService.SetUserInformationList(Peer2ServerSendService.UserInfoList);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"EXCEPTION: {ex.Message}");
            }
        }

        private void SetupPeer2PeerService()
        {
            string endPointUrl = $"http://{LoggedUserInformation.Address}/Peer2Peer";

            try
            {
                Peer2PeerListenService = new P2PListenService();
                Peer2PeerListenService.CreateRecevedChannel(endPointUrl);
                P2PListenService.SetUserInformationList(Peer2ServerSendService.UserInfoList);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"EXCEPTION: {ex.Message}");
            }
        }
    }
}