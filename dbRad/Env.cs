using System;

namespace dbRad
{
  public partial  class Env
    {
        public static string ControlDbFilePath()
        {
            string FileName;
            FileName = ApplicationDirectory() + "ControlDb.xml";

            return FileName;
        }
        public static string ApplicationDbFilePath()
        {
            string FileName;
            FileName = ApplicationDirectory() + "ApplicationDb.xml";

            return FileName;
        }
        public static string SettingsFilePath()
        {
            string FileName;
            FileName = ApplicationDirectory() + "Settings.xml";

            return FileName;
        }
        public static string ApplicationDirectory()
        {
            string Directory = null;

            Directory = AppDomain.CurrentDomain.RelativeSearchPath;

            if (Directory == null)
            {
                Directory = AppDomain.CurrentDomain.BaseDirectory;
            }

            EnsureDirSlash(ref Directory);

            return Directory;
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

    }
}
