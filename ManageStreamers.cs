using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TwitchDropFarmBot
{
    internal class ManageStreamers
    {
        public static List<StreamerData> streamers = new List<StreamerData>();

        public static void Main() 
        {
            Console.Clear();

            AnsiConsole.Write(
                new FigletText("Streamer Management")
                    .Centered()
                    .Color(Color.White)
            );
            AnsiConsole.Write(new Rule());

            LoadStreamerList();

            if (streamers.Count == 0 )
            {
                AnsiConsole.MarkupLine("[maroon]There are no streamers in the list![/]");
            } 
            else
            {
                AnsiConsole.MarkupLine("[green]Streamer Count: {0}[/]", streamers.Count);
                var table = new Table().Centered();

                table.AddColumns(
                    new TableColumn("ID").Centered(), 
                    new TableColumn("NAME").Centered(), 
                    new TableColumn("SPECIFIC GAME").Centered(),
                    new TableColumn("GAME").Centered(), 
                    //new TableColumn("WATCHTIME").Centered(),
                    new TableColumn("WATCHED").Centered(),
                    new TableColumn("DONE").Centered()
                );

                try {
                    foreach (var streamer in streamers)
                    {
                        table.AddRow(
                            streamer.Id.ToString(),
                            streamer.StreamerName.ToString(),
                            streamer.SpecificGame.ToString(),
                            streamer.SpecificGameName.ToString(),
                            //streamer.HowLongToWatch.ToString(),
                            streamer.Watched.ToString() + "/" + streamer.HowLongToWatch.ToString(),
                            streamer.Done.ToString()
                        );
                    }

                    AnsiConsole.Write(table);
                }
                catch (Exception ex) 
                {
                    AnsiConsole.MarkupLine("[red]Error adding streamer: [/]"+ ex.Message);
                    Trace.WriteLine(ex.Message); 
                }

            }

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<Option>()
                    .Title("")
                    .PageSize(10)
                    .AddChoices(new List<Option> {
                        new Option { Id = 1, Name = "Add Streamer" },
                        new Option { Id = 2, Name = "Remove Streamer" },
                        new Option { Id = 3, Name = "Back" },
                    })
                    .UseConverter(option => option.Name)
                );


            switch (choice.Id)
            {
                case 1:
                    AddStreamerToList();
                    break;

                case 2:
                    var id = AnsiConsole.Ask<Int32>("Enter streamer ID:");
                    if (!AnsiConsole.Confirm("Are you sure you want to remove the streamer?"))
                    {
                        break;
                    }
                    DBManager.DeleteStreamData(id);
                    Main();
                    break;

                case 3:
                    Program.Main();
                    break;
            }
        }

        public static void LoadStreamerList()
        {
            streamers.Clear();
            IEnumerable<StreamerData> str_db = DBManager.GetAllStreamData();
            
            foreach (StreamerData data in str_db)
            {
                StreamerData streamer = new StreamerData
                {
                    Id = data.Id,
                    StreamerName = data.StreamerName,
                    HowLongToWatch = data.HowLongToWatch,
                    SpecificGame = data.SpecificGame,
                    SpecificGameName = data.SpecificGameName,
                    Watched = data.Watched,
                    Done = data.Done
                };
                if (!streamers.Any(existingStreamer => existingStreamer.Id == data.Id) || !streamers.Any(existingStreamer => existingStreamer.StreamerName == data.StreamerName))
                {
                    streamers.Add(streamer);
                }
            }
        }
        private static void AddStreamerToList()
        {
            Console.Clear();
            string name = AnsiConsole.Ask<string>("Enter Streamer Name:");
            int hltw = AnsiConsole.Ask<Int32>("How long to watch for (Minutes):");
            bool specificGame = AnsiConsole.Ask<bool>("Wait for specific game? (True/False):");
            string specificGameName = "Not Set";
            if (specificGame)
            {
                specificGameName = AnsiConsole.Ask<string>("Specific game name:");
            }


            StreamerData streamer = new StreamerData
            {
                StreamerName = name,
                HowLongToWatch = hltw,
                SpecificGame = specificGame,
                SpecificGameName = specificGameName,
                Watched = 0,
                Done = false
            };
            DBManager.InsertStreamData(streamer);
            Main();
        }
    }
}