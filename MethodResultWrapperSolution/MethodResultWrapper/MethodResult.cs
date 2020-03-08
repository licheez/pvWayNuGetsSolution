using System;
using System.Collections.Generic;
using System.Linq;

namespace pvWay.MethodResultWrapper
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

        protected MethodResult()
        {
            _notifications = new List<IMethodResultNotification>();
        }

        public MethodResult(IMethodResult res)
            : this()
        {
            foreach (var notification in res.Notifications)
            {
                AddNotification(notification);
            }
        }

        public MethodResult(Exception e, SeverityEnum severity = SeverityEnum.Fatal)
            : this(e.GetDeepMessage(), severity)
        {
        }

        public MethodResult(string message, SeverityEnum severity) :
            this()
        {
            AddNotification(message, severity);
        }

        public MethodResult(IEnumerable<string> messages, SeverityEnum severity) :
            this()
        {
            foreach (var message in messages)
            {
                AddNotification(message, severity);
            }
        }

        private void AddNotification(string message, SeverityEnum severity)
        {
            AddNotification(new Notification(severity, message));
        }

        private void AddNotification(IMethodResultNotification notification)
        {
            _notifications.Add(notification);
        }

        public bool Failure => _notifications
                    .Any(n => n.Severity >= SeverityEnum.Error);

        public bool Success => !Failure;

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
            var str = string.Empty;
            foreach (var notification in _notifications)
            {
                if (!string.IsNullOrEmpty(str))
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

        public MethodResult(T data)
        {
            Data = data;
        }

        public MethodResult(IMethodResult methodResult) :
            base(methodResult)
        {
        }

        public MethodResult(string message, SeverityEnum severity)
            : base(message, severity)
        {
        }

        public MethodResult(IEnumerable<string> messages, SeverityEnum severity)
            : base(messages, severity)
        {
        }

        public MethodResult(Exception e, SeverityEnum severity = SeverityEnum.Fatal)
            : base(e, severity)
        {

        }
    }
}
