using System;
using System.IO;
using System.Threading;

namespace TwitchDropFarmBot
{
    class ChangeSettings
    {
        public static void ChangeProgramSettings()
        {
            //all of this could have been better but it just works
            Console.Clear();
            //Program.cfg = Functions.LoadConfig();
            Console.WriteLine(
                "Current Settings" +
                "\n- Client ID: " + Program.cfg.client_id.ToString() +
                "\n- Client Secret: " + Program.cfg.client_secret.ToString() +
                "\n- Access Token (OAuth): " + Program.cfg.access_token.ToString() +
                "\n- Auto Open Stream (This will open a new browser and take focus): " + Program.cfg.auto_open_stream +
                "\n- Auto Close Stream (This will close the whole browser): " + Program.cfg.auto_close_stream +
                "\n- Browser Process Name: " + Program.cfg.browser_proc_name
            );
            Console.WriteLine();
            Console.WriteLine(
                "\n1) Client ID" +
                "\n2) Client Secret" +
                "\n3) Access Token (OAuth)" +
                "\n4) Auto Open Stream" +
                "\n5) Auto Close Browser" +
                "\n6) Browser Process Name" +
                "\n7) Encrypt Config Values/Save Config" +
                "\n8) Decrypt Config Values (use only if you want to see the values)" + 
                "\n9) Reload Config" +
                "\n99) Back"
            );
            Console.Write(":");
            Console.Write("");
            var choice = Console.ReadLine();
            if (choice == 1.ToString())
            {
                Console.Clear();
                Console.Write("Enter new Client ID: ");
                var new_client_id = Console.ReadLine();
                Program.cfg.client_id = new_client_id.Trim();
                Console.WriteLine("Client ID Changed.");
                Thread.Sleep(1000);
                ChangeProgramSettings();
            }
            else if (choice == 2.ToString())
            {
                Console.Clear();
                Console.Write("Enter new Client Secret: ");
                var new_client_secret = Console.ReadLine();
                Program.cfg.client_secret = new_client_secret.Trim();
                Console.WriteLine("Client Secret Changed.");
                Thread.Sleep(1000);
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
                Program.cfg.auto_open_stream = !(bool)Program.cfg.auto_open_stream;
                ChangeProgramSettings();
            }
            else if (choice == 5.ToString())
            {
                Console.Clear();
                Program.cfg.auto_close_stream = !(bool)Program.cfg.auto_close_stream;
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
                Program.cfg.browser_proc_name = browser_process_name.Trim();
                ChangeProgramSettings();
            }
            else if (choice == 7.ToString())
            {
                Functions.SaveConfig();
                Console.WriteLine("Config saved/Encrypted.");
                Thread.Sleep(1000);
                ChangeProgramSettings();
            }
            else if (choice == 8.ToString())
            {
                Program.cfg.client_id = Functions.DecryptString(Program.cfg.client_id.ToString());
                Program.cfg.client_secret = Functions.DecryptString(Program.cfg.client_secret.ToString(), false);
                Program.cfg.access_token = Functions.DecryptString(Program.cfg.access_token.ToString(), false);
                
                ChangeProgramSettings();
            }
            else if (choice == 9.ToString())
            {
                Program.cfg = Functions.LoadConfig();
                ChangeProgramSettings();
            }
            else if (choice == 99.ToString())
            {
                Console.Clear();
                Program.Main();
            }
            else
            {
                Console.WriteLine("Invalid option.");
                Thread.Sleep(1000);
                Console.Clear();
                ChangeProgramSettings();
            }
        }
    }
}
