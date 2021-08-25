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

        private static int procedureId = 0;
        public static int ProcedureId
        {
            get
            {
                procedureId++;
                return procedureId;
            }
            set
            {
                procedureId = value;
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

            #pragma warning disable CA1416 // Validate platform compatibility
            eventLog1 = new EventLog();
            if (!EventLog.SourceExists(SourceName))
            {
                EventLog.CreateEventSource(SourceName, logName);
            }
            eventLog1.Source = SourceName;
            eventLog1.Log = logName;
            #pragma warning restore CA1416 // Validate platform compatibility

        }


        public static void WarnLogEntry(string message)
        {
            try
            {
                SharedEntry();
                lock (eventLog1)
                {
                    eventLog1.WriteEntry(message, EventLogEntryType.Warning, EventId++);
                }
            }
            catch (OverflowException ofex)
            {
                EventId = 1;
                InfoLogEntry(message);
            }

        }

        public static void InfoLogEntry(string message)
        {
            try
            {
                SharedEntry();

                lock (eventLog1)
                {
                    eventLog1.WriteEntry(message, EventLogEntryType.Information, EventId++);
                }
            }
            catch (OverflowException ofex)
            {
                EventId = 1;
                InfoLogEntry(message);
            }
        }

        public static void ErrorLogEntry(Exception ex, string methodName)
        {
            SharedEntry();

            string textError = string.Format("************ this error occurred in method: {0}", methodName) + " ************";
            textError += "\r\n" + ex.Message;


            try
            {
                var innerException = ex.InnerException;
                while (innerException != null)
                {
                    textError += " ==> this error have InnerException ==> " + innerException;
                    innerException = innerException.InnerException;
                }

                textError += "\r\n StackTrace: \r\n" + ex.StackTrace;
                lock (eventLog1)
                {
                    eventLog1.WriteEntry(textError, EventLogEntryType.Error, EventId++);
                }
            }
            catch (OverflowException ofex)
            {
                EventId = 1;
                ErrorLogEntry(ex, methodName);
            }
        }

        public static void ErrorLogEntry(string message)
        {
            SharedEntry();

            try
            {
                lock (eventLog1)
                {
                    eventLog1.WriteEntry(message, EventLogEntryType.Error, EventId++);
                }
            }
            catch (OverflowException ofex)
            {
                EventId = 1;
                ErrorLogEntry(message);
            }
        }

        private static void SharedEntry()
        {
            if (SourceName == null || LogName == null)
            {
                throw new Exception("you must initialize first!");
            }
        }

   
    }
}
