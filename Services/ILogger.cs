using System;

namespace MVVM.Core.Services
{
    public interface ILogger
    {
        void Log(string message);
        void LogWarning(string message);
        void LogError(string message);
        void LogException(Exception exception);
    }

    public abstract class Logger : ILogger
    {
        public static ILogger Instance { get; set; }

        public abstract void Log(string message);
        public abstract void LogWarning(string message);
        public abstract void LogError(string message);
        public virtual  void LogException(Exception exception) =>
            LogError(exception.ToString());

        protected Logger()
        {
            Instance = this;
        }
    }

    public static class StaticLogger
    {
        public static void Log(string message)
        {
            CheckLoggerInstance();
            Logger.Instance.Log(message);
        }

        public static void LogWarning(string message)
        {
            CheckLoggerInstance();
            Logger.Instance.LogWarning(message);
        }

        public static void LogError(string message)
        {
            CheckLoggerInstance();
            Logger.Instance.LogError(message);
        }

        public static void LogException(Exception exception)
        {
            CheckLoggerInstance();
            Logger.Instance.LogException(exception);
        }

        private static void CheckLoggerInstance()
        {
            if (Logger.Instance is null)
                throw new Exception("There is no instance of logger initialzied");
        }
    }

    public class ConsoleLogger : Logger
    {
        public override void Log(string message)
        {
            WriteWithColor(message, ConsoleColor.White);
        }

        public override void LogWarning(string message)
        {
            WriteWithColor(message, ConsoleColor.Yellow);
        }

        public override void LogError(string message)
        {
            WriteWithColor(message, ConsoleColor.Red);
        }

        private static void WriteWithColor(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
        }
    }
}
