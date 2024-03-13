using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace WifiName
{
    internal class WlanManager
    {
        public List<WlanInfo> WifiInfoList { get; private set; }

        public WlanManager()
        {
            WifiInfoList = new List<WlanInfo>();
        }

        public void GetWifiInfo()
        {
            List<string> wifiNames = GetWifiNames();

            foreach (var wifiName in wifiNames)
            {
                string keyContent = GetKeyContent(wifiName);
                if (!string.IsNullOrEmpty(keyContent))
                    WifiInfoList.Add(new WlanInfo { WifiName = wifiName, KeyContent = keyContent });
            }
        }

        private List<string> GetWifiNames()
        {
            Process process = CreateProcess("wlan show profiles");
            string output = RunProcess(process);
            return ExtractWifiNames(output);
        }

        private string GetKeyContent(string wifiName)
        {
            Process process = CreateProcess($"wlan show profiles {wifiName} key=clear");
            string output = RunProcess(process);
            return ExtractKeyContent(output);
        }

        private Process CreateProcess(string arguments)
        {
            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "netsh",
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            return process;
        }

        private string RunProcess(Process process)
        {
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return output;
        }

        private string ExtractKeyContent(string netshOutput)
        {
            Regex regex = new Regex(@"\s*Key Content\s*:\s*(.*)\s*", RegexOptions.IgnoreCase);
            Match match = regex.Match(netshOutput);

            return match.Success ? match.Groups[1].Value.Trim() : string.Empty;
        }

        private List<string> ExtractWifiNames(string netshOutput)
        {
            List<string> wifiNames = new List<string>();
            Regex regex = new Regex(@"\s*All User Profile\s*:\s*(.*)\s*", RegexOptions.IgnoreCase);

            string[] lines = netshOutput.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                Match match = regex.Match(line);
                if (match.Success)
                    wifiNames.Add(match.Groups[1].Value.Trim());
            }

            return wifiNames;
        }
    }
}
