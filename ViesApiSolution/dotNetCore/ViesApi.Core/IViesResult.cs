﻿using System;

namespace pvWay.ViesApi.Core
{
    public interface IViesResult
    {
        bool Success { get; }
        bool Failure { get; }

        /// <summary>
        /// Exception is only set on Failure
        /// </summary>
        Exception Exception { get; }

        /// <summary>
        /// Data is only set on Success
        /// </summary>
        IViesData Data { get; }
    }
}