using System;
using System.IO;

namespace DataTransform
{
    public interface IErrorHandler
    {
        void LogError(string error);
        void SetLogFilePath(string path);
    }

    /// <summary>
    /// This class logs an errors into log file. 
    /// This error handler can have a complex messaging logic but the simplest one is writing a log file.
    /// Log file will be located at the same place as input and output files are.
    /// </summary>
    class ErrorHandler : IErrorHandler
    {
        private string _logFilePath;
        private const string DefaultFileName = "\\Log.txt";

        private void InitNewRun()
        {
            LogError("");
            LogError(string.Format("=============== {0} ===============", DateTime.Now));
        }

        public void SetLogFilePath(string path)
        {
            _logFilePath = path + DefaultFileName;
            InitNewRun();
        }

        public void LogError(string error)
        {
            using (StreamWriter file = new StreamWriter(_logFilePath, true))
            {
                file.WriteLine(error);
            }
        }
    }
}
