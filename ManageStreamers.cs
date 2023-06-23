using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

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
                        new Option { Id = 4, Name = "Bulk Add Streamers" },
                        new Option { Id = 2, Name = "Remove Streamer" },
                        new Option { Id = 3, Name = "Clear List" },
                        new Option { Id = 99, Name = "Back" },
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
                        Main();
                        break;
                    }
                    DBManager.DeleteStreamData(id);
                    Main();
                    break;
                case 3: // bulk clear list
                    if (!AnsiConsole.Confirm("[red]Are you sure you want to clear the list?[/]"))
                    {
                        Main();
                        break;
                    }
                    foreach (StreamerData data in streamers) {
                        DBManager.DeleteStreamData(data.Id);
                    }
                    streamers.Clear();
                    AnsiConsole.MarkupLine("[green]Streamer list has been cleared.[/]");
                    Thread.Sleep(1000);
                    Main();
                    break;
                case 4:
                    AddStreamerToList(true);
                    break;
                case 99:
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
        private static void AddStreamerToList(bool bulk = false)
        {
            Console.Clear();
            if (bulk) AnsiConsole.MarkupLine("[yellow]Seperate all the names with ',' Example: Tilbzik,random_streamer,random_streamer2,random_streamer3[/]");
            string name = AnsiConsole.Ask<string>("Enter Streamer Name:");
            int hltw = AnsiConsole.Ask<Int32>("How long to watch for (Minutes):");
            bool specificGame = AnsiConsole.Ask<bool>("Wait for specific game? (True/False):");
            string specificGameName = "Not Set";
            if (specificGame)
            {
                specificGameName = AnsiConsole.Ask<string>("Specific game name:");
            }

            if (bulk) {
                foreach (string strn in name.Split(',')) {
                    StreamerData streamer = new StreamerData
                    {
                        StreamerName = strn,
                        HowLongToWatch = hltw,
                        SpecificGame = specificGame,
                        SpecificGameName = specificGameName,
                        Watched = 0,
                        Done = false
                    };
                    
                    DBManager.InsertStreamData(streamer);
                }
            } else {
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
            }
            Main();
        }
    }
}