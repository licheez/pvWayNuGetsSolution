using System.Collections.Generic;
using System.Net;
using pvWay.MethodResultWrapper.Enums;

namespace pvWay.MethodResultWrapper.Interfaces
{
    public interface IDsoHttpResult
    {
        HttpStatusCode HttpStatusCode { get; }
        SeverityEnum Status { get; }

        /// <summary>
        /// Status O (ok) W (warning) E (error) F (fatal)...
        /// </summary>
        string StatusCode { get; }
        /// <summary>
        /// The mutation performed if any N (none) C (create) U (update) D (delete)
        /// </summary>
        string MutationCode { get; }
        /// <summary>
        /// useful for paging
        /// </summary>
        bool HasMoreResults { get; }
        IEnumerable<IDsoHttpResultNotification> Notifications { get; }
    }

    public interface IDsoHttpResult<out T>: IDsoHttpResult
    {
        T Data { get; }
    }

}