namespace AOPIOC.Wcf
{
    using A.Commons.Logger;
    using log4net;
    using log4net.Appender;
    using log4net.Config;
    using log4net.Layout;
    using System;
    using System.Diagnostics;
    
    using System.Runtime.InteropServices;
    using System.Text;

    public class WcfLogger : ILogger
    {
        private ILog log = null;
        private string logFileLocation = "Log";
        private string logFilename = "service-log";

        public WcfLogger()
        {
            this.EnableDebugView = true;
            this.EnableConsole = true;
            this.EnableLog = true;
            this.EnableInfo = true;
        }

        public void Debug(string msg)
        {
            if (this.EnableDebug)
            {
                string message = "[DEBUG] " + msg;
                if (this.EnableDebugView)
                {
                    System.Diagnostics.Debug.WriteLine(message);
                }
                if (this.EnableConsole)
                {
                    Console.WriteLine(message);
                }
                if (this.EnableLog)
                {
                    try
                    {
                        this.GetLog().Debug(msg);
                    }
                    catch
                    {
                    }
                }
            }
        }

        public void Error(Exception ex, string message = null)
        {
            string str = string.IsNullOrEmpty(message) ? this.GetExceptionDetail(ex) : (message + "\r\n" + this.GetExceptionDetail(ex));
            if (this.EnableDebugView)
            {
                System.Diagnostics.Debug.WriteLine(str);
            }
            if (this.EnableConsole)
            {
                Console.WriteLine(str);
            }
            if (this.EnableLog)
            {
                try
                {
                    this.GetLog().Error(message, ex);
                }
                catch
                {
                }
            }
        }

        private string GetExceptionDetail(Exception ex)
        {
            if (ex == null)
            {
                return "";
            }
            string str = ex.Message + "\r\n" + ex.StackTrace;
            if (ex.InnerException != null)
            {
                str = str + this.GetExceptionDetail(ex.InnerException);
            }
            return str;
        }

        private ILog GetLog()
        {
            lock (this)
            {
                if (this.log != null)
                {
                    return this.log;
                }
            }
            string str = "%date [%thread] %-5level %logger [%property{NDC}] - %message%newline";
            PatternLayout layout = new PatternLayout(str);
            RollingFileAppender appender = new RollingFileAppender();
            appender.Layout = layout;
            StringBuilder builder = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(this.logFileLocation))
            {
                builder.Append(this.logFileLocation + "/");
            }
            builder.Append(this.logFilename + ".log");
            appender.File = builder.ToString();
            appender.MaxFileSize = 1024 * 1024;
            appender.MaxSizeRollBackups = -1;
            appender.RollingStyle = log4net.Appender.RollingFileAppender.RollingMode.Composite;
            appender.AppendToFile = true;
            appender.ImmediateFlush = true;
            appender.ActivateOptions();
            BasicConfigurator.Configure(appender);
            this.log = LogManager.GetLogger(this.logFilename);
            return this.log;
        }

        public void Warn(string msg)
        {
            if (this.EnableDebug)
            {
                string message = "[WARN] " + msg;
                if (this.EnableDebugView)
                {
                    System.Diagnostics.Debug.WriteLine(message);
                }
                if (this.EnableConsole)
                {
                    Console.WriteLine(message);
                }
                if (this.EnableLog)
                {
                    try
                    {
                        this.GetLog().Warn(msg);
                    }
                    catch
                    {
                    }
                }
            }
        }

        public void Write(string msg)
        {
            if (this.EnableInfo)
            {
                if (this.EnableDebugView)
                {
                    System.Diagnostics.Debug.Write(msg);
                }
                if (this.EnableConsole)
                {
                    Console.Write(msg);
                }
                if (this.EnableLog)
                {
                    try
                    {
                        this.GetLog().Info(msg);
                    }
                    catch
                    {
                    }
                }
            }
        }

        public void WriteLine(string msg)
        {
            if (this.EnableInfo)
            {
                if (this.EnableDebugView)
                {
                    System.Diagnostics.Debug.WriteLine(msg);
                }
                if (this.EnableConsole)
                {
                    Console.WriteLine(msg);
                }
                if (this.EnableLog)
                {
                    try
                    {
                        this.GetLog().Info(msg);
                    }
                    catch
                    {
                    }
                }
            }
        }

        public bool EnableConsole { get; set; }

        public bool EnableDebug { get; set; }

        public bool EnableDebugView { get; set; }

        public bool EnableInfo { get; set; }

        public bool EnableLog { get; set; }
    }
}

