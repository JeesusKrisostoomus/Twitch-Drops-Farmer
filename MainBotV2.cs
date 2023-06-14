using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Threading;
using System.Net;
using System.Diagnostics;
using Spectre.Console;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;

namespace TwitchDropFarmBot
{
    class MainBot
    {
        internal static string currentStreamer = "";
        internal static bool alrWatching = false;
        internal static int watched = 0;
        internal static int watchfor = 0;
        internal static dynamic streamer = null;

        public static Task Main() {
            Console.Clear();

            while (true) {
                if (ManageStreamers.streamers.Count == 0) {
                    Console.WriteLine("Streamer list is empty. Exiting...");
                    Thread.Sleep(1000);
                    break;
                }
                Console.WriteLine("Streamers left to go: {0}.", ManageStreamers.streamers.Count);
                //Loop over each streamer in ManageStreamers.Streamers.Count
                for (int i = 0; i < ManageStreamers.streamers.Count; i++) {
                    streamer = ManageStreamers.streamers[i];
                    currentStreamer = streamer.StreamerName;

                    if (streamer.Done == true)
                    {
                        AnsiConsole.MarkupLine("[yellow]{0} is already watched. Skipping streamer.[/]", streamer.StreamerName);
                        ManageStreamers.streamers.RemoveAll(res => res.Id == streamer.Id);
                        continue;
                    }

                    //First time live check
                    try {
                        var url = "https://api.twitch.tv/helix/search/channels?query=" + streamername;
                        var httpRequest = (HttpWebRequest)WebRequest.Create(url);

                         try
                        {
                            httpRequest.Headers["client-id"] = Functions.DecryptString(Program.cfg.client_id.ToString());
                            httpRequest.Headers["Authorization"] = "Bearer " + Functions.DecryptString(Program.cfg.access_token.ToString());
                        } catch (Exception ex) {
                            if (ex.Message.Contains("Padding is invalid and cannot be removed") && ex.StackTrace.Contains("at TwitchDropFarmBot.Functions.DecryptString(String cipherText)"))
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Given password is most likely incorrect. Closing the program in 2 sec...");
                                Console.ResetColor();
                                Thread.Sleep(2000);
                                return;
                            }
                            Trace.WriteLine(ex.Message + " | " + ex.StackTrace);
                        }

                        var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                        using (var streamReader = new StreamReader(httpResponse.GetResponseStream())) {
                            string result = streamReader.ReadToEnd();
                            var instance = JsonConvert.DeserializeObject<dynamic>(result);

                            foreach (var item in instance.data)
                            {
                                if (item.display_name == streamername || item.broadcaster_login == streamername && !alrWatching) {
                                    if (item.is_live == "false") {
                                        AnsiConsole.MarkupLine("[red]Streamer {0} is not live.[/]"); //possibly start leaving this message out?
                                        continue;
                                    }
                                    if (item.is_live == "true" && streamer.SpecificGame && !item.game_name.ToString().ToLower().Contains(streamer.SpecificGameName.ToLower())) {
                                        AnsiConsole.MarkupLine("[yellow]Streamer {0} is not streaming specified game.[/]", currentStreamer);
                                        continue;
                                    }
                                }
                            }
                        }

                    } catch (exception) {

                    }
                
            }

        }

    }

}