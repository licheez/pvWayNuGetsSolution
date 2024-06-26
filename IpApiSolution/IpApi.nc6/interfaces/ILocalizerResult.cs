﻿namespace pvWay.IpApi.nc6.interfaces;

public interface ILocalizerResult
{
    bool Success { get; }
    bool Failure { get; }
    Exception? Exception { get; }
    ILocalization? Data { get; }
}