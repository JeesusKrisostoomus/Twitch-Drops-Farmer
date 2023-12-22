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
        internal static int id = 0;
        internal static int watched = 0;
        internal static int wf = 0;
        internal static string streamername = "";
        internal static bool CurrLive = false;
        internal static bool AlrWatching = false;
        internal static int nightmare = 1000;
        internal static bool stopTask = false;

        public static void Main()
        {
            Console.Clear();
            //var streamername = "";
            bool CFS = false;

            while (true)
            {
                if (ManageStreamers.streamers.Count == 0)
                {
                    Console.WriteLine("Lets hope u got em all cuz am done.");
                    Console.ReadLine();
                    break;
                }

                Console.WriteLine("Streamers left to go: {0}.", ManageStreamers.streamers.Count);
                for (int i = 0; i < ManageStreamers.streamers.Count; i++)
                {
                    dynamic streamer = ManageStreamers.streamers[i];

                    if (streamer.Done == true)
                    {
                        AnsiConsole.MarkupLine("[yellow]{0} is already watched. Skipping streamer.[/]", streamer.StreamerName);
                        ManageStreamers.streamers.RemoveAll(res => res.Id == streamer.Id);
                        continue;
                    }

                    streamername = streamer.StreamerName;
                    try
                    {
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
                        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                        {
                            string result = streamReader.ReadToEnd();
                            var instance = JsonConvert.DeserializeObject<dynamic>(result);
                            foreach (var item in instance.data)
                            {
                                AlrWatching = false;
                                if (item.display_name == streamername || item.broadcaster_login == streamername && !AlrWatching)
                                {
                                    if (item.is_live == "false")
                                    {
                                        AnsiConsole.MarkupLine("[red]Streamer {0} is not live.[/]", streamername); //possibly start leaving this message out?
                                        continue;
                                    }
                                    if (item.is_live == "true" && streamer.SpecificGame && !item.game_name.ToString().ToLower().Contains(streamer.SpecificGameName.ToLower()))
                                    {
                                        AnsiConsole.MarkupLine("[yellow]Streamer {0} is not streaming specified game.[/]", streamername);
                                        continue;
                                    }
                                    if ((Boolean)Program.cfg.auto_open_stream && !AlrWatching)
                                    {
                                        AlrWatching = true;

                                        Task Timer = null;
                                        Task<bool> LiveChecker = null;

                                        id = streamer.Id;
                                        watched = streamer.Watched;
                                        wf = streamer.HowLongToWatch;

                                        if (watched >= wf)
                                        {
                                            AnsiConsole.MarkupLine("[yellow]User has already been watched. Skipping.[/]");
                                            DBManager.UpdateDone(id, true);
                                            continue;
                                        }


                                        AnsiConsole.MarkupLine("[green]Streamer: {0} | Live: {1} | Game: {2}[/]", item.display_name, item.is_live, item.game_name);

                                        DateTime currentTime = DateTime.Now;
                                        DateTime timeLater = currentTime.AddMinutes((streamer.HowLongToWatch - watched));
                                        Console.WriteLine("Opening stream and closing it after " + (streamer.HowLongToWatch - watched) + " minutes");
                                        Console.WriteLine("Stream will be closed at: " + timeLater);

                                        var psi = new System.Diagnostics.ProcessStartInfo();
                                        psi.FileName = "https://www.twitch.tv/" + streamername.ToLower();
                                        psi.WindowStyle = ProcessWindowStyle.Minimized;
                                        psi.CreateNoWindow = true;
                                        Process.Start(psi);
                                        nightmare = 60 * 1000 * ((streamer.HowLongToWatch + 1) - watched); // +1 cus it just quickly fixes a problem and i cant be bothered rn to make it some other way.
                                        CurrLive = true;

                                        Timer = Task.Run(() => AddTime());
                                        LiveChecker = Task.Run(() => LiveCheck());
                                        Thread.Sleep(100);
                                        Thread t = new Thread(() => {
                                            while (CurrLive && watched <= wf)
                                            {
                                                //Console.WriteLine("Thread Tick | {0} | {1} | {2}", watched, wf, watched >= wf);
                                                if (wf - watched == 1 && stopTask == false) stopTask = true;
                                                if (!CurrLive)
                                                {
                                                    Trace.WriteLine("Not live anymore");
                                                    Timer.Dispose();
                                                    LiveChecker.Dispose();
                                                    AlrWatching = false;
                                                    stopTask = false;

                                                    if ((bool)Program.cfg.auto_close_stream)
                                                    {
                                                        foreach (Process myProc in Process.GetProcesses())
                                                        {
                                                            if (myProc.ProcessName == Program.cfg.browser_proc_name.ToString())
                                                            {
                                                                myProc.Kill();
                                                            }
                                                        }
                                                    }
                                                    break;

                                                }
                                                if (watched >= wf)
                                                {
                                                    ManageStreamers.streamers.RemoveAll(res => res.Id == streamer.Id);
                                                    DBManager.UpdateDone(streamer.Id, true);
                                                    CFS = true;
                                                    Timer.Dispose();
                                                    LiveChecker.Dispose();
                                                    AlrWatching = false;
                                                    stopTask = false;

                                                    if ((bool)Program.cfg.auto_close_stream)
                                                    {
                                                        foreach (Process myProc in Process.GetProcesses())
                                                        {
                                                            if (myProc.ProcessName == Program.cfg.browser_proc_name.ToString())
                                                            {
                                                                myProc.Kill();
                                                            }
                                                        }
                                                    }
                                                    AnsiConsole.MarkupLine("[green]Watching task for {0} is now complete![/]", streamername);
                                                    break;

                                                }
                                                Thread.Sleep(1000);
                                            }
                                        });

                                        t.Start();
                                        Thread.Sleep(nightmare);
                                        t.Abort();

                                        //Thread.Sleep(1000 * 60 * (streamer.HowLongToWatch-watched));
                                        //Watcher.Dispose();
                                    }
                                }
                            }

                            streamReader.Close();
                            httpRequest.Abort();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Exception: " + ex.Message);
                        Console.ResetColor();
                    }
                }
                if (!AlrWatching)
                {
                    if (CFS)
                    {
                        Console.WriteLine("Next streamer check in 5 seconds.");
                        CFS = false;
                        Thread.Sleep(5000);
                        Console.Clear();
                    }
                    else
                    {
                        Console.WriteLine("Next check in 1 minutes.");
                        Thread.Sleep(60000);
                        Console.Clear();
                    }
                }


            }
        }
        public static async Task AddTime()
        {
            while (true && !stopTask)
            {
                await Task.Delay(TimeSpan.FromMinutes(1));
                watched++;
                DBManager.UpdateWatchedTime(id, watched);
            }
        }
        public static async Task<bool> LiveCheck()
        {
            // Every 1 minute check if stream is live or not.
            // If not stop watching the stream and go onto another one
            await Task.Delay(TimeSpan.FromMinutes(1));
            var url = "https://api.twitch.tv/helix/search/channels?query=" + streamername;
            var httpRequest = (HttpWebRequest)WebRequest.Create(url);

            try
            {
                httpRequest.Headers["client-id"] = Functions.DecryptString(Program.cfg.client_id.ToString());
                httpRequest.Headers["Authorization"] = "Bearer " + Functions.DecryptString(Program.cfg.access_token.ToString());
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Padding is invalid and cannot be removed") && ex.StackTrace.Contains("at TwitchDropFarmBot.Functions.DecryptString(String cipherText)"))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Given password is most likely incorrect. Closing the program in 2 sec...");
                    Console.ResetColor();
                    Thread.Sleep(2000);
                    return false;
                }
                Trace.WriteLine(ex.Message + " | " + ex.StackTrace);
            }

            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                string result = streamReader.ReadToEnd();
                var instance = JsonConvert.DeserializeObject<dynamic>(result);
                foreach (var item in instance.data)
                {
                    if (item.display_name == streamername || item.broadcaster_login == streamername)
                    {
                        if (item.is_live == "false")
                        {
                            CurrLive = false;
                            return false;
                        } else
                        {
                            CurrLive = true;
                            return true;
                        }
                    }
                }
                return false;
            }
        }
    }
}