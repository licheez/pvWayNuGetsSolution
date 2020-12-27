using System;
using System.Collections.Generic;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace pvWay.MailJetClientNuGet.Core
{
    internal class MailJetSendResult :
        IMailJetSendResult
    {
        public bool Success { get; }
        public Exception Exception { get; }
        public IEnumerable<MailJetMessageSendResult> Messages { get; set; }

        IEnumerable<IMailJetMessageSendResult> IMailJetSendResult.Messages => Messages;

        public MailJetSendResult()
        {
            Success = true;
        }

        private MailJetSendResult(Exception e)
        {
            Success = false;
            Exception = e;
        }

        public static IMailJetSendResult Failure(Exception e)
        {
            return new MailJetSendResult(e);
        }

    }
}