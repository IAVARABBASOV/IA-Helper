using System.Text;
using UnityEngine;


namespace IA.Utils
{
    public static class DebugExtensions
    {
        public struct LogInfo
        {
            public string Message { get; private set; }
            public Object Context { get; private set; }

            public LogInfo(string _message, Object _context)
            {
                Message = _message;
                Context = _context;
            }
        }

        //   public record Person(string Something);


        private static LogInfo CreateLogInfo(string message, Color _color = default, Object _context = default)
        {
            string colorStringRGB = _color.IsNull() ? ColorUtility.ToHtmlStringRGB(Color.white) : ColorUtility.ToHtmlStringRGB(_color);

            string logMessage = $"<color=#{colorStringRGB}>{message}</color>";

            LogInfo log = new LogInfo(logMessage, _context);

            return log;
        }

        public static void Log(this string message, Color _color = default, Object _context = default)
        {
            LogInfo log = CreateLogInfo(message, _color, _context);
            Debug.Log(log.Message, log.Context);
        }

        public static void LogError(this string message, Color _color = default, Object _context = default)
        {
            LogInfo log = CreateLogInfo(message, _color, _context);
            Debug.LogError(log.Message, log.Context);
        }

        public static void LogWarning(this string message, Color _color = default, Object _context = default)
        {
            LogInfo log = CreateLogInfo(message, _color, _context);
            Debug.LogWarning(log.Message, log.Context);
        }

        public static void LogAssertion(this string message, Color _color = default, Object _context = default)
        {
            LogInfo log = CreateLogInfo(message, _color, _context);
            Debug.LogAssertion(log.Message, log.Context);
        }
    }
}