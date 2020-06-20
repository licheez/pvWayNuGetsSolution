using System.Threading.Tasks;
using pvWay.MethodResultWrapper.Core;

namespace pvWay.ViesApi.Core
{
    public interface IViesService
    {
        Task<IMethodResult<IViesResult>> CheckVatAsync(string countryCode, string vatNumber);
    }
}