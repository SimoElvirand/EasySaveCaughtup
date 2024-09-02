using EasySave.models;
using EasySave.utils;
using Spectre.Console;
using System;

namespace EasySave
{
    internal class BackupProgramController
    {
        private ProgramModel programModel;
        private HomeView homeView;
        private int userInputOption;

        private void createBackupJob() {
            //We create a backup job
            string backupType = homeView.displayCreateBackupJobView();
            homeView.printText("entersourcetext");
            string sourceDirectory = Console.ReadLine();
            homeView.printText("enterdestinationtext");
            string destinationDirectory = Console.ReadLine();
            homeView.printText("enternametext");
            string name = Console.ReadLine();
            BackupJobModel backupJob = new BackupJobModel(sourceDirectory, destinationDirectory, name, backupType);
            if (backupJob != null)
            {
                programModel.backupJobList.Add(backupJob);
                homeView.displayBackupJobCreatedView();

            }
            else
            {
                homeView.printText("backupcreatedsuccessfullytext");
            }
        }
        private void runBackupJob()
        {
            if (programModel.backupJobList.Count == 0)
            {
                AnsiConsole.MarkupLine($"[{AppSettings.errorTextColor}]There is no backup job to run![/]");
            }
            else
            {
                homeView.displayBackupJobs(programModel.backupJobList);
                homeView.displayRunBackupJobView();
                int idOfBackupJobToRun = int.Parse(Console.ReadLine());
                if (idOfBackupJobToRun > programModel.backupJobList.Count)
                {
                    homeView.printText("validnumber");
                }
                else
                {
                    homeView.displayRunConfirmationView(programModel, idOfBackupJobToRun);
                    string userChoice = Console.ReadLine();
                    if (userChoice == "1")
                    {
                        homeView.printText("xmlorjson");
                        int log_choice = homeView.displayXMLorJSON();
                        if (log_choice == 1)
                        {
                            programModel.backupJobList[idOfBackupJobToRun].runBackupJob(1);
                            homeView.displayBackupJobRunView();
                        }
                        else if (log_choice == 2)
                        {
                            programModel.backupJobList[idOfBackupJobToRun].runBackupJob(2);
                            homeView.displayBackupJobRunView();
                        }
                        else
                        {
                            homeView.printText("validnumber");
                        }
                        homeView.displayBackupJobRunView();
                    }
                    else
                    {
                        Console.WriteLine("The backup job has not been run");
                    }

                }
            }
            
        }

        private void editBackuptJob()
        {
            if (programModel.backupJobList.Count == 0)
            {
                homeView.printText("nobackuptoedit", AppSettings.errorTextColor);
            }
            else
            {
                homeView.displayBackupJobs(programModel.backupJobList);
                homeView.displayEditBackupJobView();
                int numberOfBackupJobToEdit = int.Parse(Console.ReadLine());
                if (numberOfBackupJobToEdit > programModel.backupJobList.Count)
                {
                    homeView.printText("validnumber");
                }
                else
                {
                    string backupType = homeView.displayCreateBackupJobView();
                    homeView.printText("entersourcetext");
                    string sourceDirectory = Console.ReadLine();
                    homeView.printText("enterdestinationtext");
                    string destinationDirectory = Console.ReadLine();
                    homeView.printText("enternametext");
                    string name = Console.ReadLine();
                    BackupJobModel backupJob = new BackupJobModel(sourceDirectory, destinationDirectory, name, backupType);
                    programModel.backupJobList[numberOfBackupJobToEdit] = backupJob;
                    homeView.displayBackupJobEditedView();
                }
            }

        }

        private void deleteBackupJob() {
            if (programModel.backupJobList.Count == 0)
            {
                AnsiConsole.MarkupLine($"[{AppSettings.errorTextColor}][/]");
            }
            else
            {
                homeView.displayDeleteBackupJobView();
                int numberOfBackupJobToDelete = int.Parse(Console.ReadLine());
                if (numberOfBackupJobToDelete > programModel.backupJobList.Count)
                {
                    homeView.printText("validnumber", AppSettings.errorTextColor);
                }
                else
                {
                    homeView.displayDeleteConfirmationView(programModel, numberOfBackupJobToDelete);
                    string userChoice = Console.ReadLine();
                    if (userChoice == "1")
                    {
                        programModel.backupJobList.RemoveAt(numberOfBackupJobToDelete);
                        homeView.displayBackupJobDeletedView();
                    }
                    else
                    {

                        Console.WriteLine("The backup job has not been deleted");
                    }

                }
            }
        }

        private void listAllBackupJobs() {
            if (programModel.backupJobList.Count == 0)
            {
                homeView.printText("nobackuptodelete", AppSettings.errorTextColor);
            }
            else
            {
                Console.WriteLine("There is " + programModel.returnNumberOfBackupJob() + " backup job(s)");
                //display the backup jobs in a table
                homeView.displayBackupJobs(programModel.backupJobList);
            }
        }

        private void changeLanguage() {
            Console.Clear();
            string language = homeView.displayChangeLanguageView();
            if (language == "English/Anglais")
            {
                AppSettings.Language = "English";
                this.homeView = new HomeView();
                Console.WriteLine("Language changed to english");
            }
            else if (language == "French/Français")
            {
                AppSettings.Language = "French";
                this.homeView = new HomeView();
                Console.WriteLine("Vous avez changé la langue en français");
            }
            else
            {
                Console.WriteLine("Please enter a valid language");
            }
        }
        public BackupProgramController()
        {
            Console.Title = "EasySave from ProSoft";
            //The model and the view can be instantiate in the controller
            programModel = new ProgramModel();
            homeView = new HomeView();
            homeView.displayWelcomeView();
            while (true)
            {
                userInputOption = homeView.displayHomeView();
                //Retrieving user input for menu
                switch (userInputOption)
                {
                    case 1:
                        
                        if (programModel.checkIfWeCanCreateBackupJob())
                        {
                            createBackupJob();
                        }
                        else
                        {
                            homeView.printText("morethan5createdtext");
                        }
                        break;
                    case 2:
                        //run backup job
                        runBackupJob();
                        break;
                    case 3:
                        //edit backup job
                        editBackuptJob();
                        break;
                    case 4:
                        //delete backup job
                        homeView.displayBackupJobs(programModel.backupJobList);
                        deleteBackupJob();
                        break;
                    case 5:
                        //list backup jobs
                        listAllBackupJobs();
                        break;
                    case 6:
                        //Clear the console
                        Console.Clear();                        
                        break;
                    case 7:
                        //change language
                        changeLanguage();
                        break;
                    case 8://Stop the programs
                        homeView.displayExitView();
                        Console.Clear();
                        Environment.Exit(0);
                        break;
                }
            }
            
        }

    }
}
