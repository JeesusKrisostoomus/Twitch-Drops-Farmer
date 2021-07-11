using System;
using System.Threading;

namespace TwitchDropFarmBot
{
    class Program
    {
        public static void Main()
        {
            //nightmarenightmarenightmarenightmarenightmarenightmarenightmarenightmarenightmarenightmarenightmarenightmarenightmarenightmarenightmarenightmarenightmarenightmare
            Console.ResetColor();
            Console.Write("Ready to use: ");
            if (Properties.Settings.Default.client_id.Trim() == "Unset" || 
                Properties.Settings.Default.client_id.Trim() == "" ||
                Properties.Settings.Default.client_secret.Trim() == "Unset" ||
                Properties.Settings.Default.client_id.Trim() == "" ||
                Properties.Settings.Default.access_token.Trim() == "Unset" ||
                Properties.Settings.Default.access_token.Trim() == ""
            ) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("False");
                Console.ResetColor();
            } else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("True");
                Console.ResetColor();
            }
            Console.WriteLine();
            Console.WriteLine("This program is scuffed af so it might work and it also might not work. So good luck.");
            Console.WriteLine(
                "Welcome to Twitch Drop Farm Bot \nMade By: Jeesus Krisostoomus#7737." +
                "\n1) Run Bot" +
                "\n2) Change Settings"
            );
            Console.Write(":");

            var choice1 = Console.ReadLine();
            if (choice1 == 1.ToString()) {
                if (
                    Properties.Settings.Default.client_id.Trim() == "Unset" ||
                    Properties.Settings.Default.client_id.Trim() == "" ||
                    Properties.Settings.Default.client_secret.Trim() == "Unset" ||
                    Properties.Settings.Default.client_id.Trim() == "" ||
                    Properties.Settings.Default.access_token.Trim() == "Unset" ||
                    Properties.Settings.Default.access_token.Trim() == ""
                ) {
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
            }
            else if (choice1 == 2.ToString())
            {
                ChangeSettings.ChangeProgramSettings();
            }
            else
            {
                Console.WriteLine("This option is not possible.");
                Thread.Sleep(2000);
                Console.Clear();
                Main();
            }
        }
    }
}
