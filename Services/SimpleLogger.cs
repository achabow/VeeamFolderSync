using System;
using System.IO;

namespace VeeamFolderSync.Services
{
    // Tiny logger: console + file, thread-safe.
    public sealed class SimpleLogger
    {
        private readonly string _file;
        private readonly object _gate = new();

        public SimpleLogger(string logFile)
        {
            _file = logFile;
            Directory.CreateDirectory(Path.GetDirectoryName(_file)!);
        }

        public void Write(string text)
        {
            var line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}  {text}";
            lock (_gate)
            {
                Console.WriteLine(line);
                File.AppendAllText(_file, line + Environment.NewLine);
            }
        }
    }
}