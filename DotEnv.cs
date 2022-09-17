namespace PatientPortal
{
    using System;
    using System.IO;

    public static class DotEnv
    {
        public static void Load(string filePath)
        {
            if (!File.Exists(filePath))
                return;

            foreach (var line in File.ReadAllLines(filePath))
            {
                // Ignore commented and empty lines
                if (line.Length < 1 || line[0] == '#' )
                    continue;

                int delimiterIndex = line.IndexOf("=");
                // Ignore lines without '='
                if (delimiterIndex == -1)
                    continue;
                
                string envKey = line.Substring(0,delimiterIndex);
                string envValue = line.Substring(delimiterIndex+1);
                
                // Ignore any where key or value is not set
                if (envKey.Length == 0 || envValue.Length == 0)
                    continue;

                Environment.SetEnvironmentVariable(envKey, envValue);
            }
        }
    }
}