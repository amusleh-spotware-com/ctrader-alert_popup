using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace cAlgo.API.Alert.Utility
{
    public static class ExceptionTools
    {
        public static string GetDetail(Exception exception, bool addStackTrace = true, bool isInnerException = false, string intend = null)
        {
            StringBuilder stringBuilder = new StringBuilder();

            intend = intend ?? string.Empty;

            if (isInnerException)
            {
                stringBuilder.AppendLine();
                stringBuilder.Append(string.Format("{0}InnerException:", intend));
            }
            else
            {
                string systemType = Environment.Is64BitOperatingSystem ? "64-bit" : "32-bit";

                stringBuilder.AppendLine();
                stringBuilder.Append(string.Format("OS Version: {0}", Environment.OSVersion.VersionString));
                stringBuilder.AppendLine();
                stringBuilder.Append(string.Format("System Type: {0}", systemType));
            }

            stringBuilder.AppendLine();
            stringBuilder.Append(string.Format("{0}Source: {1}", intend, exception.Source));
            stringBuilder.AppendLine();
            stringBuilder.Append(string.Format("{0}Message: {1}", intend, exception.Message));
            stringBuilder.AppendLine();
            stringBuilder.Append(string.Format("{0}TargetSite: {1}", intend, exception.TargetSite));
            stringBuilder.AppendLine();
            stringBuilder.Append(string.Format("{0}Type: {1}", intend, exception.GetType()));
            stringBuilder.AppendLine();

            if (addStackTrace)
            {
                string stackTrace = GetStackTrace(exception, intend);

                stringBuilder.AppendLine(stackTrace);

                stringBuilder.AppendLine();
            }

            if (exception.InnerException != null)
            {
                string innerExceptionIntent = new string(' ', intend.Length + 4);

                string innerExceptionSummary = GetDetail(exception.InnerException, isInnerException: true, intend: innerExceptionIntent);

                stringBuilder.Append(innerExceptionSummary);
            }

            return stringBuilder.ToString();
        }

        public static string GetStackTrace(Exception exception, string intend = null)
        {
            if (string.IsNullOrEmpty(exception.StackTrace))
            {
                return string.Empty;
            }

            StackTrace stackTrace = new StackTrace(exception, true);

            StackFrame[] frames = stackTrace.GetFrames();

            if (frames == null || !frames.Any())
            {
                return string.Empty;
            }

            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append(string.Format("{0}StackTrace:", intend));
            stringBuilder.AppendLine();

            string tracesIntend = new string(' ', string.IsNullOrEmpty(intend) ? 4 : intend.Length + 4);

            foreach (StackFrame stackFram in frames)
            {
                string fileName = stackFram.GetFileName();

                fileName = !string.IsNullOrEmpty(fileName)
                    ? fileName.Substring(fileName.LastIndexOf(@"\", StringComparison.InvariantCultureIgnoreCase) + 1)
                    : string.Empty;

                string stackFrameText = string.Format("{0}File: {1} | Line: {2} | Col: {3} | Offset: {4} | Method: {5}", tracesIntend, fileName,
                    stackFram.GetFileLineNumber(), stackFram.GetFileColumnNumber(), stackFram.GetILOffset(), stackFram.GetMethod());

                stringBuilder.Append(stackFrameText);

                stringBuilder.AppendLine();
            }

            return stringBuilder.ToString();
        }
    }
}