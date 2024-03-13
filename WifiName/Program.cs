using System;
using ConsoleTables;

namespace WifiName
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                WlanManager wlanManager = new WlanManager();
                wlanManager.GetWifiInfo();

                Console.WriteLine("Wi-Fi with Passwords:");
                var table = new ConsoleTable("WI-FI NAME", "PASSWORD");

                foreach (var wifiInfo in wlanManager.WifiInfoList)
                    table.AddRow(wifiInfo.WifiName, wifiInfo.KeyContent);

                table.Write();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            Console.ReadKey();
        }
    }
}
