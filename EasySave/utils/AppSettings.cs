using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace EasySave.utils
{
    class AppSettings
    {
        private static string _language;
        static public string Language
        {
            get
            {
                if (_language == null)
                {
                    // Initialize the language based on the current culture if not set
                    CultureInfo ci = CultureInfo.InstalledUICulture;
                    _language = ci.Name == "fr-FR" ? "French" : "English";
                }
                return _language;
            }
            set
            {
                _language = value;
            }
        }
        private static int _consoleWidth = 85;
        public static int ConsoleWidth
        {
            get
            {
                return _consoleWidth;
            }
        }
        static public int backupJobNumberAllowed = 5;
        static public string tableHeaderColor = "gold3";
        static public string errorTextColor = "red";

    }
}
