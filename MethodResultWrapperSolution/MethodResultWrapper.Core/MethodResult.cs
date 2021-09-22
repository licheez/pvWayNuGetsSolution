using System;
using System.Collections.Generic;
using System.Linq;

namespace pvWay.MethodResultWrapper.Core
{
    public class MethodResult : IMethodResult
    {
        private readonly ICollection<IMethodResultNotification> _notifications;

        private class Notification : IMethodResultNotification
        {
            public SeverityEnum Severity { get; }
            public string Message { get; }

            public Notification(SeverityEnum severity, string message)
            {
                Severity = severity;
                Message = message;
            }

            public override string ToString()
            {
                return $"{Severity}:{Message}";
            }
        }

        /// <summary>
        /// Successful constructor
        /// </summary>
        public MethodResult()
        {
            _notifications = new List<IMethodResultNotification>();
        }

        /// <summary>
        /// Wraps the result of a previous MethodResult
        /// by copying all its notifications (message and severity).
        /// Not really useful for the non generic MethodResult class
        /// but makes sense when using the generic version of the class
        /// </summary>
        /// <param name="res"></param>
        public MethodResult(IMethodResult res)
            : this()
        {
            foreach (var notification in res.Notifications)
            {
                AddNotification(notification);
            }
        }

        /// <summary>
        /// Instantiates a MethodResult object with one
        /// notification (message and severity)
        /// </summary>
        /// <param name="message"></param>
        /// <param name="severity"></param>
        public MethodResult(string message, SeverityEnum severity) :
            this()
        {
            AddNotification(message, severity);
        }

        /// <summary>
        /// Failure constructor that instantiates a MethodResult
        /// object with one notification. The notification message
        /// is built by recursively walking down the exception
        /// and its inner exceptions (using the extension method
        /// e.GetDeepMessage()). The stack trace is also added at
        /// the end of the message. By default the Severity is
        /// considered as Fatal but you select the one of your choice
        /// </summary>
        /// <param name="e"></param>
        /// <param name="severity"></param>
        public MethodResult(Exception e, SeverityEnum severity = SeverityEnum.Fatal)
            : this(e.GetDeepMessage(), severity)
        {
        }

        /// <summary>
        /// Same as the single message constructor but this time
        /// passing a list of messages. This creates one notification
        /// for each message in the list. All notifications get the
        /// same severity
        /// </summary>
        /// <param name="messages"></param>
        /// <param name="severity"></param>
        public MethodResult(IEnumerable<string> messages, SeverityEnum severity) :
            this()
        {
            foreach (var message in messages)
            {
                AddNotification(message, severity);
            }
        }

        public void AddNotification(string message, SeverityEnum severity)
        {
            AddNotification(new Notification(severity, message));
        }

        public void AddNotification(IMethodResultNotification notification)
        {
            _notifications.Add(notification);
        }

        /// <summary>
        /// At least one notification has a severity
        /// greater or equal to Error
        /// </summary>
        public bool Failure => _notifications
                    .Any(n => n.Severity >= SeverityEnum.Error);

        /// <summary>
        /// No notification or all notifications severity
        /// are lower than Error
        /// </summary>
        public bool Success => !Failure;

        /// <summary>
        /// returns the highest severity from the list of notifications
        /// </summary>
        public SeverityEnum Severity =>
            _notifications.Any()
            ? _notifications.Max(x => x.Severity)
            : SeverityEnum.Ok;

        public IEnumerable<IMethodResultNotification> Notifications =>
            _notifications;

        public void Throw()
        {
            throw new Exception(ErrorMessage);
        }

        public string ErrorMessage => ToString();

        public override string ToString()
        {
            var str = String.Empty;
            foreach (var notification in _notifications)
            {
                if (!String.IsNullOrEmpty(str))
                    str += Environment.NewLine;
                str += notification.ToString();
            }
            return str;
        }

        public static MethodResult Ok => new MethodResult();

    }

    public class MethodResult<T> : MethodResult, IMethodResult<T>
    {
        public T Data { get; }

        /// <summary>
        /// Successful constructor that carries
        /// the result of the called method in the
        /// property Data of type T
        /// </summary>
        /// <param name="data"></param>
        public MethodResult(T data)
        {
            Data = data;
        }

        /// <summary>
        /// Wraps the result of a previous MethodResult
        /// by copying all its notifications (message and severity).
        /// Not really useful for the non generic MethodResult class
        /// but makes sense when using the generic version of the class
        /// </summary>
        /// <param name="methodResult"></param>
        public MethodResult(IMethodResult methodResult) :
            base(methodResult)
        {
        }

        /// <summary>
        /// Instantiates a MethodResult object with one
        /// notification (message and severity)
        /// </summary>
        /// <param name="message"></param>
        /// <param name="severity"></param>
        public MethodResult(string message, SeverityEnum severity)
            : base(message, severity)
        {
        }

        /// <summary>
        /// Same as the single message constructor but this time
        /// passing a list of messages. This creates one notification
        /// for each message in the list. All notifications get the
        /// same severity
        /// </summary>
        /// <param name="messages"></param>
        /// <param name="severity"></param>
        public MethodResult(IEnumerable<string> messages, SeverityEnum severity)
            : base(messages, severity)
        {
        }

        /// <summary>
        /// Failure constructor that instantiates a MethodResult
        /// object with one notification. The notification message
        /// is built by recursively walking down the exception
        /// and its inner exceptions (using the extension method
        /// e.GetDeepMessage()). The stack trace is also added at
        /// the end of the message. By default the Severity is
        /// considered as Fatal but you select the one of your choice
        /// </summary>
        /// <param name="e"></param>
        /// <param name="severity"></param>
        public MethodResult(Exception e, SeverityEnum severity = SeverityEnum.Fatal)
            : base(e, severity)
        {
        }
    }
}
