// ---------- ---------- ---------- ---------- ---------- ----------
// By: Patrick McDougle
// Class: CSE 681
// Date: Spring of 2022
// ---------- ---------- ---------- ---------- ---------- ----------
using System;

namespace CSE681.Project4.Server
{
    public static class HostServer
    {
        private static ClientServerService _receiverService;

        public static void Main(string[] args)
        {
            Console.Title = "BasicHttp Service Host";
            Console.WriteLine("  Starting Programmatic Basic Service");
            Console.WriteLine(" =====================================");
            string endpoint = "http://localhost:58080/HostServer";

            try
            {
                _receiverService = new ClientServerService();
                _receiverService.CreateRecevedChannel(endpoint);

                Console.WriteLine("  Started ReceiverService - Press key to exit:");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"EXCEPTION: {ex.Message}");
                return;
            }
            _receiverService?.Close();
        }
    }
}