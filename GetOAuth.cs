using System;
using System.Threading;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Spectre.Console;

namespace TwitchDropFarmBot
{
    class GetOAuth
    {
        public static void MainFunc()
        {
            string url;

            Console.Clear();

            if (Program.cfg.client_id.ToString().Trim() == "")
            {
                Console.WriteLine("Client ID Is Not Set. You need to set Client ID before you can continue.");
                Console.WriteLine("Press ANY key to go back");
                Console.ReadKey();
                Program.Main();
            }
            if (Program.cfg.client_secret.ToString().Trim() == "")
            {
                Console.WriteLine("Client Secret Is Not Set. You need to set Client Secret before you can continue.");
                Console.WriteLine("Press ANY key to back");
                Console.ReadKey();
                Program.Main();
            }

            url = "https://id.twitch.tv/oauth2/token?client_id=" + Functions.DecryptString(Program.cfg.client_id.ToString().Trim()) + "&client_secret=" + Functions.DecryptString(Program.cfg.client_secret.ToString().Trim()) + "&grant_type=client_credentials";

            var httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Method = "POST";

            try
            {
                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    string result = streamReader.ReadToEnd();                    
                    var data = JsonConvert.DeserializeObject<dynamic>(result);

                    Program.cfg.access_token = data.access_token;
                    AnsiConsole.MarkupLine("[green]Access token should now be set (REMEMBER TO SAVE CONFIG!).[/]\nReturning in 2 seconds.");
                    //Functions.SaveConfig();
                    Thread.Sleep(2000);
                    Console.Clear();
                    ChangeSettings.ChangeProgramSettings();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("There was an error: " + ex);
                Console.ResetColor();
                Thread.Sleep(1000);
            }
        }
    }
}
