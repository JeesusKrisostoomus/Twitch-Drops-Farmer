﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Threading;
using System.Net;
using System.Diagnostics;
using Spectre.Console;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace TwitchDropFarmBot
{
    class MainBot
    {
        internal static int id = 0;
        internal static int watched = 0;

        public static async void Main()
        {
            Console.Clear();
            var streamername = "";
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
                            foreach (var item in instance.data) //All of this could have been done using other ways but ye sure.
                            {
                                if (item.display_name == streamername || item.broadcaster_login == streamername)
                                {
                                    if (item.is_live == "false")
                                    {
                                        Console.ForegroundColor = ConsoleColor.Red;
                                        Console.WriteLine("Streamer: {0} | Live: {1}", item.display_name, item.is_live);
                                        Console.ResetColor();
                                    }
                                    else
                                    {
                                        if (streamer.SpecificGame)
                                        {
                                            if (!item.game_name.ToString().ToLower().Contains(streamer.SpecificGameName.ToLower()))
                                            {
                                                Console.WriteLine("{0} is not streaming specified game.", item.display_name);
                                                break;
                                            }
                                        }
                                        Console.ForegroundColor = ConsoleColor.Green;
                                        Console.WriteLine("Streamer: {0} | Live: {1} | Game: {2}", item.display_name, item.is_live, item.game_name);
                                        Console.ResetColor();

                                        if ((Boolean)Program.cfg.auto_open_stream)
                                        {
                                            id = streamer.Id;
                                            watched = streamer.Watched;

                                            DateTime currentTime = DateTime.Now;
                                            DateTime timeLater = currentTime.AddMinutes((streamer.HowLongToWatch-watched));
                                            Console.WriteLine("Opening stream and closing it after " + (streamer.HowLongToWatch - watched) + " minutes");
                                            Console.WriteLine("Stream will be closed at: " + timeLater);

                                            var psi = new System.Diagnostics.ProcessStartInfo();
                                            psi.FileName = "https://www.twitch.tv/" + streamername.ToLower();
                                            psi.WindowStyle = ProcessWindowStyle.Minimized;
                                            psi.CreateNoWindow = true;
                                            Process.Start(psi);
                                            
                                            Task Watcher = Task.Run(AddTime);
                                            Thread.Sleep(1000 * 60 * (streamer.HowLongToWatch-watched));
                                            Watcher.Dispose();
                                            ManageStreamers.streamers.RemoveAll(res => res.Id == streamer.Id);
                                            DBManager.UpdateDone(streamer.Id, true);
                                            CFS = true;

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
                        Console.WriteLine("Exception: " + ex);
                        Console.ResetColor();
                    }
                }

                if (CFS)
                {
                    Console.WriteLine("Next streamer check in 5 seconds.");
                    CFS = false;
                    Thread.Sleep(5000);
                    Console.Clear();
                }
                else
                {
                    Console.WriteLine("Next streamer check in 1 minutes.");
                    Thread.Sleep(60000);
                    Console.Clear();
                }
            }
        }
        public static async Task Watch()
        {
            //Watch the thing
        }

        public static async Task AddTime()
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromMinutes(1));
                watched++;
                DBManager.UpdateWatchedTime(id, watched);
            }
        }
        public static async Task LiveCheck()
        {
            //Every 1 minute check if stream is live or not. If not stop watching the stream and go onto another one
        }
    }
}