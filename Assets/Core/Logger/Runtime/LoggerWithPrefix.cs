using UnityEngine;

namespace Core.Logger
{
    public class LoggerWithPrefix : LoggerDecorator
    {
        private readonly object m_Prefix;

        internal LoggerWithPrefix(ILogger logger, object prefix) : base(logger)
        {
            m_Prefix = prefix;
        }

        public override void Log(object message)
        {
            base.Log(message.AddPrefix(m_Prefix));
        }
    }
}