using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using System.IO;

namespace RDCore
{
    public static class Logger
    {
        private const string METHOD = "Method: ";
        private const string LINE = "Line: ";
        private const string MESSAGE_TYPE = "MessageType: ";
        private const string MESSAGE = "Message";
        private const string ERROR = "Error";
        private const string TRACE = "Trace";
        private const string SPACE = " ";

        /// <summary>
        /// Instance of LogWritter
        /// </summary>
        private static LogWriter messageWriter = EnterpriseLibraryContainer.Current.GetInstance<LogWriter>();

        /// <summary>
        /// Write error | message | trace to the file location based on MessageType
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageType"></param>
        /// <param name="type"></param>
        public static void Write(string message, MessageType messageType, Type type)
        {
            switch (messageType)
            {
                case MessageType.Error:
                    WriteError(message, messageType, type);
                    break;
                case MessageType.Information:
                    WriteMessage(message, messageType, type);
                    break;
                case MessageType.Warning:
                    WriteMessage(message, messageType, type);
                    break;
                case MessageType.Success:
                    WriteMessage(message, messageType, type);
                    break;
                case MessageType.Trace:
                    WriteTrace(message, messageType, type);
                    break;
                case MessageType.Log:
                    WriteLog(message, messageType, type);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Write message to the File location
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageType"></param>
        /// <param name="type"></param>
        static void WriteMessage(string message, MessageType messageType, Type type)
        {
            CallMethod method = new CallMethod(type);
            string location = System.Environment.NewLine + METHOD + method.MethodNameFull +
                              System.Environment.NewLine + LINE + method.LineNumber +
                              System.Environment.NewLine + MESSAGE_TYPE + messageType;
            messageWriter.Write(message + SPACE + location, MESSAGE);
        }

        /// <summary>
        /// Write error to the File location
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageType"></param>
        /// <param name="type"></param>
        static void WriteError(string message, MessageType messageType, Type type)
        {
            CallMethod method = new CallMethod(type);
            string location = System.Environment.NewLine + METHOD + method.MethodNameFull +
                              System.Environment.NewLine + LINE + method.LineNumber +
                              System.Environment.NewLine + MESSAGE_TYPE + messageType;
            messageWriter.Write(message + SPACE + location, ERROR);
        }

        /// <summary>
        /// Write trace to the File location
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageType"></param>
        /// <param name="type"></param>
        static void WriteTrace(string message, MessageType messageType, Type type)
        {
            CallMethod method = new CallMethod(type);
            string location = System.Environment.NewLine + METHOD + method.MethodNameFull +
                              System.Environment.NewLine + LINE + method.LineNumber +
                              System.Environment.NewLine + MESSAGE_TYPE + messageType;
            messageWriter.Write(message + SPACE + location, TRACE);
        }

        /// <summary>
        /// Write message to the File location
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageType"></param>
        /// <param name="type"></param>
        static void WriteLog(string message, MessageType messageType, Type type)
        {
            CallMethod method = new CallMethod(type);
            messageWriter.Write(message, MESSAGE);
        }

        /// <summary>
        /// Write trace to the File location
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageType"></param>
        /// <param name="type"></param>
        /// <param name="startTime"></param>
        public static void WriteLog(string message, MessageType messageType, Type type, DateTime startTime, string filePath)
        {
            DateTime endTime = DateTime.Now;
            TimeSpan ts = endTime.Subtract(startTime);          
            CallMethod method = new CallMethod(type);
            string location = System.Environment.NewLine + METHOD + method.MethodNameFull +
                             System.Environment.NewLine + LINE + method.LineNumber +
                             System.Environment.NewLine + MESSAGE_TYPE + messageType;
            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);
            else if (!Directory.Exists(filePath + "\\Log"))
                Directory.CreateDirectory(filePath + "\\Log");
            StreamWriter sw = new StreamWriter(filePath + "\\Log\\" + messageType + ".log", true);
            sw.WriteLine("-------------------------------------------------------------------------------------------------------------");
            sw.WriteLine(message + System.Environment.NewLine + "Start Time :" + startTime.ToString() + System.Environment.NewLine + "End Time :" + endTime.ToString() + System.Environment.NewLine + "Time Spent:" + ts.ToString() + location);
            sw.WriteLine("-------------------------------------------------------------------------------------------------------------");
            sw.Close();
        }  
    }
}
