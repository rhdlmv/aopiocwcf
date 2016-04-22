namespace A.Commons.Logger
{
    using System;
    using System.Runtime.InteropServices;

    public interface ILogger
    {
        void Debug(string msg);
        void Error(Exception ex, string msg = null);
        void Warn(string msg);
        void Write(string msg);
        void WriteLine(string msg);

        bool EnableConsole { get; set; }

        bool EnableDebug { get; set; }

        bool EnableLog { get; set; }
    }
}

