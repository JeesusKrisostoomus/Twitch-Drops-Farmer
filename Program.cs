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
                Console.WriteLine("Config file does not exist! Generating config and closing program...");
                Functions.GenerateConfigFile();
                Thread.Sleep(1000);
                return;
            }

            cfg = Functions.LoadConfig();

            if (!Functions.IsPassSet)
                Functions.AskForCreds();

            //nightmarenightmarenightmarenightmarenightmarenightmarenightmarenightmarenightmarenightmarenightmarenightmarenightmarenightmarenightmarenightmarenightmarenightmare
            Console.ResetColor();
            Console.Write("Ready to use: ");
            if (cfg.client_id.ToString() == "" || cfg.client_id.ToString() == "" || cfg.access_token.ToString() == "") {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("False");
                Console.ResetColor();
            } else {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("True");
                Console.ResetColor();
            }
            Console.WriteLine();
            Console.WriteLine("This program is scuffed af so it might work and it also might not work. So good luck.");
            Console.WriteLine(
                "Welcome to Twitch Drop Farm Bot \nMade By: Jeesus Krisostoomus#7737." +
                "\n1) Run Bot" +
                "\n2) Change Settings" +
                "\n3) Generate New Config"
            );
            Console.Write(":");

            var choice1 = Console.ReadLine();
            if (choice1 == 1.ToString()) {
                if (cfg.client_id.ToString().Trim() == "" || cfg.client_id.ToString().Trim() == "" || cfg.access_token.ToString().Trim() == "") {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Your bot is not yet ready to run.\nPlease change the settings accordingly to be able to run it.");
                    Console.ResetColor();
                    Console.WriteLine("Press ANY key to continue");
                    Console.ReadKey();
                    Console.Clear();
                    Main();
                } else {
                    MainBot.Main();
                }
            } else if (choice1 == 2.ToString()) {
                ChangeSettings.ChangeProgramSettings();
            } else if (choice1 == 2.ToString()) {
                Functions.GenerateConfigFile();
            } else {
                Console.WriteLine("This option is not possible.");
                Thread.Sleep(2000);
                Console.Clear();
                Main();
            }
        }
    }
}
