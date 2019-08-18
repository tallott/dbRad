using System;
using System.Collections.Generic;
using System.Reflection;

namespace dbRad.Classes
{
    public partial class ApplicationEnviroment
    {
        public static string ApplicationDbFilePath()
        {
            string FileName;
            FileName = UserApplicationDirectory() + "ApplicationDb.xml";

            return FileName;
        }
        public static string UserFilePath()
        {
            string UserFilePath;
            UserFilePath = UserApplicationDirectory() + "User.xml";

            return UserFilePath;
        }
        public static string UserApplicationDirectory()
        {
            string Directory = null;

            Directory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            EnsureDirSlash(ref Directory);
            Directory += AssemblyName();
            EnsureDirSlash(ref Directory);

            return Directory;
        }
        public static string AssemblyName()
        {
            string AssemblyName = null;
            AssemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            return AssemblyName;
        }

        public static string ConnectionString(String databaseName)
        {
            string ConnectionString = Config.appDb.ToString();
            ConnectionString = ConnectionString.Replace("$DatabaseName$", databaseName);
            return ConnectionString;
        }


        public static void EnsureDirSlash(ref String Directory)
        {
            if (Directory != null)
            {
                if (!Directory.EndsWith("\\"))
                {
                    Directory += "\\";
                }
            }
        }

        public static string ApplicationMessage(String messageKey)
        {
            Dictionary<string, string> messageDictionary = new Dictionary<string, string>();
            string message = string.Empty;

            messageDictionary.Add("SELECT", @"Either choose a suitable filter to display the records your interested in
and select a record to edit or press 'New' to create a new record.");
            messageDictionary.Add("NEW", "Enter values for a new record and then press 'Save'");
            messageDictionary.Add("EDIT", "Edit values you want to change and then press 'Save'");
            //messageDictionary.Add("SELECT", "Select a record to eidt or press new to create a new record");
            try
            {
                message = messageDictionary[messageKey];
            }
            catch
            {
                message = "No Message";// string.Empty;
            }

            return message;
        }

    }
}
