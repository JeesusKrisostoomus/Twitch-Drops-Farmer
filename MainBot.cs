using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Threading;
using System.Net;
using System.Diagnostics;

namespace TwitchDropFarmBot
{
    class MainBot
    {
        public static void Main()
        {
            List<string> ViewedStreamers = new List<string>(); //Where all of the streamer names will be stored at.

            Console.Clear();
            var streamername = "";
            var CloseAfterMinutes = 0;
            bool CFS = false;

            Console.WriteLine("How long to watch each stream before closing? (In Minutes)");
            Console.Write(":");
            CloseAfterMinutes = Int32.Parse(Console.ReadLine());
            if (!File.Exists("streamerNames.txt"))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("streamerNames.txt Does not exist! Creating the file now.");
                File.Create("streamerNames.txt");
                Console.WriteLine("Please put streamer names into streamerNames.txt (1 NAME PER LINE!)");
                Console.WriteLine("Full file path: " + Path.GetFullPath("streamerNames.txt"));
                Console.ResetColor();
                Console.WriteLine("Press ANY key to exit");
                Console.ReadKey();
                return;
            }

            var lines = File.ReadAllLines("streamerNames.txt");
            if (lines.Length == 0)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error! streamerNames.txt Does not contain any streamer names to watch. \n" +
                    "Please enter streamer names to watch.\n" +
                    "1 NAME PER LINE!"
                );
                Console.WriteLine("Full file path: " + Path.GetFullPath("streamerNames.txt"));
                Console.ResetColor();
                Console.WriteLine("Press ANY key to exit.");
                Console.ReadKey();
                return;
            }

            //add the streamers to list
            foreach (var line in lines)
            {
                ViewedStreamers.Add(line); 
            }

            while (true)
            {
                if (ViewedStreamers.Count == 0)
                {
                    Console.WriteLine("Lets hope u got em all cuz am done.");
                    Console.ReadLine();
                    break;
                }

                Console.WriteLine("Streamers left to go: {0}.", ViewedStreamers.Count);
                for (int i = 0; i < ViewedStreamers.Count; i++)
                {
                    streamername = ViewedStreamers[i];
                    try
                    {
                        var url = "https://api.twitch.tv/helix/search/channels?query=" + streamername;
                        var httpRequest = (HttpWebRequest)WebRequest.Create(url);

                        //Headers required for checking if the person is live or not
                        httpRequest.Headers["client-id"] = Properties.Settings.Default.client_id;
                        httpRequest.Headers["Authorization"] = "Bearer " + Properties.Settings.Default.access_token; //Bearer could and most likely will change some time but until then its good.

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
                                        Console.ForegroundColor = ConsoleColor.Green;
                                        Console.WriteLine("Streamer: {0} | Live: {1} | Game: {2}", item.display_name, item.is_live, item.game_name);
                                        Console.ResetColor();

                                        if (Properties.Settings.Default.auto_open)
                                        {
                                            DateTime currentTime = DateTime.Now;
                                            DateTime timeLater = currentTime.AddMinutes(CloseAfterMinutes);
                                            Console.WriteLine("Opening stream and closing it after " + CloseAfterMinutes + " minutes");
                                            Console.WriteLine("Stream will be closed at: " + timeLater);

                                            var psi = new System.Diagnostics.ProcessStartInfo();
                                            psi.UseShellExecute = true;
                                            psi.FileName = "https://www.twitch.tv/" + streamername.ToLower();
                                            Process.Start(psi);
                                            Thread.Sleep(1000 * 60 * CloseAfterMinutes);
                                            ViewedStreamers.Remove(streamername);
                                            CFS = true;

                                            if (Properties.Settings.Default.auto_close)
                                            {
                                                foreach (Process myProc in Process.GetProcesses())
                                                {
                                                    if (myProc.ProcessName == Properties.Settings.Default.browser_procname)
                                                    {
                                                        myProc.Kill();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            //Console.WriteLine(data.id[0]);
                            //Console.WriteLine(data);
                            //Console.WriteLine(result);
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
                    Console.WriteLine("Next streamer check in 5 minutes.");
                    Thread.Sleep(300000);
                    Console.Clear();
                }
            }
        }
    }
}