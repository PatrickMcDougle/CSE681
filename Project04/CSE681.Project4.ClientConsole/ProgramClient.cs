// ---------- ---------- ---------- ---------- ---------- ----------
// By: Patrick McDougle
// Class: CSE 681
// Date: Spring of 2022
// ---------- ---------- ---------- ---------- ---------- ----------
using CSE681.Project4.Data;
using CSE681.Project4.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSE681.Project4.ClientConsole
{
    public class ProgramClient
    {
        private static void Main(string[] args)
        {
            Console.Title = "BasicHttp Client";
            Console.WriteLine("  Starting Programmatic Basic Service Client");
            Console.WriteLine(" ============================================");
            string url = "http://localhost:58080/HostServer";

            ClientService senderService = new ClientService(url);
            try
            {
                Random random = new Random();

                UserInformation me = new UserInformation()
                {
                    Name = $"TestMan{random.Next(10000, 99999)}",
                    Id = Guid.NewGuid(),
                    Address = new IpAddress()
                    {
                        Address = IpAddress.GetCurrentAddress(),
                        Port = (uint)random.Next(50_000, 60_000),
                    },
                    IsActive = true
                };

                senderService.SendInitialInfo(me);

                Console.WriteLine("  Started ReceiverService - Press key to exit:");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"EXCEPTION: {ex.Message}");
                return;
            }
            senderService.Close();
        }
    }
}