using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace TotallyAccurateModEngine
{
    public class MDebug : Singleton<MDebug>
    {
        private static StreamWriter _log;

        private static string _logPath;
        public static string LogPath 
        {
            get
            {
                return _logPath;
            }
            set
            {
                if (_logPath != value)
                {
                    string filePath = Path.Combine(_logPath, "log.txt");
                    string contents = File.ReadAllText(filePath);
                    File.Delete(filePath);
                    if (!Directory.Exists(value))
                        Directory.CreateDirectory(value);

                    string newPath = Path.Combine(value, "log.txt");

                    StreamWriter writer = File.CreateText(newPath);
                    writer.Write(contents);

                    _logPath = value;

                    _log = writer;
                }
            }
        }

        public MDebug()
        {
            Init();
        }

        private void Init()
        {
            LogPath = Assembly.GetExecutingAssembly().Location;
        }


        public static void Log(object message)
        {
            string _message = message.ToString();
            MDebug.Log(_message);
        }

        public static void Log(string message)
        {
            _log.WriteLine(message);
        }

        public static void Close()
        {
            _log.Close();
        }

    }
}
