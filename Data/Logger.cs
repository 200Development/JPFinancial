using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace JPFData
{
    public class Logger
    {
        private readonly string _folderPath;
        private readonly string _fileName;
        private static volatile Logger _instance;
        private static readonly object Locker = new object();


        private Logger()
        {
            _fileName = Assembly.GetExecutingAssembly().GetName().Name;
            _folderPath = Path.Combine(Path.GetTempPath(), $"JP Financial Logs/{_fileName}");
            CurrentVerbosity = Verbosity.All;
            MaxFileSizeBytes = 1048576; // 1MB
        }

        public void Error(Exception exception)
        {
            var errorId = $"{new Random(DateTime.Now.Millisecond).Next()}"; // creates unique GUID

            Write(errorId, LogType.Error); // Log ErrorId to reference from Info & Debug logs
            Write(exception.StackTrace + Environment.NewLine + exception.Message + Environment.NewLine +
                  exception.InnerException?.Message, LogType.Error);

            //Log error ID in Info & Debug logs
            Write($"ERROR [{errorId}]", LogType.Info);
#if DEBUG
            Write($"ERROR [{errorId}]", LogType.Debug);
#endif
        }

        public void Debug(string message)
        {
#if DEBUG
            Write(message, LogType.Debug);
#endif
        }

        public void Info(string message, Verbosity verbosity = Verbosity.Low)
        {
            if (CurrentVerbosity >= verbosity)
                Write(message, LogType.Info);
        }

        public void Calculation(string message, Verbosity verbosity = Verbosity.High)
        {
            if(CurrentVerbosity >= verbosity)
                Write(message, LogType.Calculation);
        }

        public void DataFlow(string message, Verbosity verbosity = Verbosity.All)
        {
            if (CurrentVerbosity >= verbosity)
                Write(message, LogType.DataFlow);
        }

        private void Write(string message, LogType loggerType, bool includeTimestamp = true)
        {
            lock (Locker)
            {
                try
                {
                    Directory.CreateDirectory(_folderPath);
                    var filePath = Path.Combine(_folderPath, $"{_fileName}{loggerType.ToString()}.log");
                    var oldFilePath = filePath.Replace(".", "_archive.");
                    if (File.Exists(filePath))
                    {
                        if (new FileInfo(filePath).Length > MaxFileSizeBytes)
                        {
                            if (File.Exists(oldFilePath))
                            {
                                File.Delete(oldFilePath);
                            }

                            File.Copy(filePath, oldFilePath);
                            File.WriteAllText(filePath, string.Empty);
                        }
                    }

                    File.AppendAllText(filePath, FormatMessage(message, includeTimestamp));
                }
                catch (Exception e)
                {
                   throw new Exception($"Error logging for plugin: {_fileName}", e);
                }
            }
        }

        private string FormatMessage(string message, bool includeTimestamp)
        {
            var frame = new StackFrame(3);
            var method = frame.GetMethod();
            var name = method.DeclaringType?.Name;

            var timestamp = $"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff", CultureInfo.InvariantCulture)}";
            var version = $"{Assembly.GetExecutingAssembly().GetName().Version}";

            return includeTimestamp
                ? $"{Environment.NewLine}{timestamp} [{version}] {name} :: {message}"
                : $"{Environment.NewLine}[{version}] {name} :: {message}";
        }

        public Verbosity CurrentVerbosity { get; set; }
        public long MaxFileSizeBytes { get; set; }


        public static Logger Instance
        {
            get
            {
                if (_instance != null) return _instance;
                lock (Locker)
                {
                    return _instance ?? (_instance = new Logger());
                }
            }
        }
    }

    public enum LogType
    {
        Error,
        Info,
        Debug,
        Calculation,
        DataFlow
    }

    public enum Verbosity
    {
        Off,
        Low,
        Medium,
        High,
        All
    }
}
