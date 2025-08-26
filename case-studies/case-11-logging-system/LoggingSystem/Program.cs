using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Text.Json;

// SOLUTION 1: Interface Segregation Principle (ISP)
// Separate concerns into multiple interfaces

// Core logging interface - ALL loggers must implement
public interface ILogger
{
    void Log(LogLevel level, string message,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0);

    void LogException(Exception ex,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0);
}

// Optional capability interface - ONLY buffered loggers implement
public interface IFlushableLogger : ILogger
{
    void Flush();
    bool HasPendingLogs { get; }
}

// Optional capability interface - ONLY configurable loggers implement  
public interface IConfigurableLogger : ILogger
{
    void UpdateConfiguration(object config);
}

// Configuration Models
public class LoggingConfiguration
{
    public string Provider { get; set; } = "Console";
    public string FilePath { get; set; } = "logs/default.log";
    public int BufferSize { get; set; } = 100;
    public int FlushIntervalMs { get; set; } = 5000;
}

public class AppConfiguration
{
    public LoggingConfiguration Logging { get; set; } = new();
}

// Log Entry Model
public class LogEntry
{
    public DateTime Timestamp { get; set; }
    public LogLevel Level { get; set; }
    public string Message { get; set; }
    public string ClassName { get; set; }
    public string MemberName { get; set; }
    public int LineNumber { get; set; }
    public Exception Exception { get; set; }

    public override string ToString()
    {
        var location = $"{ClassName}.{MemberName}:{LineNumber}";
        if (Exception != null)
            return $"{Timestamp} [EXCEPTION] [{location}]: {Message} - {Exception.Message}";
        return $"{Timestamp} [{Level}] [{location}]: {Message}";
    }
}

// ✅ CONSOLE LOGGER - Only implements ILogger (LSP Compliant)
public class ConsoleLogger : ILogger
{
    private readonly object _consoleLock = new object();

    public void Log(LogLevel level, string message,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        var className = Path.GetFileNameWithoutExtension(filePath);
        var location = $"{className}.{memberName}:{lineNumber}";

        lock (_consoleLock)
        {
            Console.WriteLine($"{DateTime.Now} [{level}] [{location}]: {message}");
        }
    }

    public void LogException(Exception ex,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        var className = Path.GetFileNameWithoutExtension(filePath);
        var location = $"{className}.{memberName}:{lineNumber}";

        lock (_consoleLock)
        {
            Console.WriteLine($"{DateTime.Now} [EXCEPTION] [{location}]: {ex.Message}");
        }
    }
}

// ✅ FILE LOGGER - Implements both ILogger AND IFlushableLogger (LSP Compliant)
public class BufferedFileLogger : IFlushableLogger, IDisposable
{
    private readonly string _filePath;
    private readonly ConcurrentQueue<LogEntry> _logQueue;
    private readonly Timer _flushTimer;
    private readonly object _flushLock = new object();
    private readonly int _bufferSize;
    private volatile bool _disposed = false;

    public bool HasPendingLogs => !_logQueue.IsEmpty; // ISP compliance

    public BufferedFileLogger(string filePath, int bufferSize = 100, int flushIntervalMs = 5000)
    {
        _filePath = filePath;
        _bufferSize = bufferSize;
        _logQueue = new ConcurrentQueue<LogEntry>();

        var directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        _flushTimer = new Timer(AutoFlush, null, flushIntervalMs, flushIntervalMs);
    }

    public void Log(LogLevel level, string message,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        if (_disposed) return;

        var entry = new LogEntry
        {
            Timestamp = DateTime.Now,
            Level = level,
            Message = message,
            ClassName = Path.GetFileNameWithoutExtension(filePath),
            MemberName = memberName,
            LineNumber = lineNumber
        };

        _logQueue.Enqueue(entry);

        if (_logQueue.Count >= _bufferSize)
        {
            Flush();
        }
    }

    public void LogException(Exception ex,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        if (_disposed) return;

        var entry = new LogEntry
        {
            Timestamp = DateTime.Now,
            Level = LogLevel.Error,
            Message = "Exception occurred",
            ClassName = Path.GetFileNameWithoutExtension(filePath),
            MemberName = memberName,
            LineNumber = lineNumber,
            Exception = ex
        };

        _logQueue.Enqueue(entry);
        Flush(); // Flush immediately for exceptions
    }

    public void Flush()
    {
        if (_disposed || _logQueue.IsEmpty) return;

        lock (_flushLock)
        {
            try
            {
                var entries = new List<LogEntry>();
                while (_logQueue.TryDequeue(out var entry))
                {
                    entries.Add(entry);
                }

                if (entries.Count > 0)
                {
                    var logText = string.Join(Environment.NewLine, entries.Select(e => e.ToString())) + Environment.NewLine;
                    File.AppendAllText(_filePath, logText);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"File logging failed: {ex.Message}");
            }
        }
    }

    private void AutoFlush(object state)
    {
        Flush();
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;
            _flushTimer?.Dispose();
            Flush();
        }
    }
}

// ✅ CLOUD LOGGER - Only implements ILogger (LSP Compliant)
public class CloudLogger : ILogger
{
    public void Log(LogLevel level, string message,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        var className = Path.GetFileNameWithoutExtension(filePath);
        var location = $"{className}.{memberName}:{lineNumber}";

        // In real implementation, this would send to cloud service
        Console.WriteLine($"☁️ [Cloud] {DateTime.Now} [{level}] [{location}]: {message}");
    }

    public void LogException(Exception ex,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        var className = Path.GetFileNameWithoutExtension(filePath);
        var location = $"{className}.{memberName}:{lineNumber}";

        Console.WriteLine($"☁️ [Cloud] {DateTime.Now} [EXCEPTION] [{location}]: {ex.Message}");
    }
}

// ✅ COMPOSITE LOGGER - Implements IFlushableLogger only if any child supports it
public class CompositeLogger : IFlushableLogger, IDisposable
{
    private readonly List<ILogger> _loggers;
    private readonly List<IFlushableLogger> _flushableLoggers;

    public bool HasPendingLogs => _flushableLoggers.Any(l => l.HasPendingLogs);

    public CompositeLogger(params ILogger[] loggers)
    {
        _loggers = new List<ILogger>(loggers);
        _flushableLoggers = _loggers.OfType<IFlushableLogger>().ToList();
    }

    public void Log(LogLevel level, string message,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        foreach (var logger in _loggers)
        {
            try
            {
                logger.Log(level, message, memberName, filePath, lineNumber);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Logger failed: {ex.Message}");
            }
        }
    }

    public void LogException(Exception ex,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        foreach (var logger in _loggers)
        {
            try
            {
                logger.LogException(ex, memberName, filePath, lineNumber);
            }
            catch (Exception logEx)
            {
                Console.WriteLine($"Logger failed: {logEx.Message}");
            }
        }
    }

    public void Flush()
    {
        // Only flush loggers that actually support flushing
        foreach (var flushableLogger in _flushableLoggers)
        {
            try
            {
                flushableLogger.Flush();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Logger flush failed: {ex.Message}");
            }
        }
    }

    public void Dispose()
    {
        foreach (var logger in _loggers.OfType<IDisposable>())
        {
            logger.Dispose();
        }
    }
}

// Log Level Enum
public enum LogLevel
{
    Info,
    Warning,
    Error
}

// ✅ ENHANCED LOGGER MANAGER - Now LSP Compliant
public sealed class LoggerManager
{
    private static readonly Lazy<LoggerManager> _instance = new Lazy<LoggerManager>(() => new LoggerManager());
    private readonly ILogger _logger;

    public static LoggerManager Instance => _instance.Value;
    public static ILogger Logger => Instance._logger;

    // Type-safe access to flushable logger if supported
    public static IFlushableLogger FlushableLogger => Instance._logger as IFlushableLogger;

    private LoggerManager()
    {
        _logger = CreateFromConfiguration();
    }

    private static ILogger CreateFromConfiguration()
    {
        var config = ReadConfiguration();

        return config.Provider.ToLower() switch
        {
            "console" => new ConsoleLogger(),
            "file" => new BufferedFileLogger(config.FilePath, config.BufferSize, config.FlushIntervalMs),
            "cloud" => new CloudLogger(),
            "composite" => new CompositeLogger(
                new ConsoleLogger(),
                new BufferedFileLogger(config.FilePath, config.BufferSize, config.FlushIntervalMs)
            ),
            _ => new ConsoleLogger()
        };
    }

    private static LoggingConfiguration ReadConfiguration()
    {
        try
        {
            if (!File.Exists("appsettings.json"))
            {
                Console.WriteLine("appsettings.json not found, using default configuration");
                return new LoggingConfiguration();
            }

            string jsonString = File.ReadAllText("appsettings.json");
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var appConfig = JsonSerializer.Deserialize<AppConfiguration>(jsonString, options);

            return appConfig?.Logging ?? new LoggingConfiguration();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Configuration read failed: {ex.Message}");
            return new LoggingConfiguration();
        }
    }

    // ✅ LSP-COMPLIANT FLUSH - Only flushes if logger supports it
    public void FlushIfSupported()
    {
        if (_logger is IFlushableLogger flushableLogger)
        {
            flushableLogger.Flush();
            Console.WriteLine($"Flushed logger. Pending logs: {flushableLogger.HasPendingLogs}");
        }
        else
        {
            Console.WriteLine("Current logger doesn't require flushing.");
        }
    }

    public void Dispose()
    {
        if (_logger is IDisposable disposableLogger)
        {
            disposableLogger.Dispose();
        }
    }
}

// ✅ ENHANCED STATIC FACADE - Now LSP Compliant
public static class Logger
{
    public static void Log(LogLevel level, string message,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        LoggerManager.Logger.Log(level, message, memberName, filePath, lineNumber);
    }

    public static void LogException(Exception ex,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        LoggerManager.Logger.LogException(ex, memberName, filePath, lineNumber);
    }

    public static void Info(string message,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        LoggerManager.Logger.Log(LogLevel.Info, message, memberName, filePath, lineNumber);
    }

    public static void Warning(string message,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        LoggerManager.Logger.Log(LogLevel.Warning, message, memberName, filePath, lineNumber);
    }

    public static void Error(string message,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        LoggerManager.Logger.Log(LogLevel.Error, message, memberName, filePath, lineNumber);
    }

    // ✅ SAFE FLUSH - Only attempts flush if logger supports it
    public static void FlushIfSupported()
    {
        LoggerManager.Instance.FlushIfSupported();
    }

    // ✅ EXPLICIT FLUSH - For when you know the logger supports it
    public static void Flush()
    {
        var flushableLogger = LoggerManager.FlushableLogger;
        if (flushableLogger != null)
        {
            flushableLogger.Flush();
        }
        else
        {
            Console.WriteLine("Current logger doesn't support flushing.");
        }
    }
}

// Example usage classes
public class UserService
{
    public void CreateUser(string userName)
    {
        Logger.Info($"Creating user: {userName}");

        try
        {
            if (string.IsNullOrEmpty(userName))
                throw new ArgumentException("Username cannot be empty");

            Logger.Info($"User {userName} created successfully");
        }
        catch (Exception ex)
        {
            Logger.LogException(ex);
            throw;
        }
    }
}

// Main Program demonstrating LSP compliance
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== LSP-Compliant Logger Demo ===");

        var userService = new UserService();

        // Test logging
        Logger.Info("Application started");
        userService.CreateUser("JohnDoe");

        // ✅ LSP-Compliant flushing - adapts based on logger capability
        Logger.FlushIfSupported();

        // Demonstrate type checking
        if (LoggerManager.FlushableLogger != null)
        {
            Console.WriteLine($"Current logger supports flushing. Pending logs: {LoggerManager.FlushableLogger.HasPendingLogs}");
        }
        else
        {
            Console.WriteLine("Current logger is immediate-write (no buffering).");
        }

        Console.WriteLine("\n=== Demo Complete ===");
        Console.ReadKey();
    }
}