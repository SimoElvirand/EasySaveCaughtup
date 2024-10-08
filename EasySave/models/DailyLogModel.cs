﻿using System;
using EasySave.utils;
using System.IO;
using System.Text.Json;
using System.Xml.Serialization;

namespace EasySave
{
    public class DailyLogModel
    {
        public string Name { get; set; }
        public string FileSource { get; set; }
        public string FileTarget { get; set; }
        public string FileSize { get; set; }
        public double FileTransferTime { get; set; }
        public string Time { get; set; }

        public static void JsonLogger(string name,string fileSource,string fileTarget, DateTime time, double fileTransferTime, long fileSize)
        {
            string path = Path.Combine("c:", "Log");
            FileHandling.CreateDirIfNotExist(path);
            string fileName = $"{DateTime.Now.Day}-{DateTime.Now.Month}-{DateTime.Now.Year}_Logs.json";
            //write on json 
            DailyLogModel m = new DailyLogModel(name, fileSource, fileTarget, fileTransferTime, time, fileSize);
            string jsonContent = JsonSerializer.Serialize(m, new JsonSerializerOptions { WriteIndented = true });
            File.AppendAllText(Path.Combine(path, fileName), $"{jsonContent},");
        }
        public static void XMLLogger(string name, string fileSource, string fileTarget, DateTime time, double fileTransferTime, long fileSize)
        {
            string path = Path.Combine("c:", "Log");
            FileHandling.CreateDirIfNotExist(path);
            string fileName = $"{DateTime.Now.Day}-{DateTime.Now.Month}-{DateTime.Now.Year}_Logs.xml";
            DailyLogModel m = new DailyLogModel(name, fileSource, fileTarget, fileTransferTime, time,  fileSize);
            XmlSerializer x = new XmlSerializer(m.GetType());
            using (TextWriter writer = new StreamWriter(Path.Combine(path, fileName),true))
            {
                x.Serialize(writer, m);
            }
        }
        //parameterless constructor for serialization
        public DailyLogModel() { }
        public DailyLogModel(string name, string source, string destination, double fileTransferTime, DateTime tim, long size)
        {
            this.Name = name;
            this.FileSource = source;
            this.FileTarget = destination;
            this.FileTransferTime = fileTransferTime;
            this.Time = tim.ToString();
            this.FileSize = size.ToString();
        }
    }
}