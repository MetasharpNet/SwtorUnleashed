using System;
using System.Diagnostics;
using System.IO;

namespace SwtorUnleashed.Model
{
    public class Log
    {
        private const string SwtorLogFile = "SwtorUnleashed.log";

        private static void Message(StreamWriter sw, string message)
        {
            sw.WriteLine(message);
            Trace.WriteLine(message);
        }

        public static void Debug(string message)
        {
            if (Setup.Settings != null && Setup.Settings.Log)
            {
                var stackTrace    = new StackTrace();
                var callingMethod = stackTrace.GetFrame(1).GetMethod();
                message           = ((callingMethod.DeclaringType != null) ? callingMethod.DeclaringType.Name + "." : "") +
                                    callingMethod.Name + "() " + message;
                var sw            = new StreamWriter(SwtorLogFile, true);
                Message(sw, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " [DEBUG]   " + message);
                sw.Close();
            }
        }

        public static void Error(string message)
        {
            if (Setup.Settings != null && Setup.Settings.Log)
            {
                var stackTrace    = new StackTrace();
                var callingMethod = stackTrace.GetFrame(1).GetMethod();
                message           = ((callingMethod.DeclaringType != null) ? callingMethod.DeclaringType.Name + "." : "") +
                                    callingMethod.Name + "() " + message;
                var sw            = new StreamWriter(SwtorLogFile, true);
                Message(sw, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " [ERROR]   " + message);
                sw.Close();
            }
        }

        public static void Error(Exception ex)
        {
            if (Setup.Settings != null && Setup.Settings.Log)
            {
                var stackTrace    = new StackTrace();
                var callingMethod = stackTrace.GetFrame(1).GetMethod();
                var message       = ((callingMethod.DeclaringType != null) ? callingMethod.DeclaringType.Name + "." : "") +
                                    callingMethod.Name + "() Exception : " + ex.Message;
                var sw            = new StreamWriter(SwtorLogFile, true);
                Message(sw, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " [ERROR]   " + message);
                sw.Close();
            }
        }

        public static void Info(string message)
        {
            if (Setup.Settings != null && Setup.Settings.Log)
            {
                var stackTrace    = new StackTrace();
                var callingMethod = stackTrace.GetFrame(1).GetMethod();
                message           = ((callingMethod.DeclaringType != null) ? callingMethod.DeclaringType.Name + "." : "") +
                                    callingMethod.Name + "() " + message;
                var sw            = new StreamWriter(SwtorLogFile, true);
                Message(sw, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " [INFO]    " + message);
                sw.Close();
            }
        }

        public static void Method(params object[] argsValues)
        {
            if (Setup.Settings != null && Setup.Settings.Log)
            {
                var stackTrace    = new StackTrace();
                var callingMethod = stackTrace.GetFrame(1).GetMethod();
                var message       = ((callingMethod.DeclaringType != null) ? callingMethod.DeclaringType.Name + "." : "") +
                                    callingMethod.Name + "(";
                var args          = callingMethod.GetParameters();
                for (int i = 0; i < args.Length; ++i)
                {
                    if (argsValues.Length > i)
                    {
                        message += args[i].Name + "=" + (argsValues[i] ?? "null");
                        if (i < args.Length - 1)
                            message += ", ";
                    }
                }
                message += ")";
                var sw  = new StreamWriter(SwtorLogFile, true);
                Message(sw, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " [METHOD]  " + message);
                sw.Close();
            }
        }

        public static void User(params object[] argsValues)
        {
            if (Setup.Settings != null && Setup.Settings.Log)
            {
                var stackTrace    = new StackTrace();
                var callingMethod = stackTrace.GetFrame(1).GetMethod();
                var message       = ((callingMethod.DeclaringType != null) ? callingMethod.DeclaringType.Name + "." : "") +
                                    callingMethod.Name + "(";
                var args          = callingMethod.GetParameters();
                for (int i = 0; i < args.Length; ++i)
                {
                    if (argsValues.Length > i)
                    {
                        message += args[i].Name + "=" + (argsValues[i] ?? "null");
                        if (i < args.Length - 1)
                            message += ", ";
                    }
                }
                message += ")";
                var sw  = new StreamWriter(SwtorLogFile, true);
                Message(sw, "");
                Message(sw, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " [USER]    " + message);
                sw.Close();
            }
        }

        public static void Warning(Exception ex)
        {
            if (Setup.Settings != null && Setup.Settings.Log)
            {
                var stackTrace    = new StackTrace();
                var callingMethod = stackTrace.GetFrame(1).GetMethod();
                var message       = ((callingMethod.DeclaringType != null) ? callingMethod.DeclaringType.Name + "." : "") +
                                    callingMethod.Name + "() Exception : " + ex.Message;
                var sw            = new StreamWriter(SwtorLogFile, true);
                Message(sw, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " [WARNING]  " + message);
                sw.Close();
            }
        }

        public static void Warning(string message)
        {
            if (Setup.Settings != null && Setup.Settings.Log)
            {
                var stackTrace    = new StackTrace();
                var callingMethod = stackTrace.GetFrame(1).GetMethod();
                message           = ((callingMethod.DeclaringType != null) ? callingMethod.DeclaringType.Name + "." : "") +
                                    callingMethod.Name + "() " + message;
                var sw            = new StreamWriter(SwtorLogFile, true);
                Message(sw, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " [WARNING] " + message);
                sw.Close();
            }
        }
    }
}
