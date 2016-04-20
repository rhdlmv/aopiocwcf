namespace AOPIOC.Wcf
{
    using EgoalTech.Commons.Logger;
    using System;
    using System.Diagnostics;

    public class StopwatchAttribute : OperationAttribute
    {
        public override void AfterInvoke(object instance, object[] inputs, object result)
        {
            Stopwatch stopwatch = WcfContext.Current.Get<Stopwatch>("STOPWATCH");
            this.LogStopwatch(stopwatch);
        }

        public override void BeforeInvoke(object instance, object[] inputs)
        {
            Debug.WriteLine("Stopwatch");
            Stopwatch stopwatch = new Stopwatch();
            WcfContext.Current.Set("STOPWATCH", stopwatch);
            stopwatch.Start();
        }

        private ILogger GetLogger()
        {
            ILogger logger = null;
            IObjectContainer objectContainer = ObjectContainerFactory.GetObjectContainer();
            if (objectContainer.IsRegistered(typeof(ILogger), "STOPWATCH_LOGGER"))
            {
                return objectContainer.Resolve<ILogger>("STOPWATCH_LOGGER");
            }
            if (objectContainer.IsRegistered(typeof(ILogger)))
            {
                logger = objectContainer.Resolve<ILogger>();
            }
            return logger;
        }

        private void LogStopwatch(Stopwatch stopwatch)
        {
            if (stopwatch != null)
            {
                try
                {
                    stopwatch.Stop();
                    long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
                    string message = string.Format("{0}: {1} ms", base.OperationDescription.Name, elapsedMilliseconds);
                    ILogger logger = this.GetLogger();
                    if (logger == null)
                    {
                        Debug.WriteLine(message);
                    }
                    else
                    {
                        logger.WriteLine(message);
                    }
                }
                catch
                {
                }
            }
        }

        public override void OnInvokeError(object instance, object[] inputs, object result)
        {
        }
    }
}

