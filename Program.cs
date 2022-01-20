using System;
using System.Threading;
using System.IO;

namespace TwitchDropFarmBot
{
    class Program
    {
        public static dynamic cfg = null;
        public static void Main()
        {
            if (!File.Exists("config.json"))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Config file does not exist! Generating config.");
                Functions.GenerateConfigFile();
            }

            if (!Functions.IsPassSet)
                Functions.AskForCreds();

            cfg = Functions.LoadConfig();

            if (cfg.access_token.ToString().Contains("=")) {
                Functions.DecryptString(cfg.access_token.ToString(), false);
                Console.Clear();
            }

            //nightmarenightmarenightmarenightmarenightmarenightmarenightmarenightmarenightmarenightmarenightmarenightmarenightmarenightmarenightmarenightmarenightmarenightmare
            Console.ResetColor();
            Console.Write("Ready to use: ");
            if (cfg.client_id.ToString() == "" || cfg.client_id.ToString() == "" || cfg.access_token.ToString() == "" || Functions.IsPassInvalid) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("False");
                Console.ResetColor();
            } else {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("True");
                Console.ResetColor();
            }
            Console.WriteLine();
            Console.Write("Password: ");
            if (Functions.IsPassInvalid) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Invalid");
                Console.ResetColor();
            } else {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Valid");
                Console.ResetColor();
            }

            Console.WriteLine(Environment.NewLine);
            Console.WriteLine(
                "Twitch Drop Farm Bot \nMade By: Jeesus Krisostoomus#7737.\n" +
                "\n1) Run Bot" +
                "\n2) Change Settings" +
                "\n3) Generate New Config (Will delete current config)" +
                "\n4) Change Password"
            );
            if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)+ "\\https___github.com_Jeesus"))
            {
                Console.WriteLine("99) Delete all old configs. (Recommended)");
            }
            Console.Write(":");

            var choice = Console.ReadLine();
            if (choice == 1.ToString()) {
                if (cfg.client_id.ToString().Trim() == "" || cfg.client_id.ToString().Trim() == "" || cfg.access_token.ToString().Trim() == "") {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Your bot is not yet ready to run.\nPlease change the settings accordingly to be able to run it.");
                    Console.ResetColor();
                    Thread.Sleep(3000);
                    Console.Clear();
                    Main();
                } else if (Functions.IsPassInvalid){
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid Password! Returning.");
                    Console.ResetColor();
                    Thread.Sleep(1000);
                    Console.Clear();
                    Main();
                }
                else
                {
                    MainBot.Main();
                }
            } else if (choice == 2.ToString()) {
                ChangeSettings.ChangeProgramSettings();
            } else if (choice == 3.ToString()) {
                Functions.GenerateConfigFile();
                Thread.Sleep(1000);
                Console.Clear();
                Main();
            }
            else if (choice == 4.ToString())
            {
                if (Functions.IsPassInvalid) {
                    Console.WriteLine("Current password is invalid. Closing.");
                    Thread.Sleep(1000);
                    return;
                }
                cfg.client_id = Functions.DecryptString(Program.cfg.client_id.ToString(), false);
                cfg.client_secret = Functions.DecryptString(Program.cfg.client_secret.ToString(), false);
                cfg.access_token = Functions.DecryptString(Program.cfg.access_token.ToString(), false);

                Console.Clear();
                Functions.AskForCreds(true);

                Functions.SaveConfig();

                Console.WriteLine("Password change successful.");
                Thread.Sleep(1000);
                Functions.AskForCreds();

                Main();
            } else if (choice == 99.ToString() && Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\https___github.com_Jeesus")) {
                Directory.Delete(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\https___github.com_Jeesus", true);
                Console.WriteLine("Old configs deleted");
                Thread.Sleep(1000);
                Console.Clear();
                Main();
            } else {
                Console.WriteLine("This option is not possible.");
                Thread.Sleep(2000);
                Console.Clear();
                Main();
            }
        }
    }
}
