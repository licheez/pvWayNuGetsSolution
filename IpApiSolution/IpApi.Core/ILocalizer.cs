using System.Threading.Tasks;
using pvWay.MethodResultWrapper.Core;

namespace pvWay.IpApi.Core
{
    public interface ILocalizer
    {
        Task<IMethodResult<ILocalization>> LocalizeAsync(string ip);
    }
}