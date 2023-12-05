using Castle.DynamicProxy;
using System;
using System.IO;
using System.Linq;

public class LoggingInterceptor : IInterceptor
{
    private readonly string _logFilePath;

    public LoggingInterceptor()
    {
        _logFilePath = "interceptor.txt";
    }

    public void Intercept(IInvocation invocation)
    {
        try
        {
            var methodName = $"{invocation.TargetType.FullName}.{invocation.Method.Name}";
            var arguments = string.Join(", ", invocation.Arguments.Select(a => a != null ? a.ToString() : "null"));
            var logMessage = $"Calling method: {methodName} with arguments: {arguments}\n";

            Console.WriteLine(logMessage);
            File.AppendAllText(_logFilePath, logMessage);

            invocation.Proceed();

            logMessage = $"Method {methodName} executed. Return value: {invocation.ReturnValue}\n";
            Console.WriteLine(logMessage);
            File.AppendAllText(_logFilePath, logMessage);
        }
        catch (Exception ex)
        {
            var errorMessage = $"Exception in method {invocation.Method.Name}: {ex.Message}\n";
            Console.WriteLine(errorMessage);
            File.AppendAllText(_logFilePath, errorMessage);
            throw;
        }
    }
}
