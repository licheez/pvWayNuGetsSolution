using System;
using System.Collections.Generic;

namespace pvWay.MailJetClientNuGet.Core
{
    public interface IMailJetSendResult
    {
        bool Success { get; }
        Exception Exception { get; }
        IEnumerable<IMailJetMessageSendResult> Messages { get; }
    }
}