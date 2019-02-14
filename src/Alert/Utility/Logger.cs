using System;

namespace cAlgo.API.Alert.Utility
{
    public class Logger
    {
        #region Fields

        private Action<string> _tracer;

        #endregion Fields

        public Logger(Action<string> tracer)
        {
            _tracer = tracer;

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        #region Methods

        public void Log(string message)
        {
            _tracer.Invoke(message);
        }

        public void Log(Exception exception)
        {
            string exceptionSummary = ExceptionTools.GetDetail(exception);

            _tracer.Invoke(exceptionSummary);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject != null)
            {
                Log(e.ExceptionObject as Exception);
            }
        }

        #endregion Methods
    }
}