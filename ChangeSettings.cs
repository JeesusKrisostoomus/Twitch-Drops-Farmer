using System;
using System.Threading;

namespace TwitchDropFarmBot
{
    class ChangeSettings
    {
        public static void ChangeProgramSettings()
        {
            //all of this could have been better but it just works

            Console.Clear();
            Console.WriteLine(
                "Current Settings" +
                "\n- Client ID: " + Properties.Settings.Default.client_id +
                "\n- Client Secret: " + Properties.Settings.Default.client_secret +
                "\n- Access Token (OAuth): " + Properties.Settings.Default.access_token +
                "\n- Auto Open Stream (This will open a new browser and take focus): " + Properties.Settings.Default.auto_open +
                "\n- Auto Close Stream (This will close the whole browser): " + Properties.Settings.Default.auto_close +
                "\n- Browser Process Name: " + Properties.Settings.Default.browser_procname
            );
            Console.WriteLine();
            Console.WriteLine(
                "\n1) Client ID" +
                "\n2) Client Secret" +
                "\n3) Access Token (OAuth)" +
                "\n4) Auto Open Stream" +
                "\n5) Auto Close Browser" +
                "\n6) Browser Process Name" +
                "\n10) Back"
            );
            Console.Write(":");
            Console.Write("");
            var choice = Console.ReadLine();
            if (choice == 1.ToString())
            {
                Console.Clear();
                Console.Write("Enter new Client ID: ");
                var new_client_id = Console.ReadLine();
                Properties.Settings.Default.client_id = new_client_id.Trim();
                Properties.Settings.Default.Save();
                ChangeProgramSettings();
            }
            else if (choice == 2.ToString())
            {
                Console.Clear();
                Console.Write("Enter new Client Secret: ");
                var new_client_secret = Console.ReadLine();
                Properties.Settings.Default.client_secret = new_client_secret.Trim();
                Properties.Settings.Default.Save();
                ChangeProgramSettings();
            }
            else if (choice == 3.ToString())
            {
                Console.Clear();
                GetOAuth.MainFunc();
                ChangeProgramSettings();
            }
            else if (choice == 4.ToString())
            {
                Console.Clear();
                Properties.Settings.Default.auto_open = !Properties.Settings.Default.auto_open;
                Properties.Settings.Default.Save();
                ChangeProgramSettings();
            }
            else if (choice == 5.ToString())
            {
                Console.Clear();
                Properties.Settings.Default.auto_close = !Properties.Settings.Default.auto_close;
                Properties.Settings.Default.Save();
                ChangeProgramSettings();
            }
            else if (choice == 6.ToString())
            {
                Console.Clear();
                Console.WriteLine(
                    "Examples" +
                    "\nOpera - opera" +
                    "\nGoogle Chrome - chrome" +
                    "\nFirefox - firefox" +
                    "\nMicrosoft Edge - MicrosoftEdge" +
                    "\nInternet Explorer - iexplore"
                );
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Be sure that the name is correct. It will try killing anything you told it to.");
                Console.ResetColor();
                Console.Write("Enter browser process name: ");
                var browser_process_name = Console.ReadLine();
                Properties.Settings.Default.browser_procname = browser_process_name.Trim();
                Properties.Settings.Default.Save();
                ChangeProgramSettings();
            }
            else if (choice == 10.ToString())
            {
                Console.Clear();
                Program.Main();
            }
            else
            {
                Console.WriteLine("This option is not possible.");
                Thread.Sleep(2000);
                Console.Clear();
                ChangeProgramSettings();
            }
        }
    }
}
