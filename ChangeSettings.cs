using Spectre.Console;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace TwitchDropFarmBot
{
    class ChangeSettings
    {
        public static void ChangeProgramSettings()
        {
            //all of this could have been better but it just works
            Console.Clear();
            AnsiConsole.Write(
                new FigletText("Settings")
                    .Centered()
                    .Color(Color.White)
            );
            AnsiConsole.Write(new Rule());
            //Program.cfg = Functions.LoadConfig();
            AnsiConsole.MarkupLine(String.Format("Current Settings:" +
                "\n- [teal]Client ID[/]: {0}" +
                "\n- [teal]Client Secret[/]: {1}" +
                "\n- [teal]Access Token (OAuth)[/]: {2}" +
                "\n- [teal]Auto Open Stream (This will open a new browser and take focus)[/]: {3}" +
                "\n- [teal]Auto Close Stream (This will close the whole browser)[/]: {4}" +
                "\n- [teal]Browser Process Name[/]: {5}",
                Program.cfg.client_id.ToString(), 
                Program.cfg.client_secret.ToString(), 
                Program.cfg.access_token.ToString(), 
                Program.cfg.auto_open_stream, 
                Program.cfg.auto_close_stream, 
                Program.cfg.browser_proc_name
            ));
            
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<Option>()
                    .Title("")
                    .PageSize(10)
                    .AddChoices(new[] {
                        new Option { Id = 1, Name = "Client ID" },
                        new Option { Id = 2, Name = "Client Secret" },
                        new Option { Id = 3, Name = "Generate Access Token (OAuth)" },
                        new Option { Id = 4, Name = "Auto Open Stream" },
                        new Option { Id = 5, Name = "Auto Close Browser" },
                        new Option { Id = 6, Name = "Browser Process Name" },
                        new Option { Id = 7, Name = "Encrypt Config Values/Save Config" },
                        new Option { Id = 8, Name = "Decrypt Config Values [red](use only if you want to see the values)[/]" },
                        new Option { Id = 9, Name = "Reload Config" },
                        new Option { Id = 10, Name = "Back" },
                    })
                    .UseConverter(option => option.Name)
            );
            
            switch (choice.Id)
            {
                case 1:
                    Console.Clear();
                    Program.cfg.client_id = AnsiConsole.Ask<string>("Enter new Client ID:");
                    ChangeProgramSettings();
                    break;

                case 2:
                    Console.Clear();
                    Program.cfg.client_secret = AnsiConsole.Ask<string>("Enter new Client Secret:");
                    ChangeProgramSettings();
                    break;

                case 3:
                    Console.Clear();
                    GetOAuth.MainFunc();
                    ChangeProgramSettings();
                    break;

                case 4:
                    Console.Clear();
                    Program.cfg.auto_open_stream = !(bool)Program.cfg.auto_open_stream;
                    ChangeProgramSettings();
                    break;

                case 5:
                    Console.Clear();
                    Program.cfg.auto_close_stream = !(bool)Program.cfg.auto_close_stream;
                    ChangeProgramSettings();
                    break;

                case 6:
                    Console.Clear();

                    AnsiConsole.MarkupLine("Examples:" +
                        "\nOpera - [teal]opera[/]" +
                        "\nGoogle Chrome - [teal]chrome[/]" +
                        "\nFirefox - [teal]firefox[/]" +
                        "\nMicrosoft Edge - [teal]MicrosoftEdge[/]" +
                        "\nInternet Explorer - [teal]iexplore[/]"
                    );

                    AnsiConsole.MarkupLine("\n[yellow]Be sure that the name is correct. It will try killing anything you told it to.[/]");
                    Program.cfg.browser_proc_name = AnsiConsole.Ask<string>("Enter browser process name:");
                    ChangeProgramSettings();
                    break;

                case 7:
                    Functions.SaveConfig();
                    AnsiConsole.WriteLine("[green]Config save/Encrypted[/]");
                    Thread.Sleep(1000);
                    ChangeProgramSettings();
                    break;

                case 8:
                    Program.cfg.client_id = Functions.DecryptString(Program.cfg.client_id.ToString());
                    Program.cfg.client_secret = Functions.DecryptString(Program.cfg.client_secret.ToString(), false);
                    Program.cfg.access_token = Functions.DecryptString(Program.cfg.access_token.ToString(), false);

                    ChangeProgramSettings();
                    break;

                case 9:
                    Program.cfg = Functions.LoadConfig();
                    ChangeProgramSettings();
                    break;

                case 10:
                    Console.Clear();
                    Program.Main();
                    break;

                default:
                    AnsiConsole.MarkupLine("[orangered1]Invalid Option[/]");
                    Thread.Sleep(1000);
                    Console.Clear();
                    ChangeProgramSettings();
                    break;
            }
        }
    }
}
