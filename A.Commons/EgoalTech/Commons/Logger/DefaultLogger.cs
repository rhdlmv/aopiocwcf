namespace EgoalTech.Commons.Logger
{
    using log4net;
    using log4net.Appender;
    using log4net.Config;
    using log4net.Layout;
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;

    public class DefaultLogger : EgoalTech.Commons.Logger.Logger
    {
        private ILog log;

        public DefaultLogger(string name) : base(name)
        {
            this.log = null;
            this.FileLocation = "log";
        }

        public override void Debug(string msg)
        {
            throw new NotImplementedException();
        }

        private void Error(string msg)
        {
            if (base.EnableConsole)
            {
                Console.Write(msg);
            }
            if (base.EnableDebug)
            {
                System.Diagnostics.Debug.Write(msg);
            }
            if (base.EnableLog)
            {
                this.GetLog().Error(msg);
            }
        }

        public override void Error(Exception ex, string msg = null)
        {
            if (!string.IsNullOrWhiteSpace(msg))
            {
                this.Error(msg);
            }
            this.Error(base.GetExceptionDetail(ex));
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
            string pattern = "%date [%thread] %-5level %logger [%property{NDC}] - %message%newline";
            PatternLayout layout = new PatternLayout(pattern);
            RollingFileAppender appender = new RollingFileAppender {
                Layout = layout
            };
            StringBuilder builder = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(this.FileLocation))
            {
                builder.Append(this.FileLocation + "/");
            }
            builder.Append(base.Name + ".log");
            appender.File = builder.ToString();
            appender.MaxFileSize = 0x100000L;
            appender.MaxSizeRollBackups = -1;
            appender.RollingStyle = RollingFileAppender.RollingMode.Composite;
            appender.AppendToFile = true;
            appender.ImmediateFlush = true;
            appender.ActivateOptions();
            BasicConfigurator.Configure(appender);
            this.log = LogManager.GetLogger(base.Name);
            return this.log;
        }

        public override void Warn(string msg)
        {
            throw new NotImplementedException();
        }

        public override void Write(string msg)
        {
            if (base.EnableConsole)
            {
                Console.Write(msg);
            }
            if (base.EnableDebug)
            {
                System.Diagnostics.Debug.Write(msg);
            }
            if (base.EnableLog)
            {
                this.GetLog().Info(msg);
            }
        }

        public override void WriteLine(string msg)
        {
            if (base.EnableConsole)
            {
                Console.WriteLine(msg);
            }
            if (base.EnableDebug)
            {
                System.Diagnostics.Debug.WriteLine(msg);
            }
            if (base.EnableLog)
            {
                this.GetLog().Info(msg);
            }
        }

        public string FileLocation { get; set; }
    }
}

