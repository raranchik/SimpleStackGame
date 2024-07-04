using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Logger
{
    public class LoggerDecorator : ILogger
    {
        private readonly ILogger m_Logger;

        public ILogHandler logHandler
        {
            get => m_Logger.logHandler;
            set => m_Logger.logHandler = value;
        }

        public bool logEnabled
        {
            get => m_Logger.logEnabled;
            set => m_Logger.logEnabled = value;
        }

        public LogType filterLogType
        {
            get => m_Logger.filterLogType;
            set => m_Logger.filterLogType = value;
        }

        internal LoggerDecorator(ILogger logger)
        {
            m_Logger = logger;
        }

        public virtual void LogFormat(LogType logType, Object context, string format, params object[] args)
        {
            m_Logger.LogFormat(logType, context, format, args);
        }

        public virtual void LogException(Exception exception, Object context)
        {
            m_Logger.LogException(exception, context);
        }

        public virtual bool IsLogTypeAllowed(LogType logType)
        {
            return m_Logger.IsLogTypeAllowed(logType);
        }

        public virtual void Log(LogType logType, object message)
        {
            m_Logger.Log(logType, message);
        }

        public virtual void Log(LogType logType, object message, Object context)
        {
            m_Logger.Log(logType, message, context);
        }

        public virtual void Log(LogType logType, string tag, object message)
        {
            m_Logger.Log(logType, tag, message);
        }

        public virtual void Log(LogType logType, string tag, object message, Object context)
        {
            m_Logger.Log(logType, tag, message, context);
        }

        public virtual void Log(object message)
        {
            m_Logger.Log(message);
        }

        public virtual void Log(string tag, object message)
        {
            m_Logger.Log(tag, message);
        }

        public virtual void Log(string tag, object message, Object context)
        {
            m_Logger.Log(tag, message, context);
        }

        public virtual void LogWarning(string tag, object message)
        {
            m_Logger.LogWarning(tag, message);
        }

        public virtual void LogWarning(string tag, object message, Object context)
        {
            m_Logger.LogWarning(tag, message, context);
        }

        public virtual void LogError(string tag, object message)
        {
            m_Logger.LogError(tag, message);
        }

        public virtual void LogError(string tag, object message, Object context)
        {
            m_Logger.LogError(tag, message, context);
        }

        public virtual void LogFormat(LogType logType, string format, params object[] args)
        {
            m_Logger.LogFormat(logType, format, args);
        }

        public virtual void LogException(Exception exception)
        {
            m_Logger.LogException(exception);
        }
    }
}