using System;
using UnityEngine;

namespace Logger
{
    public static class LoggerExtensions
    {
        public static ILogger WithPrefix(this ILogger logger, object prefix) => new LoggerWithPrefix(logger, prefix);

        public static object AddPrefix(this object message, object prefix) => $"{prefix} {message}";

        public static object ToPrefix(this string raw) => $"[{raw}]:";

        public static object ToPrefix(this Type raw) => ToPrefix(raw.Name);
    }
}