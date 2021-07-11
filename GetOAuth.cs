using System;
using System.Threading;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace TwitchDropFarmBot
{
    class GetOAuth
    {
        public static void MainFunc()
        {
            string url;

            //also f you twitch

            Console.Clear();

            if (Properties.Settings.Default.client_id.Trim() == "Unset" || Properties.Settings.Default.client_id.Trim() == "")
            {
                Console.WriteLine("Client ID Is Not Set. You need to set Client ID before you can continue.");
                Console.WriteLine("Press ANY key to go back");
                Console.ReadKey();
                Program.Main();
            }
            if (Properties.Settings.Default.client_secret.Trim() == "Unset" || Properties.Settings.Default.client_secret.Trim() == "")
            {
                Console.WriteLine("Client Secret Is Not Set. You need to set Client Secret before you can continue.");
                Console.WriteLine("Press ANY key to back");
                Console.ReadKey();
                Program.Main();
            }

            url = "https://id.twitch.tv/oauth2/token?client_id=" + Properties.Settings.Default.client_id.Trim() + "&client_secret=" + Properties.Settings.Default.client_secret.Trim() + "&grant_type=client_credentials";

            var httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Method = "POST";

            try
            {
                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    string result = streamReader.ReadToEnd();                    
                    var data = JsonConvert.DeserializeObject<dynamic>(result);

                    Properties.Settings.Default.access_token = data.access_token;
                    Properties.Settings.Default.Save();
                    Console.WriteLine("Access token should now be set. Returning in 2 seconds.");
                    Thread.Sleep(2000);
                    Console.Clear();
                    Program.Main();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("There was an error: " + ex);
                Console.ResetColor();
            }
        }
    }
}
