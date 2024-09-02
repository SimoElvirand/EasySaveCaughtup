using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EasySave;
using EasySave.models;
using EasySave.utils;
using Spectre.Console;


namespace EasySave
{
    class HomeView
    {
        private System.Resources.ResourceManager mgr;
        public System.Resources.ResourceManager resourceManager { get => mgr;}
        public HomeView()
        {
            Console.WindowWidth = AppSettings.ConsoleWidth;
            // Load the appropriate resource file initially
            UpdateResourceManager();
        }
        public void printText(string key, string color)
        {
            AnsiConsole.MarkupLine($"[{color}]{mgr.GetString(key)}[/]");
        }
        public void printText(string key)
        {
            AnsiConsole.MarkupLine(mgr.GetString(key));
        }
        private void UpdateResourceManager()
        {
            mgr = new System.Resources.ResourceManager($"EasySave.ressources.{AppSettings.Language}Resource", System.Reflection.Assembly.GetExecutingAssembly());
        }
        public void displayWelcomeView() 
        {

            AnsiConsole.MarkupLine("[red]*************************************************************************************[/]");
            AnsiConsole.MarkupLine("[red]***[/]                              Welcome to EasySave                              [red]***[/]");
            AnsiConsole.MarkupLine("[red]*************************************************************************************[/]");
            AnsiConsole.MarkupLine("[red]***[/]                      You can find the full source coude here                  [red]***[/]");
            AnsiConsole.MarkupLine("[red]***[/]                              once we put it on public                         [red]***[/]");
            AnsiConsole.MarkupLine("[red]***[/]                   [bold purple3]https://github.com/AbdelilahMeddahi/EasySave                [/][red]***[/]");
            AnsiConsole.MarkupLine($"[red]***[/]                you are using the system language which is: [green]{AppSettings.Language}[/]            [red]***[/]");
        }

        public int displayXMLorJSON()
        {
            Dictionary<string, int> optionsMap = new Dictionary<string, int>();
            optionsMap["XML"] = 1;
            optionsMap["JSON"] = 2;
            // Display the menu

            AnsiConsole.MarkupLine("[red]*************************************************************************************[/]");
            AnsiConsole.MarkupLine("[red]***                                    Menu                                       ***[/]");
            AnsiConsole.MarkupLine("[red]*************************************************************************************[/]");            // Ask for the user's for backup type
            var option = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title(mgr.GetString("optiontext"))
                    .AddChoices(new[] {
            "XML","JSON"
                    }));
            return optionsMap[option];

        }
        public int displayHomeView()
        {
            Dictionary<string, int> optionsMap = new Dictionary<string, int>();
            optionsMap[mgr.GetString("option1")] = 1;
            optionsMap[mgr.GetString("option2")] = 2;
            optionsMap[mgr.GetString("option3")] = 3;
            optionsMap[mgr.GetString("option4")] = 4;
            optionsMap[mgr.GetString("option5")] = 5;
            optionsMap[mgr.GetString("option6")] = 6;
            optionsMap[mgr.GetString("option7")] = 7;
            optionsMap[mgr.GetString("option8")] = 8;
            // Display the menu
             
            AnsiConsole.MarkupLine("[red]*************************************************************************************[/]");
            AnsiConsole.MarkupLine("[red]***                                    Menu                                       ***[/]");
            AnsiConsole.MarkupLine("[red]*************************************************************************************[/]");            // Ask for the user's for backup type
            var option = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title(mgr.GetString("optiontext"))
                    .AddChoices(new[] {
            mgr.GetString("option1"),mgr.GetString("option2"),mgr.GetString("option3"),mgr.GetString("option4"),mgr.GetString("option5"),mgr.GetString("option6"),mgr.GetString("option7"),mgr.GetString("option8"),
                    }));
            return optionsMap[option];            

        }
        public void displayBackupJobs(List<BackupJobModel> jobs)
        {
            if (jobs.Count == 0)
            {
                AnsiConsole.MarkupLine($"[{AppSettings.errorTextColor}]No backup jobs found[/]");
                return;
            }
            // Create a table
            int i = 0;
            //Table().Centered() to center the table later;
            var table = new Table();
            table.Border(TableBorder.DoubleEdge);
            AnsiConsole.Live(table).Start( ctx => { 
                table.AddColumn($"[{AppSettings.tableHeaderColor}]ID[/]");
                ctx.Refresh();
                Thread.Sleep(600);
                table.AddColumn($"[{AppSettings.tableHeaderColor}]Name[/]");
                ctx.Refresh();
                Thread.Sleep(600);
                table.AddColumn($"[{AppSettings.tableHeaderColor}]Source[/]");
                ctx.Refresh();
                Thread.Sleep(600); table.AddColumn($"[{AppSettings.tableHeaderColor}]Destination[/]");
                ctx.Refresh();
                Thread.Sleep(600); table.AddColumn($"[{AppSettings.tableHeaderColor}]Type[/]");
                ctx.Refresh();
                Thread.Sleep(600);                // create rows for each backup job
                foreach (BackupJobModel backupJob in jobs)
                {
                    table.AddRow(i.ToString(), backupJob.name, backupJob.sourceDirectory, backupJob.destinationDirectory, backupJob.backupType);
                    ctx.Refresh();
                    Thread.Sleep(1000); i++;
                }
            });
            
            //AnsiConsole.Write(table);
        }
        public string displayCreateBackupJobView()
        {
            // Ask for the user's for backup type
            var backupType = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title(mgr.GetString("selectbacukptypetext"))
                    .AddChoices(new[] {
            "Full", "Differential",
                    }));
            return backupType;
            /*Console.WriteLine("");
            Console.WriteLine("1. Full");
            Console.WriteLine("2. Differential");*/
        }

        public void displayRunBackupJobView()
        {
            Console.WriteLine("Please select a backup job:");
        }

        public void displayEditBackupJobView()
        {
            Console.WriteLine("Please enter the number of the backup job to edit:");
        }

        public void displayDeleteBackupJobView()
        {
            Console.WriteLine("Please enter the number of the backup job to delete:");
        }

        public string displayChangeLanguageView()
        {
            // Ask for the user's for backup type
            var language = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Please select a [green]Language[/]")
                    .AddChoices(new[] {
            "English/Anglais", "French/Français",
                    }));
            return language;
        }
        public void displayExitView()
        {
            Console.WriteLine("Goodbye!");
        }

        public void displayBackupJobCreatedView()
        {
            Console.WriteLine();
            Console.Clear();   
            AnsiConsole.MarkupLine("[yellow4_1]Backup job created![/]");
            Console.WriteLine();
        }

        public void displayBackupJobEditedView()
        {
            Console.WriteLine("Backup job edited!");
        }

        public void displayDeleteConfirmationView(ProgramModel model,int index)
        {
            Console.WriteLine("Are you sure that you want to delete");
            Console.WriteLine("the backup job of name :" + model.backupJobList[index].name + "?");
            Console.WriteLine("1. Yes");
            Console.WriteLine("2. No");

        }
        public void displayRunConfirmationView(ProgramModel model, int index)
        {
            Console.WriteLine("Are you sure that you want to run");
            Console.WriteLine("the backup job of name :" + model.backupJobList[index].name + "?");
            Console.WriteLine("1. Yes");
            Console.WriteLine("2. No");

        }

        public void displayBackupJobDeletedView()
        {
            Console.WriteLine("Backup job deleted!");
        }

        public void displayBackupJobRunView()
        {
            Console.WriteLine("Backup job run!");
        }
        
    }
}
