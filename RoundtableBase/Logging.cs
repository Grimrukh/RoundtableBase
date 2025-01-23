namespace RoundtableBase;

/// <summary>
/// Static class with logging settings and methods.
/// </summary>
public static class Logging
{
    static bool DoDebugPrint => true;
    
    static string? LogFile;
    
    static string Now => DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    
    public static void SetLogFile(string? logFile)
    {
        LogFile = logFile;
        
        if (LogFile == null)
            return;

        // Clear log file contents.
        try
        {
            File.WriteAllText(LogFile, "");
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[{Now}]   ERROR: Failed to clear log file: {ex.Message}");
            Console.ResetColor();
        }
    }

    public static void Debug(string msg, ConsoleColor? color = null)
    {
        if (!DoDebugPrint) return;

        string now = Now;
        
        if (LogFile != null)
        {
            try
            {
                File.AppendAllText(LogFile, $"[{now}]   DEBUG: {msg}\n");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[{now}]   ERROR: Failed to write Debug to log file: {ex.Message}");
                Console.ResetColor();
            }
        }
        
        if (color != null)
            Console.ForegroundColor = color.Value;
        Console.WriteLine($"[{now}]   DEBUG: {msg}");
        if (color != null)
            Console.ResetColor();
    }
    
    public static void Info(string msg)
    {
        string now = Now;

        if (LogFile != null)
        {
            try
            {
                File.AppendAllText(LogFile, $"[{now}]    INFO: {msg}\n");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[{now}]   ERROR: Failed to write Info to log file: {ex.Message}");
                Console.ResetColor();
            }
        }
        
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine($"[{now}]    INFO: {msg}");
        Console.ResetColor();
    }
    
    public static void Warning(string msg)
    {
        string now = Now;

        if (LogFile != null)
        {
            try
            {
                File.AppendAllText(LogFile, $"[{now}] WARNING: {msg}\n");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[{now}]   ERROR: Failed to write Warning to log file: {ex.Message}");
                Console.ResetColor();
            }
        }
        
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"[{now}] WARNING: {msg}");
        Console.ResetColor();
    }
    
    public static void Error(string msg)
    {
        string now = Now;

        if (LogFile != null)
        {
            try
            {
                File.AppendAllText(LogFile, $"[{now}]   ERROR: {msg}\n");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[{now}]   ERROR: Failed to write Error to log file: {ex.Message}");
                Console.ResetColor();
            }
        }
        
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[{now}]   ERROR: {msg}");
        Console.ResetColor();
    }
}