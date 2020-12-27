using System;
using System.Collections.Generic;

namespace pvWay.MailJetClientNuGet.Fw
{
    public interface IMailJetSendResult
    {
        bool Success { get; }
        Exception Exception { get; }
        IEnumerable<IMailJetMessageSendResult> Messages { get; }
    }
}