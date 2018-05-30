using System;
using log4net.Appender;
using log4net.Core;

namespace myzy.Util
{
    public class UIAppender : OutputDebugStringAppender
    {
        protected override void Append(LoggingEvent loggingEvent)
        {
            if (LogMsgEvent != null)
            {
                var msg = this.RenderLoggingEvent(loggingEvent);
                LogMsgEvent.Invoke(msg);
            }
        }
        public static event Action<string> LogMsgEvent;
    }
}
