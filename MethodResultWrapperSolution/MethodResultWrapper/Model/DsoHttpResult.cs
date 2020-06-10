using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using pvWay.MethodResultWrapper.Enums;
using pvWay.MethodResultWrapper.Interfaces;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace pvWay.MethodResultWrapper.Model
{
    /// <summary>
    /// Downstream object Http Result
    /// </summary>
    public class DsoHttpResult : IDsoHttpResult
    {
        internal HttpStatusCode HttpStatusCode =>
            Status == SeverityEnum.Fatal
            || Status == SeverityEnum.Error
                ? HttpStatusCode.InternalServerError
                : HttpStatusCode.OK;

        internal SeverityEnum Status => EnumSeverity.GetValue(StatusCode);

        public string StatusCode { get; set; }
        public string MutationCode { get; set; }
        public IEnumerable<IDsoHttpResultNotification> Notifications { get; set; }
        public bool HasMoreResults { get; set; }

        /// <summary>
        /// Successful constructor 
        /// </summary>
        public DsoHttpResult()
        {
            StatusCode = EnumSeverity.GetCode(SeverityEnum.Ok);
            Notifications = new List<DsoHttpResultNotification>();
            MutationCode = EnumDsoHttpResultMutation
                .GetCode(DsoHttpResultMutationEnum.None);
        }

        /// <summary>
        /// Enables to set the StatusCode
        /// </summary>
        /// <param name="severity"></param>
        public DsoHttpResult(SeverityEnum severity)
        {
            StatusCode = EnumSeverity.GetCode(severity);
            Notifications = new List<DsoHttpResultNotification>();
            MutationCode = EnumDsoHttpResultMutation
                .GetCode(DsoHttpResultMutationEnum.None);
        }

        /// <summary>
        /// Successful constructor passing the mutation type
        /// </summary>
        /// <param name="mutation"></param>
        public DsoHttpResult(DsoHttpResultMutationEnum mutation) :
            this(SeverityEnum.Ok, false, mutation)
        {
        }

        protected DsoHttpResult(
            SeverityEnum severity,
            bool hasMoreResults,
            DsoHttpResultMutationEnum mutation = DsoHttpResultMutationEnum.None) :
            this()
        {
            StatusCode = EnumSeverity.GetCode(severity);
            MutationCode = EnumDsoHttpResultMutation.GetCode(mutation);
            HasMoreResults = hasMoreResults;
        }

        /// <summary>
        /// Failure constructor passing back a MethodResult
        /// </summary>
        /// <param name="res"></param>
        public DsoHttpResult(
            IMethodResult res) :
            this(res.Severity, false)
        {
            Notifications = res.Notifications.Select(
                    x => new DsoHttpResultNotification(x.Severity, x.Message))
                .ToList();
        }

        public DsoHttpResult(Exception e) :
            this(new MethodResult(e))
        {
        }

    }

    public class DsoHttpResult<T> : DsoHttpResult, IDsoHttpResult<T>
    {
        public T Data { get; set; }

        public DsoHttpResult()
        {
        }
        
        /// <summary>
        /// Successful constructor passing back some data
        /// </summary>
        /// <param name="data"></param>
        public DsoHttpResult(T data)
        {
            Data = data;
        }

        /// <summary>
        /// Successful constructor passing back some data and extra parameters
        /// </summary>
        /// <param name="data"></param>
        /// <param name="hasMoreResults"></param>
        public DsoHttpResult(
            T data,
            bool hasMoreResults) :
            base(SeverityEnum.Ok, hasMoreResults)
        {
            Data = data;
        }

        /// <summary>
        /// Successful constructor passing back some data and extra parameters
        /// </summary>
        /// <param name="data"></param>
        /// <param name="mutation"></param>
        public DsoHttpResult(
            T data,
            DsoHttpResultMutationEnum mutation) :
            base(SeverityEnum.Ok, false, mutation)
        {
            Data = data;
        }

        /// <summary>
        /// Failure constructor passing back a MethodResult
        /// </summary>
        /// <param name="res"></param>
        public DsoHttpResult(
            IMethodResult res) :
            base(res)
        {
        }

        public DsoHttpResult(Exception e) :
            base(e)
        {
        }
    }

}
