using System;
using System.Reflection;

namespace dbRad
{
    public partial class Env
    {
        public static string ControlDbFilePath()
        {
            string FileName;
            FileName = UserApplicationDirectory() + "ControlDb.xml";

            return FileName;
        }
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
        //public static string ApplicationDirectory()
        //{
        //    string Directory = null;

        //    Directory = AppDomain.CurrentDomain.RelativeSearchPath;

        //    if (Directory == null)
        //    {
        //        Directory = AppDomain.CurrentDomain.BaseDirectory;
        //    }

        //    EnsureDirSlash(ref Directory);

        //    return Directory;
        //}

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

    }
}
