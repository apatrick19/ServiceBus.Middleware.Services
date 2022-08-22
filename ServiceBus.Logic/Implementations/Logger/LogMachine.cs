using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus.Logic.Implementations.Logger
{
    public class LogMachine
    {
        public static void LogInformation(string ClassName,string MethodName, string Message)
        {
            try
            {
                Trace.TraceInformation($"ClassName: {ClassName}; MethodName: {MethodName}; Message: {Message}");
            }
            catch (Exception ex)
            {
                Trace.TraceError($"An error occurred in logger class {ex}");
            }
        }

        public static void LogInformation(string ClassName, string MethodName, string Message, Exception e)
        {
            try
            {
                Trace.TraceInformation($"ClassName: {ClassName}; MethodName: {MethodName}; Message: {Message} Error {e.Message}");
            }
            catch (Exception ex)
            {
                Trace.TraceError($"An error occurred in logger class {ex}");
            }
        }
    }
}
