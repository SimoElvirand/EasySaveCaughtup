using EasySave.utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasySave.models
{
    internal class ProgramModel
    {

        private List<BackupJobModel> _backupJobList;
        public List<BackupJobModel> backupJobList { get => _backupJobList; set => _backupJobList = value; }
        
        public int returnNumberOfBackupJob()
        {
            return backupJobList.Count;
        }

        public bool checkIfWeCanCreateBackupJob()
        {
            return (backupJobList.Count < AppSettings.backupJobNumberAllowed);
        }

        public ProgramModel()
        {
            backupJobList = new List<BackupJobModel>();
        }
    }
}
