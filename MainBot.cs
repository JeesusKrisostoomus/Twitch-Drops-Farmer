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
            List<string> StreamerList = new List<string>(); //Where all of the streamer names will be stored at.

            Console.Clear();
            var streamername = "";
            var CloseAfterMinutes = 0;
            bool SpecificGame = false;
            string SpecificGameName = "";
            bool CFS = false;

            Console.WriteLine("How long to watch each stream before closing? (In Minutes)");
            Console.Write(":");
            CloseAfterMinutes = Int32.Parse(Console.ReadLine());

            Console.WriteLine("Wait for specific game? True/False");
            Console.Write(":");
            SpecificGame = Boolean.Parse(Console.ReadLine());

            if (SpecificGame)
            {
                Console.WriteLine("Specific Game Name");
                Console.Write(":");
                SpecificGameName = Console.ReadLine().Trim();
            }
            
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
                StreamerList.Add(line); 
            }

            while (true)
            {
                if (StreamerList.Count == 0)
                {
                    Console.WriteLine("Lets hope u got em all cuz am done.");
                    Console.ReadLine();
                    break;
                }

                Console.WriteLine("Streamers left to go: {0}.", StreamerList.Count);
                for (int i = 0; i < StreamerList.Count; i++)
                {
                    streamername = StreamerList[i];
                    try
                    {
                        var url = "https://api.twitch.tv/helix/search/channels?query=" + streamername;
                        var httpRequest = (HttpWebRequest)WebRequest.Create(url);

                        //Headers required for checking if the person is live or not
                        //Also this try catch should work but if it doesnt then oh well fuck this
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
                                        if (SpecificGame)
                                        {
                                            if (item.game_name.ToString().ToLower().Contains(SpecificGameName.ToLower()))
                                            {
                                                Console.ForegroundColor = ConsoleColor.Green;
                                                Console.WriteLine("Streamer: {0} | Live: {1} | Game: {2}", item.display_name, item.is_live, item.game_name);
                                                Console.ResetColor();

                                                if ((bool)Program.cfg.auto_open_stream)
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
                                                    StreamerList.Remove(streamername);
                                                    CFS = true;

                                                    if (Program.cfg.auto_close_stream.ToString())
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
                                            else
                                            {
                                                Console.ForegroundColor = ConsoleColor.Red;
                                                Console.WriteLine("Streamer: {0} | Live: {1}", item.display_name, item.is_live);
                                                Console.ResetColor();
                                            }
                                        } else
                                        {
                                            Console.ForegroundColor = ConsoleColor.Green;
                                            Console.WriteLine("Streamer: {0} | Live: {1} | Game: {2}", item.display_name, item.is_live, item.game_name);
                                            Console.ResetColor();

                                            if (Program.cfg.auto_open_stream)
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
                                                StreamerList.Remove(streamername);
                                                CFS = true;

                                                if (Program.cfg.auto_close_stream.ToString())
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
                    Console.WriteLine("Next streamer check in 5 minutes.");
                    Thread.Sleep(300000);
                    Console.Clear();
                }
            }
        }
    }
}