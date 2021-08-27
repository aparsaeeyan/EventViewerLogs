using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventViewerLogs
{
    public static class Logs
    {
        private static string SourceName = null;
        private static string LogName = null;
        private static EventLog eventLog1;

        public static int EventId { get; private set; }


        private static void CheckInit()
        {
            if (SourceName == null || LogName == null)
            {
                throw new Exception("You must first call the Init method.!");
            }
        }

        public static void Init(string sourceName, string logName)
        {
            if (SourceName != null && LogName != null)
            {
                throw new Exception("You have already called this method!");
            }

            SourceName = sourceName;
            LogName = logName;

            eventLog1 = new EventLog();
            if (!EventLog.SourceExists(SourceName))
            {
                EventLog.CreateEventSource(SourceName, LogName);
            }
            eventLog1.Source = SourceName;
            eventLog1.Log = LogName;

        }


        public static void WarnLogEntry(string message)
        {
            try
            {
                CheckInit();

                eventLog1.WriteEntry(message, EventLogEntryType.Warning, EventId++);
            }
            catch (ArgumentException)
            {
                EventId = 0;
                WarnLogEntry(message);
            }

        }

        public static void InfoLogEntry(string message)
        {
            try
            {
                CheckInit();

                eventLog1.WriteEntry(message, EventLogEntryType.Information, EventId++);
            }
            catch (ArgumentException)
            {
                EventId = 0;
                InfoLogEntry(message);
            }
        }

        public static void ErrorLogEntry(Exception ex, string methodName)
        {
            CheckInit();

            string textError = $"************ this error occurred in method: {methodName} ************ \r\n {ex.Message}";

            try
            {
                var innerException = ex.InnerException;
                while (innerException != null)
                {
                    textError += $" ==> this error have InnerException ==> {innerException}";
                    innerException = innerException.InnerException;
                }

                textError += $"\r\n StackTrace: \r\n {ex.StackTrace}";

                eventLog1.WriteEntry(textError, EventLogEntryType.Error, EventId++);
            }
            catch (ArgumentException)
            {
                EventId = 0;
                ErrorLogEntry(ex, methodName);
            }
        }

        public static void ErrorLogEntry(string message)
        {
            CheckInit();

            try
            {
                eventLog1.WriteEntry(message, EventLogEntryType.Error, EventId++);
            }
            catch (ArgumentException)
            {
                EventId = 0;
                ErrorLogEntry(message);
            }
        }




    }
}
