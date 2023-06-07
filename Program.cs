using System;
using System.Threading;
using System.IO;
using Spectre.Console;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using LiteDB;

namespace TwitchDropFarmBot
{
    public class Option
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    class Program
    {
        public static dynamic cfg = null;
        public static void Main()
        {
            if (!File.Exists("config.json"))
            {
                Functions.GenerateConfigFile();
                AnsiConsole.MarkupLine("[red]Config file does not exist![/] Generating one now.");
            }
            //if (!File.Exists(DBManager.databasePath)) { new LiteDatabase(DBManager.databasePath); }
            DBManager dbManager = new DBManager(DBManager.databasePath);
            ManageStreamers.LoadStreamerList();

            if (!Functions.IsPassSet)
                Functions.AskForCreds();

            cfg = Functions.LoadConfig();

            if (cfg.access_token.ToString().Contains("=")) {
                Functions.DecryptString(cfg.access_token.ToString(), false);
                Console.Clear();
            }

            AnsiConsole.Write(
                new FigletText("Twitch Drops Farm Bot")
                    .Centered()
                    .Color(Color.White)
            );
            AnsiConsole.Write(new Rule("[green]Made by: Jeesus Krisostoomus#7737[/]"));

            if (cfg.client_id.ToString() == "" || cfg.client_id.ToString() == "" || cfg.access_token.ToString() == "" || Functions.IsPassInvalid) {
                AnsiConsole.MarkupLine("Ready to use: [red]False[/]");
            } else {
                AnsiConsole.MarkupLine("Ready to use: [green]True[/]");
            }
            if (Functions.IsPassInvalid) {
                AnsiConsole.MarkupLine("Password: [red]Invalid[/]");
            } else {
                AnsiConsole.MarkupLine("Password: [green]Valid[/]");
            }

            var choices = new List<Option> {
                    new Option { Id = 1, Name = "Run Bot" },
                    new Option { Id = 2, Name = "Change Settings" },
                    new Option { Id = 3, Name = "Generate New Config [red](Will delete current config)[/]" },
                    new Option { Id = 4, Name = "Change Password" },
                    //5 is ignored as it is used for deleting outdated configs if they do exist.
                    new Option { Id = 7, Name = "Manage Streamers" },
                    new Option { Id = 6, Name = "[red]Quit[/]" },
            };

            if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\https___github.com_Jeesus"))
            {
                //Console.WriteLine("99) Delete all old configs. (Recommended)");
                choices.Add(new Option { Id = 5, Name = "Delete all deprecated configs. (Recommended)" });
            }

            var selection = AnsiConsole.Prompt(
                new SelectionPrompt<Option>()
                    .Title("")
                    .PageSize(10)
                    .AddChoices(choices)
                    .UseConverter(option => option.Name)
            );

            switch (selection.Id)
            {
                case 1:
                    if (cfg.client_id.ToString().Trim() == "" || cfg.client_id.ToString().Trim() == "" || cfg.access_token.ToString().Trim() == "")
                    {
                        AnsiConsole.MarkupLine("[red]Your bot is not yet ready to run![/]\nPlease change the settings accordingly to be able to run it.");
                        Thread.Sleep(3000);
                        Console.Clear();
                        Main();
                    }
                    else if (Functions.IsPassInvalid)
                    {
                        AnsiConsole.MarkupLine("[red]Invalid Password![/]\nReturning...");
                        Thread.Sleep(1000);
                        Console.Clear();
                        Main();
                    }
                    else
                    {
                        MainBot.Main();
                    }
                    break;

                case 2:
                    ChangeSettings.ChangeProgramSettings();
                    break;

                case 3:
                    Functions.GenerateConfigFile();
                    Functions.AskForCreds(true);
                    Thread.Sleep(1000);
                    Console.Clear();
                    Main();
                    break;

                case 4:
                    if (Functions.IsPassInvalid)
                    {
                        AnsiConsole.MarkupLine("[red]Current password is invalid![/]\nClosing the program...");
                        Thread.Sleep(1000);
                        return;
                    }
                    cfg.client_id = Functions.DecryptString(Program.cfg.client_id.ToString(), false);
                    cfg.client_secret = Functions.DecryptString(Program.cfg.client_secret.ToString(), false);
                    cfg.access_token = Functions.DecryptString(Program.cfg.access_token.ToString(), false);

                    Console.Clear();
                    Functions.AskForCreds(true);

                    Functions.SaveConfig();

                    AnsiConsole.MarkupLine("Password change: [green]successful[/]");
                    Thread.Sleep(1000);
                    Functions.AskForCreds();

                    Main();
                    break;

                case 5:
                    Directory.Delete(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\https___github.com_Jeesus", true);
                    AnsiConsole.MarkupLine("[green]Deprecated configs deleted.[/]");
                    Thread.Sleep(1000);
                    Console.Clear();
                    Main();
                    break;

                case 6:
                    Environment.Exit(0);
                    break;

                case 7:
                    ManageStreamers.Main();
                    break;

                default:
                    AnsiConsole.MarkupLine("[orangered1]Invalid Option[/]");
                    Thread.Sleep(2000);
                    Console.Clear();
                    Main();
                    break;
            }
        }
    }
}
